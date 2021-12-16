using System;
using System.Threading.Tasks;

namespace Integral.Application.Common.Services
{
    public interface IEmailService : IDisposable
    {
        Task SendToSupportAsync(string subject, string message);
    }
}
