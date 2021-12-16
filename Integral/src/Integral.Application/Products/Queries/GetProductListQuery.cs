using AutoMapper;
using AutoMapper.QueryableExtensions;
using Integral.Application.Common.Persistence;
using Integral.Application.Products.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Products.Queries
{
    public class GetProductListQuery : IRequest<ProductListViewModel>
    {
        public class GetProductListQueryHandler
            : IRequestHandler<GetProductListQuery, ProductListViewModel>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public GetProductListQueryHandler(IDeferredDbContext context, IMapper mapper)
            {
                _dbContext = context;
                _mapper = mapper;
            }

            public async Task<ProductListViewModel> Handle(GetProductListQuery request, CancellationToken cancellationToken)
            {
                var products = await _dbContext.Products
                    .OrderBy(m => m.SortingOrder)
                    .ProjectTo<ProductListViewModel.Item>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                var model = new ProductListViewModel
                {
                    Items = products
                };

                return await Task.FromResult(model);
            }
        }
    }
}
