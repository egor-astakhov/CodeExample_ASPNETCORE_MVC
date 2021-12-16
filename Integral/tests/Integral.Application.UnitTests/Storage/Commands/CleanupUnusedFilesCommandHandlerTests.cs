using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Storage.Commands;
using Integral.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Integral.Application.UnitTests.Storage.Commands
{
    public class CleanupUnusedFilesCommandHandlerTests
    {
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;

        private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;

        public CleanupUnusedFilesCommandHandlerTests()
        {
            _dateTimeServiceMock = new Mock<IDateTimeService>();
            _dateTimeServiceMock.Setup(m => m.Today).Returns(new DateTime(2000, 1, 1));

            _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
            _applicationSettingServiceMock.Setup(m => m.GetAsync<LandingCarouselSettingsDTO>())
                .ReturnsAsync(new LandingCarouselSettingsDTO());
        }

        private Mock<ICurrentUserService> CurrentUserServiceMock => new Mock<ICurrentUserService>();

        [Fact]
        public async Task UnusedProductImagesShouldBeRemoved()
        {
            var dbContext = GetDbContext();
            dbContext.Products.AddRange(new List<Product> { 
                new Product { ImagePath = "images/1.file" },
            });
            await dbContext.SaveChangesAsync();

            var fileServiceMock = GetFileServiceMock(AssetType.Value.ProductImage);

            await new CleanupUnusedFilesCommand
                .CleanupUnusedFilesCommandHandler(dbContext, _applicationSettingServiceMock.Object, 
                    fileServiceMock.Object, _dateTimeServiceMock.Object)
                .Handle(new CleanupUnusedFilesCommand(), default);

            fileServiceMock.Verify(m => m.Delete(It.IsAny<AssetType.Value>(), It.IsAny<string>()), Times.Once);
            fileServiceMock.Verify(m => m.Delete(AssetType.Value.ProductImage, "3.file"), Times.Once);
        }

        [Fact]
        public async Task UnusedProductAttachmentsShouldBeRemoved()
        {
            var dbContext = GetDbContext();
            dbContext.ProductAttachments.AddRange(new List<ProductAttachment> {
                new ProductAttachment { FilePath = "images/1.file" },
            });
            await dbContext.SaveChangesAsync();

            var fileServiceMock = GetFileServiceMock(AssetType.Value.ProductAttachment);

            await new CleanupUnusedFilesCommand
                .CleanupUnusedFilesCommandHandler(dbContext, _applicationSettingServiceMock.Object,
                    fileServiceMock.Object, _dateTimeServiceMock.Object)
                .Handle(new CleanupUnusedFilesCommand(), default);

            fileServiceMock.Verify(m => m.Delete(It.IsAny<AssetType.Value>(), It.IsAny<string>()), Times.Once);
            fileServiceMock.Verify(m => m.Delete(AssetType.Value.ProductAttachment, "3.file"), Times.Once);
        }

        [Fact]
        public async Task UnusedLandingCarouselItemsShouldBeRemoved()
        {
            var dto = new LandingCarouselSettingsDTO();
            dto.Items.Add(new LandingCarouselSettingsDTO.Item
            {
                Path = "images/1.file"
            });

            var applicationSettingsServiceMock = new Mock<IApplicationSettingService>();
            applicationSettingsServiceMock.Setup(m => m.GetAsync<LandingCarouselSettingsDTO>()).ReturnsAsync(dto);

            var fileServiceMock = GetFileServiceMock(AssetType.Value.LandingCarouselItem);

            await new CleanupUnusedFilesCommand
                .CleanupUnusedFilesCommandHandler(GetDbContext(), applicationSettingsServiceMock.Object,
                    fileServiceMock.Object, _dateTimeServiceMock.Object)
                .Handle(new CleanupUnusedFilesCommand(), default);

            fileServiceMock.Verify(m => m.Delete(It.IsAny<AssetType.Value>(), It.IsAny<string>()), Times.Once);
            fileServiceMock.Verify(m => m.Delete(AssetType.Value.LandingCarouselItem, "3.file"), Times.Once);
        }

        private IDeferredDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;

            var dbContext = new ApplicationDbContext(options, CurrentUserServiceMock.Object, _dateTimeServiceMock.Object);

            return new DeferredDbContext(dbContext, CurrentUserServiceMock.Object, _dateTimeServiceMock.Object);
        }

        private Mock<IFileService> GetFileServiceMock(AssetType.Value fileType)
        {
            var fileServiceMock = new Mock<IFileService>();

            var files = new List<ApplicationFileInfo> {
                new ApplicationFileInfo { Name = "1.file", CreationTime = new DateTime(2000, 1, 1) }, //used
                //new ApplicationFileInfo { Name = "2.file", CreationTime = new DateTime(2000, 1, 1) }, //"today file" cannot be deleted
                new ApplicationFileInfo { Name = "3.file", CreationTime = new DateTime(1999, 1, 1) }, //can be deleted
            };
            
            fileServiceMock.Setup(m => m.GetFiles(fileType)).Returns(files);

            return fileServiceMock;
        }
    }
}
