using Integral.Application.Common.Storage;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Integral.Application.Common.Services
{
    public interface IFileService
    {
        void CreateDefaultDirectories();

        IEnumerable<ApplicationFileInfo> GetFiles(AssetType.Value assetType);

        IEnumerable<ApplicationFileInfo> GetFiles(FileType.Value fileType);

        Stream GetFileStream(AssetType.Value assetType, string relativePath);

        Stream GetFileStream(FileType.Value fileType, string relativePath);

        Stream GetFileStream(FileType.Value fileType, string relativePath, FileAccess fileAccess);

        Stream GetFileStream(FileType.Value fileType, string relativePath, FileAccess fileAccess, FileShare fileShare);

        Task<string> CreateAsync(AssetType.Value assetType, IFormFile file);

        void Delete(AssetType.Value assetType, string fileName);

        void Delete(FileType.Value fileType, string relativePath);

        string CreateAssetsBackup();

        void ApplyAssetsBackup(ZipArchive archive);
    }
}
