using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Commands
{
    public class DeleteBackupCommand : IRequest
    {
        public DeleteBackupCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public class DeleteBackupCommandHandler : IRequestHandler<DeleteBackupCommand>
        {
            private readonly IFileService _fileService;

            public DeleteBackupCommandHandler(IFileService fileService)
            {
                _fileService = fileService;
            }

            public Task<Unit> Handle(DeleteBackupCommand request, CancellationToken cancellationToken)
            {
                _fileService.Delete(FileType.Value.Backup, request.Name);

                return Unit.Task;
            }
        }
    }
}
