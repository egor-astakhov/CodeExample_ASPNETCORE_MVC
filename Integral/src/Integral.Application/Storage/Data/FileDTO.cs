using System.IO;

namespace Integral.Application.Storage.Data
{
    public class FileDTO
    {
        public string Name { get; set; }

        public Stream Stream { get; set; }
    }
}
