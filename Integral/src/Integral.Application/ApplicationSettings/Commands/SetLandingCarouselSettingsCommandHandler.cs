using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.ApplicationSettings.Commands
{
    public class SetLandingCarouselSettingsCommandHandler : AsyncRequestHandler<LandingCarouselSettingsViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IFileService _fileService;

        public SetLandingCarouselSettingsCommandHandler(
            IDeferredDbContext dbContext,
            IApplicationSettingService applicationSettingService,
            IFileService fileService
            )
        {
            _dbContext = dbContext;
            _applicationSettingService = applicationSettingService;
            _fileService = fileService;
        }

        protected override async Task Handle(LandingCarouselSettingsViewModel request, CancellationToken cancellationToken)
        {
            var existingDto = await _applicationSettingService.GetAsync<LandingCarouselSettingsDTO>();
            var newDto = new LandingCarouselSettingsDTO();

            foreach (var requestItem in request.Items)
            {
                if (requestItem.File == null)
                {
                    AddExisting(existingDto, newDto, requestItem);
                }
                else
                {
                    await AddNew(newDto, requestItem);
                }
            };

            await _applicationSettingService.SetAsync(newDto);

            await _dbContext.SaveChangesAsync();
        }

        private static void AddExisting(LandingCarouselSettingsDTO existingDto, LandingCarouselSettingsDTO newDto, LandingCarouselSettingsViewModel.Item requestItem)
        {
            var existingItem = existingDto.Items
                .FirstOrDefault(m => m.Path == requestItem.Path) 
                ?? throw new ApplicationException($"File {requestItem.DisplayName} not found in the existing setting.");

            existingDto.Items.Remove(existingItem);

            existingItem.Text = requestItem.Text;

            newDto.Items.Add(existingItem);
        }

        private async Task AddNew(LandingCarouselSettingsDTO newDto, LandingCarouselSettingsViewModel.Item requestItem)
        {
            var path = await _fileService.CreateAsync(AssetType.Value.LandingCarouselItem, requestItem.File);

            var newItem = new LandingCarouselSettingsDTO.Item
            {
                DisplayName = requestItem.DisplayName,
                Text = requestItem.Text,
                Path = path
            };

            newDto.Items.Add(newItem);
        }
    }
}
