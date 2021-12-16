using AutoMapper;
using Integral.Application.Common.Mappings;
using Integral.Application.Common.Persistence.Entities;
using System;
using System.Linq;

namespace Integral.Application.Landing.Data
{
    public class LandingProductAttachmentViewModel : IMapFrom<ProductAttachment>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FileVersion { get; set; }

        public DateTime FileDate { get; set; }

        public string FilePath { get; set; }

        [IgnoreMap]
        public string FileExtension => FilePath.Split('.').LastOrDefault();
    }
}
