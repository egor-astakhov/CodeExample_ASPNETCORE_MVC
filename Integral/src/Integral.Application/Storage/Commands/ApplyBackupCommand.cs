using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Commands
{
    public class ApplyBackupCommand : IRequest
    {
        public IFormFile File { get; set; }

        public class ApplyBackupCommandHandler : IRequestHandler<ApplyBackupCommand>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IFileService _fileService;

            public ApplyBackupCommandHandler(IApplicationDbContext dbContext, IFileService fileService)
            {
                _dbContext = dbContext;
                _fileService = fileService;
            }

            public async Task<Unit> Handle(ApplyBackupCommand request, CancellationToken cancellationToken)
            {
                using var archive = new ZipArchive(request.File.OpenReadStream(), ZipArchiveMode.Read);
                using var sr = new StreamReader(archive.GetEntry(BackupDTO.FILE_NAME).Open());

                var json = await sr.ReadToEndAsync();

                var backupDTO = JsonConvert.DeserializeObject<BackupDTO>(json);

                await ApplyDbBackupAsync(backupDTO);

                _fileService.ApplyAssetsBackup(archive);
                _fileService.Delete(FileType.Value.Asset, BackupDTO.FILE_NAME);

                return Unit.Value;
            }

            private async Task ApplyDbBackupAsync(BackupDTO backupDTO)
            {
                using var scope = await _dbContext.Database.BeginTransactionAsync();

                await DeleteTable(nameof(_dbContext.ApplicationSettings));
                await DeleteTable(nameof(_dbContext.Products));
                await DeleteTable(nameof(_dbContext.ProductSpecifications));
                await DeleteTable(nameof(_dbContext.ProductAttachments));

                await _dbContext.ApplicationSettings.AddRangeAsync(backupDTO.ApplicationSettings);
                await _dbContext.Products.AddRangeAsync(backupDTO.Products);
                await _dbContext.ProductSpecifications.AddRangeAsync(backupDTO.ProductSpecifications);
                await _dbContext.ProductAttachments.AddRangeAsync(backupDTO.ProductAttachments);

                await _dbContext.SaveChangesAsync();
                await scope.CommitAsync();
            }

            private async Task DeleteTable(string tableName)
            {
                await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
            }
        }
    }
}
