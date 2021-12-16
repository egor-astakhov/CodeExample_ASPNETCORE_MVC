using System;

namespace Integral.Application.Common.Persistence.Entities
{
    public class ProductAttachment : AuditableEntity
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public string FileVersion { get; set; }

        public DateTime FileDate { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Product Product { get; set; }
    }
}
