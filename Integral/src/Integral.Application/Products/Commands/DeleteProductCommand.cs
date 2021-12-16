using Integral.Application.Common.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Products.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public DeleteProductCommand(int productId)
        {
            ProductId = productId;
        }

        public int ProductId { get; }

        public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
        {
            private readonly IApplicationDbContext _dbContext;

            public DeleteProductCommandHandler(IDeferredDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
            {
                var product = await _dbContext.Products.FindAsync(request.ProductId);

                _dbContext.Products.Remove(product);

                await _dbContext.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
