using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Commands
{
    public class CleanupUnusedFilesCommand : IRequest
    {
        public class CleanupUnusedFilesCommandHandler : IRequestHandler<CleanupUnusedFilesCommand>
        {
            private readonly IDeferredDbContext _dbContext;
            private readonly IApplicationSettingService _applicationSettingService;
            private readonly IFileService _fileService;
            private readonly IDateTimeService _dateTimeService;

            public CleanupUnusedFilesCommandHandler(
                IDeferredDbContext dbContext,
                IApplicationSettingService applicationSettingService,
                IFileService fileService,
                IDateTimeService dateTimeService)
            {
                _dbContext = dbContext;
                _applicationSettingService = applicationSettingService;
                _fileService = fileService;
                _dateTimeService = dateTimeService;
            }

            public async Task<Unit> Handle(CleanupUnusedFilesCommand request, CancellationToken cancellationToken)
            {
                if (_dbContext.ChangesExist)
                {
                    return Unit.Value;
                }

                await RemoveUnusedFiles();

                return Unit.Value;
            }

            private async Task RemoveUnusedFiles()
            {
                var toDelete = new List<(string, AssetType.Value)>();

                toDelete.AddRange(await GetUnusedFiles<Product>(m => m.ImagePath, AssetType.Value.ProductImage));
                toDelete.AddRange(await GetUnusedFiles<ProductAttachment>(m => m.FilePath, AssetType.Value.ProductAttachment));
                toDelete.AddRange(await GetUnusedLandingCarouselItems());

                foreach (var (name, type) in toDelete)
                {
                    _fileService.Delete(type, name);
                }
            }

            private async Task<IEnumerable<(string, AssetType.Value)>> GetUnusedFiles<TEntity>
            (
                Expression<Func<TEntity, string>> pathSelector,
                AssetType.Value fileType
            ) where TEntity : class
            {
                var usedFiles = (await _dbContext.Set<TEntity>()
                    .Select(pathSelector)
                    .ToListAsync())
                    .Select(path => path.Split("/").Last());

                return GetFiles(fileType)
                    .Except(usedFiles)
                    .Select(fileName => (fileName, fileType));
            }

            private async Task<IEnumerable<(string, AssetType.Value)>> GetUnusedLandingCarouselItems()
            {
                var dto = await _applicationSettingService.GetAsync<LandingCarouselSettingsDTO>();

                var usedFiles = dto.Items
                    .Select(item => item.Path)
                    .Select(path => path.Split("/").Last());

                return GetFiles(AssetType.Value.LandingCarouselItem)
                    .Except(usedFiles)
                    .Select(fileName => (fileName, AssetType.Value.LandingCarouselItem));
            }

            private IEnumerable<string> GetFiles(AssetType.Value fileType)
            {
                return _fileService.GetFiles(fileType)
                    //.Where(fi => fi.CreationTime < _dateTimeService.Today)
                    .Select(fi => fi.Name)
                    .ToList();
            }
        }
    }
}
