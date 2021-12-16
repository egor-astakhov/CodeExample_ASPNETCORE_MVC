using AutoMapper;
using Integral.Application.Common.Mappings;
using Integral.Application.Common.Persistence.Entities;
using System.Collections.Generic;

namespace Integral.Application.Landing.Data
{
    public class LandingProductViewModel : IMapFrom<Product>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string ImageName { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> ApplicationAreaList { get; set; } = new List<string>();

        public IEnumerable<string> FeaturesList { get; set; } = new List<string>();

        public List<LandingProductSpecificationViewModel> Specifications { get; set; } 
            = new List<LandingProductSpecificationViewModel>();

        public List<LandingProductAttachmentViewModel> Attachments { get; set; }
            = new List<LandingProductAttachmentViewModel>();

        [IgnoreMap]
        public string UIID => $"product-{Id}";

        [IgnoreMap]
        public string UIDescriptionId => $"product-description-{Id}";

        [IgnoreMap]
        public string UIApplicationAreaId => $"product-application-area-{Id}";

        [IgnoreMap]
        public string UIFeaturesId => $"product-features-{Id}";

        [IgnoreMap]
        public string UISpecificationsId => $"product-specifications-{Id}";

        [IgnoreMap]
        public string UIAttachmentsId => $"product-attachments-{Id}";

        [IgnoreMap]
        public string MobileUIID => $"m-product-{Id}";

        [IgnoreMap]
        public string MobileCarouselId => $"m-product-carousel-{Id}";
    }
}
