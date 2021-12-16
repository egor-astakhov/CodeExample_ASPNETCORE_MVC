using Integral.Application.Common.Mappings;
using Integral.Application.Common.Persistence.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace Integral.Application.Products.Data
{
    public class EditProductViewModel : IRequest, IMapFrom<Product>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        [IgnoreMap]
        public IFormFile ImageFile { get; set; }

        public string ImageName { get; set; }

        public string Description { get; set; }

        public string ApplicationArea { get; set; }

        public string Features { get; set; }

        public List<Specification> Specifications { get; set; } = new List<Specification>();

        public class Specification : IMapFrom<ProductSpecification>
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Value { get; set; }
        }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public class Attachment : IMapFrom<ProductAttachment>
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string FileVersion { get; set; }

            public DateTime? FileDate { get; set; }

            [IgnoreMap]
            public IFormFile File { get; set; }

            public string FileName { get; set; }
        }

        public int SortingOrder { get; set; }
    }
}
