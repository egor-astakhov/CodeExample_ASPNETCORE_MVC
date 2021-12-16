using AutoMapper;
using AutoMapper.QueryableExtensions;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Data;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Storage.Queries
{
    public class GetBackupsQuery : IRequest<BackupViewModel>
    {
        public class GetBackupsQueryHandler : IRequestHandler<GetBackupsQuery, BackupViewModel>
        {
            private readonly IFileService _fileService;
            private readonly IMapper _mapper;

            public GetBackupsQueryHandler(IFileService fileService, IMapper mapper)
            {
                _fileService = fileService;
                _mapper = mapper;
            }

            public async Task<BackupViewModel> Handle(GetBackupsQuery request, CancellationToken cancellationToken)
            {
                var model = new BackupViewModel
                {
                    Items = _fileService.GetFiles(FileType.Value.Backup)
                        .AsQueryable()
                        .ProjectTo<BackupViewModel.Item>(_mapper.ConfigurationProvider)
                        .ToList()
                };

                return await Task.FromResult(model);
            }
        }
    }
}
