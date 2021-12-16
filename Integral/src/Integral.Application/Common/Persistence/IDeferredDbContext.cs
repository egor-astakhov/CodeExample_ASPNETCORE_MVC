using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integral.Application.Common.Persistence
{
    public interface IDeferredDbContext : IApplicationDbContext
    {
        bool ChangesExist { get; }

        Task PersistChangesAsync();

        void DiscardChanges();

        ICollection<DeferredChange> DeferredChanges { get; }
    }
}
