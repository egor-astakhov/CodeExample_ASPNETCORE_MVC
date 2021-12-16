using AutoMapper;
using Integral.Application.Common.Persistence;
using Integral.Application.Products.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Products.Queries
{
    public class GetProductForEditingQuery : IRequest<EditProductViewModel>
    {
        public GetProductForEditingQuery(int? productId)
        {
            ProductId = productId;
        }

        public int? ProductId { get; }

        public class GetProductForEditingQueryHandler
            : IRequestHandler<GetProductForEditingQuery, EditProductViewModel>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public GetProductForEditingQueryHandler(IDeferredDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<EditProductViewModel> Handle(GetProductForEditingQuery request, CancellationToken cancellationToken)
            {
                var product = await _dbContext.Products
                    .Include(m => m.Specifications)
                    .Include(m => m.Attachments)
                    .FirstOrDefaultAsync(m => m.Id == request.ProductId);

                if (product == null)
                {
                    return new EditProductViewModel();
                }

                return _mapper.Map<EditProductViewModel>(product);
            }
        }
    }
}
