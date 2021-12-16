using Integral.Application.Common.Mappings;
using Integral.Application.Common.Persistence.Entities;
using System.Collections.Generic;

namespace Integral.Application.Products.Data
{
    public class ProductListViewModel
    {
        public List<Item> Items { get; set; } = new List<Item>();

        public class Item : IMapFrom<Product>
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string ShortDescription { get; set; }

            public int SortingOrder { get; set; }
        }
    }
}
