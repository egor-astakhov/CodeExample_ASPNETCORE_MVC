using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Services;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Services
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        private readonly IApplicationSettingService _applicationSettingService;

        public EmailServiceFactory(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }

        public async Task<IEmailService> GetInstanceAsync()
        {
            var settings = await _applicationSettingService.GetAsync<EmailServiceSettingsDTO>();

            return new EmailService(settings);
        }
    }
}
