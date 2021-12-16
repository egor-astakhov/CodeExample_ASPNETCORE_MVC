using Integral.Application.Common.Mappings;
using System;
using System.IO;

namespace Integral.Application.Common.Storage
{
    public class ApplicationFileInfo : IMapFrom<FileInfo>
    {
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
