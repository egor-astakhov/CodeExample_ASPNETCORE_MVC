using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Services;
using Integral.Infrastructure.Persistence;
using Integral.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Integral.Infrastructure.UnitTests.Services
{
    public class ApplicationSettingServiceTests
    {
        private Mock<IDateTimeService> _dateTimeServiceMock => new Mock<IDateTimeService>();
        private Mock<ICurrentUserService> _currentUserServiceMock => new Mock<ICurrentUserService>();

        [Fact]
        public void Should_ThrowException_When_SettingIsNotSupported()
        {
            Should.Throw<NotSupportedException>(async () => {
                await GetService().GetAsync<object>();
            });
        }

        [Fact]
        public async Task Should_ReturnDefault_When_SettingDoesNotExistInDB()
        { 
            var setting = await GetService().GetAsync<CommonSettingsDTO>();

            setting.ShouldBe(default);
        }

        [Fact]
        public async Task Should_ReturnDeserializedValue_When_Get()
        {
            var expected = new CommonSettingsDTO();

            var dbContext = GetDbContext();
            dbContext.ApplicationSettings.Add(new ApplicationSetting
            {
                Key = ApplicationSetting.COMMON_SETTINGS_KEY,
                Value = JsonConvert.SerializeObject(expected)
            });

            var actual = await GetService(dbContext).GetAsync<CommonSettingsDTO>();

            actual.ShouldBeOfType(expected.GetType());
        }

        [Fact]
        public async Task Should_SerializeValue_When_Set()
        {
            var setting = await GetService().SetAsync(new CommonSettingsDTO());

            JsonConvert.DeserializeObject<CommonSettingsDTO>(setting.Value).ShouldBeOfType<CommonSettingsDTO>();
        }

        [Fact]
        public async Task Should_AddNewSetting_When_SettingDoesNotExistInDB()
        {
            var dbContext = GetDbContext();

            dbContext.ApplicationSettings.ShouldBeEmpty();

            var setting = await GetService(dbContext).SetAsync(new CommonSettingsDTO());
            await dbContext.SaveChangesAsync();

            dbContext.ApplicationSettings.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Should_UpdateExisting_When_SettingsExistsInDB()
        {
            var expected = new ApplicationSetting
            {
                Key = ApplicationSetting.COMMON_SETTINGS_KEY
            };

            var dbContext = GetDbContext();
            dbContext.ApplicationSettings.Add(expected);

            var actual = await GetService(dbContext).SetAsync(new CommonSettingsDTO());

            actual.ShouldBe(expected);
            actual.Value.ShouldNotBeNullOrEmpty();
        }

        private IApplicationSettingService GetService()
        {
            return new ApplicationSettingService(GetDbContext());
        }

        private IApplicationSettingService GetService(IDeferredDbContext dbContext)
        {
            return new ApplicationSettingService(dbContext);
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
