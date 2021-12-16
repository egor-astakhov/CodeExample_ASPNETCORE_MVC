using System;

namespace Integral.Application.Common.Services
{
    public interface IDateTimeService
    {
        DateTime Now { get; }

        DateTime Today { get; }
    }
}
