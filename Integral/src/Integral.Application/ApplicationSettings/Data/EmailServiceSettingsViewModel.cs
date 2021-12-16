using Integral.Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;

namespace Integral.Application.ApplicationSettings.Data
{
    public class EmailServiceSettingsViewModel : IRequest, IMapFrom<EmailServiceSettingsDTO>
    {
        public static IEnumerable<int> SupportedPorts { get; } = new int[] { 25, 587 };

        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SupportEmail { get; set; }
    }
}
