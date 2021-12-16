using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Products.Commands;
using Integral.Application.Products.Data;
using Integral.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Integral.Application.UnitTests.Products.Commands
{
    public class EditProductCommandHandlerTests
    {
        private Mock<IDateTimeService> _dateTimeServiceMock => new Mock<IDateTimeService>();
        private Mock<ICurrentUserService> _currentUserServiceMock => new Mock<ICurrentUserService>();

        public EditProductCommandHandlerTests()
        {
            
        }

        [Fact]
        public async Task NewProductShouldBeAddedCorrectly()
        {
            var fileMock = new Mock<IFormFile>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<AssetType.Value>(), It.IsAny<IFormFile>()))
                    .ReturnsAsync("FakeImagePath");

            var request = new EditProductViewModel
            {
                Name = "Name1",
                ShortDescription = "ShortDescription1",
                ImageName = "ImageName1.jpg",
                ImageFile = fileMock.Object,
                Description = "Description1",
                ApplicationArea = "ApplicationArea1",
                Features = "Features1",
                Specifications = new List<EditProductViewModel.Specification>
                {
                    new EditProductViewModel.Specification
                    {
                        Name = "Name1",
                        Value = "Value1"
                    }
                },
                Attachments = new List<EditProductViewModel.Attachment>
                {
                    new EditProductViewModel.Attachment
                    {
                        Name = "Name1",
                        FileVersion = "FileVersion1",
                        FileDate = new DateTime(2000, 10, 25),
                        FileName = "FileName1",
                        File = fileMock.Object,
                    }
                },
                SortingOrder = 100
            };

            var dbContext = GetDbContext();

            await new EditProductCommandHandler(dbContext, fileServiceMock.Object)
                .Handle(request, CancellationToken.None);

            var product = await dbContext.Products.FirstOrDefaultAsync();
            
            product.ShouldNotBeNull();

            product.Name.ShouldBe(request.Name);
            product.ShortDescription.ShouldBe(request.ShortDescription);
            product.Description.ShouldBe(request.Description);
            product.ApplicationArea.ShouldBe(request.ApplicationArea);
            product.Features.ShouldBe(request.Features);
            product.ImageName.ShouldBe(request.ImageName);
            product.ImagePath.ShouldNotBeNullOrWhiteSpace();
            product.SortingOrder.ShouldBe(100);

            for (var i = 0; i < product.Specifications.Count; i++)
            {
                var actual = product.Specifications.ElementAt(i);
                var expected = request.Specifications.ElementAt(i);

                actual.Name.ShouldBe(expected.Name);
                actual.Value.ShouldBe(expected.Value);
            }

            for (var i = 0; i < product.Attachments.Count; i++)
            {
                var actual = product.Attachments.ElementAt(i);
                var expected = request.Attachments.ElementAt(i);

                actual.Name.ShouldBe(expected.Name);
                actual.FileVersion.ShouldBe(expected.FileVersion);
                actual.FileDate.ShouldBe(expected.FileDate.Value);
                actual.FileName.ShouldBe(expected.FileName);
                actual.FilePath.ShouldNotBeNullOrWhiteSpace();
            }

            fileServiceMock.Verify(m => 
                m.CreateAsync(AssetType.Value.ProductImage, It.IsAny<IFormFile>()), Times.Once);
            fileServiceMock.Verify(m => 
                m.CreateAsync(AssetType.Value.ProductAttachment, It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task ProductImageShouldNotBeUpdatedIfNoFileWasSelected()
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<AssetType.Value>(), It.IsAny<IFormFile>()))
                    .ReturnsAsync("FakeImagePath");

            const string expectedImageName = "OriginalName";
            const string expectedImagePath = "OriginalPath";

            var dbContext = GetDbContext();
            dbContext.Products.Add(new Product
            {
                ImageName = expectedImageName,
                ImagePath = expectedImagePath
            });
            await dbContext.SaveChangesAsync();

            var request = new EditProductViewModel
            {
                Id = 1
            };
            
            await new EditProductCommandHandler(dbContext, fileServiceMock.Object)
                .Handle(request, CancellationToken.None);

            var product = await dbContext.Products.FirstOrDefaultAsync();
            
            product.ShouldNotBeNull();
            product.ImageName.ShouldBe(expectedImageName);
            product.ImagePath.ShouldBe(expectedImagePath);

            fileServiceMock.Verify(m => m.CreateAsync(AssetType.Value.ProductImage, It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task ProductSpecificationsShouldBeReconciledCorrectly()
        {
            var fileServiceMock = new Mock<IFileService>();

            var existingProduct = new Product
            {
                ImagePath = "test.jpg"
            };
            existingProduct.Specifications.Add(new ProductSpecification { Name = "Name1", Value = "Value1" });
            existingProduct.Specifications.Add(new ProductSpecification { Name = "Name2", Value = "Value2" });
            existingProduct.Specifications.Add(new ProductSpecification { Name = "Name3", Value = "Value3" });

            var dbContext = GetDbContext();
            dbContext.Products.Add(existingProduct);
            await dbContext.SaveChangesAsync();

            var request = new EditProductViewModel
            {
                Id = 1,
                Specifications = new List<EditProductViewModel.Specification>
                {
                    new EditProductViewModel.Specification { Id = 2, Name = "Name2", Value = "Value2" },
                    new EditProductViewModel.Specification { Id = 3, Name = "Name33", Value = "Value33" },
                    new EditProductViewModel.Specification { Name = "Name4", Value = "Value4" },
                }
            };

            await new EditProductCommandHandler(dbContext, fileServiceMock.Object)
                .Handle(request, CancellationToken.None);

            var product = await dbContext.Products.FirstOrDefaultAsync();
            product.ShouldNotBeNull();

            for (var i = 0; i < product.Specifications.Count; i++)
            {
                var actual = product.Specifications.ElementAt(i);
                var expected = request.Specifications.ElementAt(i);

                actual.Name.ShouldBe(expected.Name);
                actual.Value.ShouldBe(expected.Value);
            }
        }

        [Fact]
        public async Task ProductAttachmentsShouldBeReconciledCorrectly()
        {
            var fileMock = new Mock<IFormFile>();
            var fileServiceMock = new Mock<IFileService>();

            const string newImagePath = "new/Image/Path";
            fileServiceMock.Setup(m =>
                m.CreateAsync(It.IsAny<AssetType.Value>(), It.IsAny<IFormFile>()))
                    .ReturnsAsync(newImagePath);

            var existingProduct = new Product 
            { 
                ImagePath = "test.jpg"
            };
            existingProduct.Attachments.Add(new ProductAttachment 
            {
                Name = "Name1",
                FileVersion = "FileVersion1",
                FileDate = new DateTime(1, 1, 1),
                FileName = "FileName1",
                FilePath = "FilePath1",
            });
            existingProduct.Attachments.Add(new ProductAttachment
            {
                Name = "Name2",
                FileVersion = "FileVersion2",
                FileDate = new DateTime(2, 2, 2),
                FileName = "FileName2",
                FilePath = "FilePath2",
            });

            var dbContext = GetDbContext();
            dbContext.Products.Add(existingProduct);
            await dbContext.SaveChangesAsync();

            var request = new EditProductViewModel
            {
                Id = 1,
                Attachments = new List<EditProductViewModel.Attachment>
                {
                    new EditProductViewModel.Attachment
                    {
                        Id = 2,
                        Name = "Name22",
                        FileVersion = "FileVersion22",
                        FileDate = new DateTime(2020, 2, 20),
                        FileName = "FileName2"
                    },
                    new EditProductViewModel.Attachment
                    {
                        Name = "Name3",
                        FileVersion = "FileVersion3",
                        FileDate = new DateTime(3, 3, 3),
                        FileName = "FileName3",
                        File = fileMock.Object,
                    },
                }
            };

            await new EditProductCommandHandler(dbContext, fileServiceMock.Object)
                .Handle(request, CancellationToken.None);

            var product = await dbContext.Products.FirstOrDefaultAsync();
            product.ShouldNotBeNull();

            for (var i = 0; i < product.Attachments.Count; i++)
            {
                var actual = product.Attachments.ElementAt(i);
                var expected = request.Attachments.ElementAt(i);

                actual.Name.ShouldBe(expected.Name);
                actual.FileVersion.ShouldBe(expected.FileVersion);
                actual.FileDate.ShouldBe(expected.FileDate.Value);
                actual.FileName.ShouldBe(expected.FileName);

                switch (actual.Id)
                {
                    case 2:
                        actual.FilePath.ShouldNotBe(newImagePath);
                        break;
                    case 3:
                        actual.FilePath.ShouldBe(newImagePath);
                        break;
                }
            }

            fileServiceMock.Verify(m =>
                m.CreateAsync(AssetType.Value.ProductAttachment, It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task ProductCannotBeSavedWithoutImage()
        {
            Should.Throw<ApplicationException>(async () => {
                await new EditProductCommandHandler(GetDbContext(), new Mock<IFileService>().Object)
                    .Handle(new EditProductViewModel(), CancellationToken.None);
            });

            await Task.CompletedTask;
        }

        private IDeferredDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;

            var dbContext = new ApplicationDbContext(options, _currentUserServiceMock.Object, _dateTimeServiceMock.Object);

            return new DeferredDbContext(dbContext, _currentUserServiceMock.Object, _dateTimeServiceMock.Object);
        }
    }
}
