using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Data;
using MediatR;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Commands
{
    public class CreateBackupCommand : IRequest
    {
        public class CreateBackupCommandHandler : IRequestHandler<CreateBackupCommand>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IFileService _fileService;

            public CreateBackupCommandHandler(IApplicationDbContext dbContext, IFileService fileService)
            {
                _dbContext = dbContext;
                _fileService = fileService;
            }

            public async Task<Unit> Handle(CreateBackupCommand request, CancellationToken cancellationToken)
            {
                string backupName = null;

                try
                {
                    backupName = _fileService.CreateAssetsBackup();

                    using var zip = _fileService.GetFileStream(FileType.Value.Backup, backupName, FileAccess.ReadWrite);
                    using var archive = new ZipArchive(zip, ZipArchiveMode.Update);

                    var dbJsonEntry = archive.CreateEntry(BackupDTO.FILE_NAME);

                    using var sw = new StreamWriter(dbJsonEntry.Open(), Encoding.UTF8);

                    var dto = await CreateDbBackupAsync();
                    var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings 
                    { 
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }).ToString();

                    await sw.WriteAsync(json);
                }
                catch
                {
                    if (backupName != null)
                    {
                        _fileService.Delete(FileType.Value.Backup, backupName);
                    }

                    throw;
                }

                return Unit.Value;
            }

            private async Task<BackupDTO> CreateDbBackupAsync()
            {
                var dto = new BackupDTO
                {
                    Version = BackupDTO.BACKUP_VERSION,
                    CreationTime = DateTime.Now,
                };

                await _dbContext.CopyToAsync(dto);

                return dto;
            }
        }
    }
}
