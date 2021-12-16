using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Persistence
{
    public class DeferredDbContext : IDeferredDbContext
    {
        private static readonly IDictionary<string, ICollection<DeferredChange>> _deferredChanges =
            new Dictionary<string, ICollection<DeferredChange>>();

        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public DeferredDbContext(
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService
            )
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;

            DbContext = dbContext;

            if (UserExists)
            {
                MemoryContext = CreateInMemoryContext();
            }
        }

        private IApplicationDbContext DbContext { get; }

        private IApplicationDbContext MemoryContext { get; set; }

        public bool ChangesExist => _deferredChanges.SelectMany(m => m.Value).Any();

        public ICollection<DeferredChange> DeferredChanges 
        { 
            get
            {
                if (!UserExists)
                {
                    return new List<DeferredChange>();
                }

                if (!_deferredChanges.ContainsKey(_currentUserService.UserId))
                {
                    _deferredChanges[_currentUserService.UserId] = new List<DeferredChange>();
                }

                return _deferredChanges[_currentUserService.UserId];
            }
        }

        private IApplicationDbContext Context
        {
            get
            {
                if (DeferredChanges.Any())
                {
                    return MemoryContext;
                }

                return DbContext;
            }
        }

        private async Task InitializeInMemoryContextAsync()
        {
            using var inMemoryContext = CreateInMemoryContext();

            await DbContext.CopyToAsync(inMemoryContext);

            await inMemoryContext.SaveChangesAsync();
        }

        private async Task<int> ApplyDbContextChangesToMemoryAsync()
        {
            foreach (var dbEntry in DbContext.ChangeTracker.Entries())
            {
                MemoryContext.Entry(dbEntry.Entity).State = dbEntry.State;
            }

            return await SaveChangesAsync(DbContext.ChangeTracker);
        }

        private DeferredChange CreateDeferredChange(ChangeTracker changeTracker)
        {
            var entries = changeTracker.Entries().Where(m => m.State != EntityState.Unchanged);

            if (entries.Any())
            {
                return new DeferredChange(entries);
            }

            return null;
        }

        public async Task PersistChangesAsync()
        {
            while (DeferredChanges.Any())
            {
                var change = DeferredChanges.First();

                using (var dbContext = CreateDbContext())
                {
                    foreach (var entry in change.Entries)
                    {
                        dbContext.Entry(entry.Entity).State = entry.State;
                    }

                    await dbContext.SaveChangesAsync();
                }

                DeferredChanges.Remove(change);
            }
            
            DiscardChanges();
        }

        public void DiscardChanges()
        {
            MemoryContext.Database.EnsureDeleted();

            DeferredChanges.Clear();
        }

        private IApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(_currentUserService.UserId)
                .Options;

            return new ApplicationDbContext(options, _currentUserService, _dateTimeService);
        }

        private IApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(DbContext.Options, _currentUserService, _dateTimeService);
        }

        private bool UserExists => _currentUserService.UserId != null;

        private async Task<int> SaveChangesAsync(ChangeTracker changeTracker)
        {
            var change = CreateDeferredChange(changeTracker);

            var result = await MemoryContext.SaveChangesAsync();

            if (change != null)
            {
                DeferredChanges.Add(change);
            }

            return result;
        }

        #region IApplicationDbContext

        public DbContextOptions Options => Context.Options;

        public DbSet<ApplicationSetting> ApplicationSettings => Context.ApplicationSettings;

        public DbSet<Product> Products => Context.Products;

        public DbSet<ProductSpecification> ProductSpecifications => Context.ProductSpecifications;

        public DbSet<ProductAttachment> ProductAttachments => Context.ProductAttachments;

        public DatabaseFacade Database => Context.Database;

        public ChangeTracker ChangeTracker => Context.ChangeTracker;

        public EntityEntry Entry(object entity)
        {
            return Context.Entry(entity);
        }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return Context.Entry(entity);
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class => Context.Set<TEntity>();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!UserExists || !Context.ChangeTracker.HasChanges())
            {
                return await Context.SaveChangesAsync();
            }

            if (!DeferredChanges.Any())
            {
                await InitializeInMemoryContextAsync();

                return await ApplyDbContextChangesToMemoryAsync();
            }

            return await SaveChangesAsync(MemoryContext.ChangeTracker);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    if (MemoryContext != null)
                    {
                        MemoryContext.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DeferredDbContext()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
