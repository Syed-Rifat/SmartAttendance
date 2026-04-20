using Microsoft.Extensions.Configuration;

namespace SmartAttendance.Patterns.Singleton
{
    public class AppSettings
    {
        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public decimal MinAttendanceThreshold
        {
            get
            {
                var val = _configuration["AppSettings:MinAttendanceThreshold"];
                if (decimal.TryParse(val, out decimal threshold))
                {
                    return threshold;
                }
                return 75.0m; // Default
            }
        }
    }
}
