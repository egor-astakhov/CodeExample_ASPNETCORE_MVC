using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Integral.Application.Common.Storage;
using Integral.Application.Products.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Products.Commands
{
    public class EditProductCommandHandler : IRequestHandler<EditProductViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IFileService _fileService;

        public EditProductCommandHandler(
            IDeferredDbContext dbContext,
            IFileService fileService
            )
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public async Task<Unit> Handle(EditProductViewModel request, CancellationToken cancellationToken)
        {
            var product = await FindOrCreateAsync(request);

            product.Name = request.Name;
            product.ShortDescription = request.ShortDescription;
            product.Description = request.Description;
            product.ApplicationArea = request.ApplicationArea;
            product.Features = request.Features;
            product.SortingOrder = request.SortingOrder;

            if (request.ImageFile == null)
            {
                if (product.ImagePath == null) throw new ApplicationException("Product cannot be saved without image.");
            }
            else
            {
                product.ImageName = request.ImageName;
                product.ImagePath = await _fileService.CreateAsync(AssetType.Value.ProductImage, request.ImageFile);
            }

            ReconcileProductSpecifications(product, request.Specifications);
            await ReconcileProductAttachmentsAsync(product, request.Attachments);

            await _dbContext.SaveChangesAsync();

            return Unit.Value;
        }

        private async Task<Product> FindOrCreateAsync(EditProductViewModel request)
        {
            if (request.Id > 0)
            {
                return await _dbContext.Products
                    .Include(p => p.Specifications)
                    .Include(p => p.Attachments)
                    .FirstOrDefaultAsync(p => p.Id == request.Id) 
                    ?? throw new ApplicationException($"Product with Id: {request.Id} was not found.");
            }

            var productEntry = await _dbContext.Products.AddAsync(new Product());

            return productEntry.Entity;
        }

        private void ReconcileProductSpecifications(Product product, List<EditProductViewModel.Specification> specifications)
        {
            var toDelete = product.Specifications.ToList();

            foreach (var specification in specifications)
            {
                if (specification.Id > 0)
                {
                    var existing = toDelete.FirstOrDefault(m => m.Id == specification.Id)
                        ?? throw new ApplicationException($"Specification with Id: {specification.Id} was not found.");

                    existing.Name = specification.Name;
                    existing.Value = specification.Value;

                    toDelete.Remove(existing);
                }
                else
                {
                    _dbContext.ProductSpecifications.Add(new ProductSpecification
                    {
                        Product = product,
                        Name = specification.Name,
                        Value = specification.Value
                    });
                }
            }

            _dbContext.ProductSpecifications.RemoveRange(toDelete);
        }

        private async Task ReconcileProductAttachmentsAsync(Product product, List<EditProductViewModel.Attachment> attachments)
        {
            var toDelete = product.Attachments.ToList();

            foreach (var attachment in attachments)
            {
                if (attachment.Id > 0)
                {
                    var existing = toDelete.FirstOrDefault(m => m.Id == attachment.Id)
                        ?? throw new ApplicationException($"Attachment with Id: {attachment.Id} was not found.");

                    existing.Name = attachment.Name;
                    existing.FileVersion = attachment.FileVersion;
                    existing.FileDate = attachment.FileDate.Value;

                    if (attachment.File != null)
                    {
                        existing.FileName = attachment.FileName;
                        existing.FilePath = await _fileService.CreateAsync(AssetType.Value.ProductAttachment, attachment.File);
                    }

                    toDelete.Remove(existing);
                }
                else
                {
                    var filePath = await _fileService.CreateAsync(AssetType.Value.ProductAttachment, attachment.File);

                    _dbContext.ProductAttachments.Add(new ProductAttachment
                    {
                        Product = product,
                        Name = attachment.Name,
                        FileVersion = attachment.FileVersion,
                        FileDate = attachment.FileDate.Value,
                        FileName = attachment.FileName,
                        FilePath = filePath,
                    });
                }
            }

            _dbContext.ProductAttachments.RemoveRange(toDelete);
        }
    }
}
