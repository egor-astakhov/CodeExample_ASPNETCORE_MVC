using Integral.Application.Common.Mappings;
using MediatR;

namespace Integral.Application.ApplicationSettings.Data
{
    public class CommonSettingsViewModel : IRequest, IMapFrom<CommonSettingsDTO>
    {
        public string Address { get; set; }

        public string MapSource { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
