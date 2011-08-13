using System;

namespace Clavis
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }

    public class TimeService : ITimeService
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}