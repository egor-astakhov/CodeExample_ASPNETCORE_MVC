using AutoMapper;
using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.ApplicationSettings.Commands
{ 
    public class SetCommonSettingsCommandHandler : AsyncRequestHandler<CommonSettingsViewModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IApplicationDbContext _dbContext;

        public SetCommonSettingsCommandHandler(
            IMapper mapper,
            IApplicationSettingService applicationSettingService,
            IDeferredDbContext dbContext)
        {
            _mapper = mapper;
            _applicationSettingService = applicationSettingService;
            _dbContext = dbContext;
        }

        protected override async Task Handle(CommonSettingsViewModel request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<CommonSettingsDTO>(request);

            await _applicationSettingService.SetAsync(dto);

            await _dbContext.SaveChangesAsync();
        }
    }
}
