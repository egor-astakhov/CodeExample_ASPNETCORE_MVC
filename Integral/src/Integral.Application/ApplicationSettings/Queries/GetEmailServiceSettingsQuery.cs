using AutoMapper;
using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.ApplicationSettings.Queries
{
    public class GetEmailServiceSettingsQuery : IRequest<EmailServiceSettingsViewModel>
    {
        public class GetEmailServiceSettingsQueryHandler 
            : IRequestHandler<GetEmailServiceSettingsQuery, EmailServiceSettingsViewModel>
        {
            private readonly IApplicationSettingService _applicationSettingService;
            private readonly IMapper _mapper;

            public GetEmailServiceSettingsQueryHandler(
                IApplicationSettingService applicationSettingService,
                IMapper mapper)
            {
                _applicationSettingService = applicationSettingService;
                _mapper = mapper;
            }

            public async Task<EmailServiceSettingsViewModel> Handle(GetEmailServiceSettingsQuery request, CancellationToken cancellationToken)
            {
                var setting = await _applicationSettingService.GetAsync<EmailServiceSettingsDTO>();
                
                return _mapper.Map<EmailServiceSettingsViewModel>(setting);
            }
        }
    }
}
