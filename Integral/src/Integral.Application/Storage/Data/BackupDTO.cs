using Integral.Application.Common.Persistence.Entities;
using System;
using System.Collections.Generic;

namespace Integral.Application.Storage.Data
{
    public class BackupDTO
    {
        public const string FILE_NAME = "db.json";
        public const int BACKUP_VERSION = 1;

        public int Version { get; set; }

        public DateTime CreationTime { get; set; }

        public List<ApplicationSetting> ApplicationSettings { get; } = new List<ApplicationSetting>();

        public List<Product> Products { get; } = new List<Product>();

        public List<ProductSpecification> ProductSpecifications { get; } = new List<ProductSpecification>();

        public List<ProductAttachment> ProductAttachments { get; } = new List<ProductAttachment>();
    }
}
