using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Queries
{
    public class GetBackupFileQuery : IRequest<FileDTO>
    {
        public GetBackupFileQuery(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public class GetBackupFileQueryHandler : IRequestHandler<GetBackupFileQuery, FileDTO>
        {
            private readonly IFileService _fileService;

            public GetBackupFileQueryHandler(IFileService fileService)
            {
                _fileService = fileService;
            }

            public async Task<FileDTO> Handle(GetBackupFileQuery request, CancellationToken cancellationToken)
            {
                var dto = new FileDTO
                {
                    Name = request.Name,
                    Stream = _fileService.GetFileStream(FileType.Value.Backup, request.Name)
                };

                return await Task.FromResult(dto);
            }
        }
    }
}
