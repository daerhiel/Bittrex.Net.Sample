using System;
using System.Runtime.Serialization;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the date/time series calculation utilities.
    /// </summary>
    [DataContract]
    public static class TimeCalculator
    {
        /// <summary>
        /// Gets the reference date/time value respective to N-minute span interval.
        /// </summary>
        /// <param name="time">Date/time value.</param>
        /// <param name="spanMinutes">N-minute interval span.</param>
        /// <returns>Reference date/time value.</returns>
        public static DateTime GetReference(DateTime time, int spanMinutes)
        {
            if ((TimeSpan.TicksPerHour / TimeSpan.TicksPerMinute) % spanMinutes == 0)
                return time.AddTicks(-time.Ticks % TimeSpan.FromMinutes(spanMinutes).Ticks);
            else
                throw new ArgumentException($"Minutes span {spanMinutes} is not supported, as it doesn't fit into an hour N times.", nameof(spanMinutes));
        }
    }
}