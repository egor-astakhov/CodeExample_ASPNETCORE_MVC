using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Services
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        private static readonly IDictionary<Type, string> _keyMap = new Dictionary<Type, string>
        {
            { typeof(CommonSettingsDTO), ApplicationSetting.COMMON_SETTINGS_KEY },
            { typeof(EmailServiceSettingsDTO), ApplicationSetting.EMAIL_SERVICE_SETTINGS_KEY },
            { typeof(LandingCarouselSettingsDTO), ApplicationSetting.LANDING_CAROUSEL_SETTINGS_KEY },
        };

        private readonly IApplicationDbContext _dbContext;

        public ApplicationSettingService(IDeferredDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetAsync<T>()
        {
            var setting = await _dbContext.ApplicationSettings.FindAsync(GetKey<T>());

            if (string.IsNullOrEmpty(setting?.Value))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(setting.Value);
        }

        public async Task<ApplicationSetting> SetAsync<T>(T dto)
        {
            var setting = await FindOrAddAsync<T>();

            setting.Value = JsonConvert.SerializeObject(dto);

            return setting;
        }

        private async Task<ApplicationSetting> FindOrAddAsync<T>()
        {
            var key = GetKey<T>();

            var setting = await _dbContext.ApplicationSettings.FindAsync(key);

            if (setting == null)
            {
                setting = new ApplicationSetting
                {
                    Key = key
                };

                await _dbContext.ApplicationSettings.AddAsync(setting);
            }

            return setting;
        }

        private string GetKey<T>()
        {
            var type = typeof(T);

            if (!_keyMap.ContainsKey(type))
            {
                throw new NotSupportedException($"{type.FullName} doesn't have associated application setting.");
            }

            return _keyMap[type];
        }
    }
}
