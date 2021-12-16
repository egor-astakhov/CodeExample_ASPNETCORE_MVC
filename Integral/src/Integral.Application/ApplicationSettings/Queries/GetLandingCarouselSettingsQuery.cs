using AutoMapper;
using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.ApplicationSettings.Queries
{
    public class GetLandingCarouselSettingsQuery : IRequest<LandingCarouselSettingsViewModel>
    {
        public class GetLandingCarouselSettingsQueryHandler
            : IRequestHandler<GetLandingCarouselSettingsQuery, LandingCarouselSettingsViewModel>
        {
            private readonly IMapper _mapper;
            private readonly IApplicationSettingService _applicationSettingService;

            public GetLandingCarouselSettingsQueryHandler(
                IMapper mapper,
                IApplicationSettingService applicationSettingService)
            {
                _mapper = mapper;
                _applicationSettingService = applicationSettingService;
            }

            public async Task<LandingCarouselSettingsViewModel> Handle(GetLandingCarouselSettingsQuery request, CancellationToken cancellationToken)
            {
                var setting = await _applicationSettingService.GetAsync<LandingCarouselSettingsDTO>();

                return _mapper.Map<LandingCarouselSettingsViewModel>(setting);
            }
        }
    }
}
