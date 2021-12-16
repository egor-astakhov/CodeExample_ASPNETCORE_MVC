using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integral.Application.Common.Persistence
{
    public static class DbContextExtensions
    {
        public static async Task CopyToAsync(this IApplicationDbContext source, dynamic target)
        {
            static async Task<IEnumerable<T>> GetAsync<T>(IApplicationDbContext source) where T : class
                => await source.Set<T>().AsNoTracking().ToListAsync();

            target.ApplicationSettings.AddRange(await GetAsync<ApplicationSetting>(source));
            target.Products.AddRange(await GetAsync<Product>(source));
            target.ProductSpecifications.AddRange(await GetAsync<ProductSpecification>(source));
            target.ProductAttachments.AddRange(await GetAsync<ProductAttachment>(source));
        }
    }
}
