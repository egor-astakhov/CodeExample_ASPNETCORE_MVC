using AutoMapper;
using AutoMapper.QueryableExtensions;
using Integral.Application.Common.Configuration;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IMapper _mapper;

        public FileService(AppSettings appSettings, IMapper mapper)
        {
            AppSettings = appSettings;
            _mapper = mapper;
        }

        public AppSettings AppSettings { get; }

        public void CreateDefaultDirectories()
        {
            Directory.CreateDirectory(GetAbsolutePath(FileType.Value.Backup));

            foreach (var assetType in Enum.GetValues(typeof(AssetType.Value)).Cast<AssetType.Value>())
            {
                CreateDirectory(assetType);
            }
        }

        private void CreateDirectory(AssetType.Value assetType)
        {
            Directory.CreateDirectory(GetDirectoryPath(assetType));
        }

        public IEnumerable<ApplicationFileInfo> GetFiles(AssetType.Value assetType)
        {
            return GetFiles(GetDirectoryPath(assetType));
        }

        public IEnumerable<ApplicationFileInfo> GetFiles(FileType.Value fileType)
        {
            return GetFiles(GetAbsolutePath(fileType));
        }

        private string GetDirectoryPath(AssetType.Value assetType)
        {
            return GetAbsolutePath(FileType.Value.Asset, AssetType.GetFolder(assetType));
        }

        private IEnumerable<ApplicationFileInfo> GetFiles(string directoryPath)
        {
            return new DirectoryInfo(directoryPath)
                .EnumerateFiles()
                .AsQueryable()
                .ProjectTo<ApplicationFileInfo>(_mapper.ConfigurationProvider);
        }

        public Stream GetFileStream(AssetType.Value assetType, string relativePath)
        {
            return GetFileStream(FileType.Value.Asset, GetRelativePath(assetType, relativePath));
        }

        public Stream GetFileStream(FileType.Value fileType, string relativePath)
        {
            return GetFileStream(fileType, relativePath, FileAccess.Read);
        }

        public Stream GetFileStream(FileType.Value fileType, string relativePath, FileAccess fileAccess)
        {
            return GetFileStream(fileType, relativePath, fileAccess, FileShare.Read);
        }

        public Stream GetFileStream(FileType.Value fileType, string relativePath, FileAccess fileAccess, FileShare fileShare)
        {
            return File.Open(GetAbsolutePath(fileType, relativePath), FileMode.Open, fileAccess, fileShare);
        }

        public async Task<string> CreateAsync(AssetType.Value assetType, IFormFile file)
        {
            file = file ?? throw new ArgumentNullException(nameof(file));

            var relativePath = GetRelativePath(assetType, CreateFileName(file));

            using var stream = File.Create(GetAbsolutePath(FileType.Value.Asset, relativePath));

            await file.CopyToAsync(stream);

            return relativePath;
        }

        private string CreateFileName(IFormFile file)
        {
            return $"{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
        }

        public void Delete(AssetType.Value assetType, string fileName)
        {
            Delete(FileType.Value.Asset, GetRelativePath(assetType, fileName));
        }

        public void Delete(FileType.Value fileType, string relativePath)
        {
            File.Delete(GetAbsolutePath(fileType, relativePath));
        }

        private string GetRelativePath(AssetType.Value assetType, string fileName)
        {
            return Path.Combine(AssetType.GetFolder(assetType), fileName);
        }

        public string CreateAssetsBackup()
        {
            var backupName = $"backup_{DateTime.Now:yyyyMMddHHmmssfffff}.zip";

            ZipFile.CreateFromDirectory(
                GetAbsolutePath(FileType.Value.Asset),
                GetAbsolutePath(FileType.Value.Backup, backupName)
            );

            return backupName;
        }

        public void ApplyAssetsBackup(ZipArchive archive)
        {
            archive.ExtractToDirectory(GetAbsolutePath(FileType.Value.Asset), overwriteFiles: true);
        }

        private string GetAbsolutePath(FileType.Value fileType, string relativePath)
        {
            return $"{GetAbsolutePath(fileType)}{relativePath}";
        }

        private string GetAbsolutePath(FileType.Value fileType)
        {
            return fileType switch
            {
                FileType.Value.Asset => AppSettings.Paths.ExternalWebRoot,
                FileType.Value.Backup => AppSettings.Paths.BackupRoot,
                _ => throw new NotSupportedException($"FileType {fileType} is not supported.")
            };
        }
    }
}
