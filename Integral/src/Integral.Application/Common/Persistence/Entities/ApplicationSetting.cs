namespace Integral.Application.Common.Persistence.Entities
{
    public class ApplicationSetting
    {
        public const string COMMON_SETTINGS_KEY = "Common.Settings";
        public const string EMAIL_SERVICE_SETTINGS_KEY = "EmailService.Settings";
        public const string LANDING_CAROUSEL_SETTINGS_KEY = "Landing.Carousel.Settings";

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
