using Integral.Application.Common.Mappings;

namespace Integral.Application.ApplicationSettings.Data
{
    public class CommonSettingsDTO : IMapFrom<CommonSettingsViewModel>
    {
        public string Address { get; set; }

        public string MapSource { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
