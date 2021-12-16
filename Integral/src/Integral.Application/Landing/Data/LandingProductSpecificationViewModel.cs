using Integral.Application.Common.Mappings;
using Integral.Application.Common.Persistence.Entities;

namespace Integral.Application.Landing.Data
{
    public class LandingProductSpecificationViewModel : IMapFrom<ProductSpecification>
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
