using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Common.Persistence
{
    public interface IApplicationDbContext : IDisposable
    {
        DbContextOptions Options { get; }

        DatabaseFacade Database { get; }

        ChangeTracker ChangeTracker { get; }

        EntityEntry Entry(object entity);

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet<ApplicationSetting> ApplicationSettings { get; }

        DbSet<Product> Products { get; }

        DbSet<ProductSpecification> ProductSpecifications { get;}

        DbSet<ProductAttachment> ProductAttachments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
