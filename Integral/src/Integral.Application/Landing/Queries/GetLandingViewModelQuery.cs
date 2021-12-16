using AutoMapper;
using AutoMapper.QueryableExtensions;
using Integral.Application.Common.Persistence;
using Integral.Application.Landing.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Landing.Queries
{
    public class GetLandingViewModelQuery : IRequest<LandingViewModel>
    {
        public class GetLandingViewModelQueryHandler : IRequestHandler<GetLandingViewModelQuery, LandingViewModel>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public GetLandingViewModelQueryHandler(IDeferredDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<LandingViewModel> Handle(GetLandingViewModelQuery request, CancellationToken cancellationToken)
            {
                var products = await _dbContext.Products
                    .OrderBy(m => m.SortingOrder)
                    .ProjectTo<LandingProductViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new LandingViewModel
                {
                    Products = products
                };
            }
        }
    }
}
