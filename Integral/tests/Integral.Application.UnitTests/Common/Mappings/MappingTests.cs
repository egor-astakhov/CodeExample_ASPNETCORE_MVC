using AutoMapper;
using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence.Entities;
using Integral.Application.Common.Storage;
using Integral.Application.Products.Data;
using System;
using System.IO;
using Xunit;

namespace Integral.Application.UnitTests.Common.Mappings
{
    public class MappingTests : IClassFixture<MappingTestsFixture>
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests(MappingTestsFixture fixture)
        {
            _configuration = fixture.ConfigurationProvider;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(EmailServiceSettingsDTO), typeof(EmailServiceSettingsViewModel))]
        [InlineData(typeof(EmailServiceSettingsViewModel), typeof(EmailServiceSettingsDTO))]
        [InlineData(typeof(CommonSettingsDTO), typeof(CommonSettingsViewModel))]
        [InlineData(typeof(CommonSettingsViewModel), typeof(CommonSettingsDTO))]
        [InlineData(typeof(LandingCarouselSettingsDTO), typeof(LandingCarouselSettingsViewModel))]
        [InlineData(typeof(Product), typeof(EditProductViewModel))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source);

            _mapper.Map(instance, source, destination);
        }
    }
}
