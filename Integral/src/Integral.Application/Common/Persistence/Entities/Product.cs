using System.Collections.Generic;
using System.Linq;

namespace Integral.Application.Common.Persistence.Entities
{
    public class Product : AuditableEntity
    {
        public Product()
        {

        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string ImageName { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }

        public string ApplicationArea { get; set; }

        public IEnumerable<string> ApplicationAreaList =>
            ApplicationArea.Split("\n")
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();

        public string Features { get; set; }

        public IEnumerable<string> FeaturesList =>
            Features.Split("\n")
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();

        public ICollection<ProductSpecification> Specifications { get; } = new List<ProductSpecification>();

        public ICollection<ProductAttachment> Attachments { get; } = new List<ProductAttachment>();

        public int SortingOrder { get; set; }
    }
}
