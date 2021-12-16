using AutoMapper;
using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.ApplicationSettings.Queries
{
    public class GetCommonSettingsQuery : IRequest<CommonSettingsViewModel>
    {
        public class GetCommonSettingsQueryHandler
            : IRequestHandler<GetCommonSettingsQuery, CommonSettingsViewModel>
        {
            private readonly IApplicationSettingService _applicationSettingService;
            private readonly IMapper _mapper;

            public GetCommonSettingsQueryHandler(
                IApplicationSettingService applicationSettingService,
                IMapper mapper)
            {
                _applicationSettingService = applicationSettingService;
                _mapper = mapper;
            }

            public async Task<CommonSettingsViewModel> Handle(GetCommonSettingsQuery request, CancellationToken cancellationToken)
            {
                var setting = await _applicationSettingService.GetAsync<CommonSettingsDTO>();

                return _mapper.Map<CommonSettingsViewModel>(setting);
            }
        }
    }
}
