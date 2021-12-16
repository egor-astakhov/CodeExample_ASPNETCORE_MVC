using Integral.Application.Common.Services;
using System;

namespace Integral.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;
    }
}
