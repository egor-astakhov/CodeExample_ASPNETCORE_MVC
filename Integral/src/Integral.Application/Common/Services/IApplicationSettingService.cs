using Integral.Application.Common.Persistence.Entities;
using System.Threading.Tasks;

namespace Integral.Application.Common.Services
{
    public interface IApplicationSettingService
    {
        Task<T> GetAsync<T>();

        Task<ApplicationSetting> SetAsync<T>(T dto);
    }
}
