using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Queries
{
    public class GetProductAttachmentFileQuery : IRequest<FileDTO>
    {
        public GetProductAttachmentFileQuery(int attachmentId)
        {
            AttachmentId = attachmentId;
        }

        public int AttachmentId { get; }

        public class GetProductAttachmentFileQueryHandler : IRequestHandler<GetProductAttachmentFileQuery, FileDTO>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IFileService _fileService;

            public GetProductAttachmentFileQueryHandler(IDeferredDbContext dbContext, IFileService fileService)
            {
                _dbContext = dbContext;
                _fileService = fileService;
            }

            public async Task<FileDTO> Handle(GetProductAttachmentFileQuery request, CancellationToken cancellationToken)
            {
                var attachment = await _dbContext.ProductAttachments.FindAsync(request.AttachmentId);
                if (attachment == null)
                {
                    return null;
                }

                return new FileDTO
                {
                    Name = attachment.FileName,
                    Stream = _fileService.GetFileStream(FileType.Value.Asset, attachment.FilePath)
                };
            }
        }
    }
}
