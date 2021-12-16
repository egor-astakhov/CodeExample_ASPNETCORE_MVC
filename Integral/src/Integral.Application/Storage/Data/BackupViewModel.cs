using Integral.Application.Common.Mappings;
using Integral.Application.Common.Storage;
using System;
using System.Collections.Generic;

namespace Integral.Application.Storage.Data
{
    public class BackupViewModel
    {
        public IEnumerable<Item> Items { get; set; } = new List<Item>();

        public class Item : IMapFrom<ApplicationFileInfo>
        {
            public string Name { get; set; }

            public DateTime CreationTime { get; set; }
        }
    }
}
