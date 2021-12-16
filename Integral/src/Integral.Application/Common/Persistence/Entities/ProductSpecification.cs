namespace Integral.Application.Common.Persistence.Entities
{
    public class ProductSpecification : AuditableEntity
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Product Product { get; set; }
    }
}
