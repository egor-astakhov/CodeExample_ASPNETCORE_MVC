using Integral.Application.Common.Mappings;

namespace Integral.Application.ApplicationSettings.Data
{
    public class EmailServiceSettingsDTO : IMapFrom<EmailServiceSettingsViewModel>
    {
        public string Host { get; set; }

        public int Port { get; set;  }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SupportEmail { get; set; }
    }
}
