using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SmartAttendance.Models.ViewModels;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Models.Entities;
using System.Linq;

namespace SmartAttendance.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _uow;

        public AccountController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "Admin") return RedirectToAction("Dashboard", "Admin");
                if (role == "Teacher") return RedirectToAction("Dashboard", "Teacher");
                if (role == "Student") return RedirectToAction("Dashboard", "Student");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find user by Username, Email, or StudentCode
                User? user = null;

                // First try by Username or Email
                user = (await _uow.Users.FindAsync(u =>
                    (u.Username == model.LoginId || u.Email == model.LoginId)
                    && u.IsActive
                    && u.Role == model.Role
                )).FirstOrDefault();

                // If not found and role is Student, try by StudentCode
                if (user == null && model.Role == "Student")
                {
                    var student = (await _uow.Students.FindAsync(s => s.StudentCode == model.LoginId)).FirstOrDefault();
                    if (student != null)
                    {
                        user = (await _uow.Users.FindAsync(u =>
                            u.UserId == student.UserId && u.IsActive && u.Role == "Student"
                        )).FirstOrDefault();
                    }
                }

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    if (user.Role == "Admin") return RedirectToAction("Dashboard", "Admin");
                    if (user.Role == "Teacher") return RedirectToAction("Dashboard", "Teacher");
                    if (user.Role == "Student") return RedirectToAction("Dashboard", "Student");
                }

                ModelState.AddModelError("", "Invalid credentials or role mismatch. Please check your login ID, password, and role.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // ================= SETTINGS =================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return NotFound();

            var vm = new ProfileSettingsViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            // Load role-specific profile
            if (user.Role == "Student")
            {
                var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
                if (student != null)
                {
                    vm.FullName = student.FullName;
                    vm.Department = student.Department;
                    vm.ContactNumber = student.ContactNumber;
                }
            }
            else if (user.Role == "Teacher")
            {
                var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
                if (teacher != null)
                {
                    vm.FullName = teacher.FullName;
                    vm.Department = teacher.Department;
                    vm.ContactNumber = teacher.ContactNumber;
                }
            }

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all fields correctly.";
                return RedirectToAction(nameof(Settings));
            }

            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction(nameof(Settings));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully!";
            return RedirectToAction(nameof(Settings));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileSettingsViewModel model)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return NotFound();

            // Update role-specific profile
            if (user.Role == "Student")
            {
                var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
                if (student != null)
                {
                    student.FullName = model.FullName ?? student.FullName;
                    student.Department = model.Department ?? student.Department;
                    student.ContactNumber = model.ContactNumber;
                    _uow.Students.Update(student);
                }
            }
            else if (user.Role == "Teacher")
            {
                var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
                if (teacher != null)
                {
                    teacher.FullName = model.FullName ?? teacher.FullName;
                    teacher.Department = model.Department ?? teacher.Department;
                    teacher.ContactNumber = model.ContactNumber;
                    _uow.Teachers.Update(teacher);
                }
            }

            await _uow.SaveChangesAsync();
            TempData["Message"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Settings));
        }
    }
}
