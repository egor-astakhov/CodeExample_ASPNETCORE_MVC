using System;

namespace Integral.Application.Common.Storage
{
    public static class AssetType
    {
        public static string GetFolder(Value value)
        {
            return value switch
            {
                Value.LandingCarouselItem => "landing-carousel/",
                Value.ProductImage => "product-images/",
                Value.ProductAttachment => "product-attachments/",
                _ => throw new NotSupportedException($"AssetType {value} is not supported."),
            };
        }

        public enum Value
        {
            LandingCarouselItem,
            ProductImage,
            ProductAttachment
        }
    }
}
