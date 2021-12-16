using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integral.Application.Common.Persistence
{
    public class DeferredChange
    {
        public DeferredChange(IEnumerable<EntityEntry> entries)
        {
            if (!entries.Any())
            {
                throw new ApplicationException("Entries cannot be epmty.");
            }

            Entries = entries.Select(entry => new DeferredChangeEntry(entry)).ToList();
        }

        public IEnumerable<DeferredChangeEntry> Entries { get; }
    }
}
