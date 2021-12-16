using System.Threading.Tasks;

namespace Integral.Application.Common.Services
{
    public interface IEmailServiceFactory
    {
        Task<IEmailService> GetInstanceAsync();
    }
}
