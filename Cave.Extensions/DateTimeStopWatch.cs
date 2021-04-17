using System;
using System.Diagnostics;
using System.Threading;

namespace Cave
{
    /// <summary>
    /// Implements a basic timer with a low resolution based on <see cref="Environment.TickCount" /> (This class has an accuracy of about
    /// 15 msec on most platforms.)
    /// </summary>
    [DebuggerDisplay("{Elapsed}")]
    public sealed class DateTimeStopWatch : IStopWatch
    {
        #region Static

        /// <summary>Starts a new stop watch.</summary>
        /// <returns>The started stop watch.</returns>
        public static DateTimeStopWatch StartNew()
        {
            var result = new DateTimeStopWatch();
            result.Start();
            return result;
        }

        #endregion

        /// <summary>Gets the elapsed time if the timer is no longer running.</summary>
        TimeSpan elapsed;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="DateTimeStopWatch" /> class.</summary>
        public DateTimeStopWatch() => Reset();

        #endregion

        #region IStopWatch Members

        /// <summary>Gets the elapsed time.</summary>
        public TimeSpan Elapsed => IsRunning ? DateTime.UtcNow - StartDateTime : elapsed;

        /// <summary>Gets the elapsed time in milli seconds.</summary>
        public long ElapsedMilliSeconds => Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>Gets the elapsed time in seconds.</summary>
        public double ElapsedSeconds => Elapsed.Ticks / (double)TimeSpan.TicksPerSecond;

        /// <summary>Gets the frequency of the <see cref="IStopWatch" /> in HZ.</summary>
        public long Frequency => (long)(TimeSpan.TicksPerSecond / (double)Resolution.Ticks);

        /// <summary>Gets a value indicating whether the <see cref="IStopWatch" /> is running or not.</summary>
        public bool IsRunning { get; private set; }

        /// <summary>Resets the <see cref="IStopWatch" /> (can be used even if the <see cref="IStopWatch" /> is running).</summary>
        public void Reset()
        {
            StartDateTime = DateTime.UtcNow;
            elapsed = TimeSpan.Zero;
        }

        /// <summary>Gets the resolution of the <see cref="IStopWatch" /> in seconds.</summary>
        public TimeSpan Resolution => StopWatch.CheckResolution(StartNew());

        /// <summary>Starts the <see cref="IStopWatch" /> (to restart a <see cref="IStopWatch" /> use <see cref="Reset" /> first!).</summary>
        public void Start()
        {
            if (elapsed != TimeSpan.Zero)
            {
                throw new InvalidOperationException("Timer has to be reset first!");
            }

            if (IsRunning)
            {
                throw new InvalidOperationException("Timer already running!");
            }

            StartDateTime = DateTime.UtcNow;
            IsRunning = true;
        }

        /// <summary>Gets the <see cref="DateTime" /> (utc) value at start of the <see cref="IStopWatch" />.</summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>Stops the <see cref="IStopWatch" />.</summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Timer not running!");
            }

            elapsed = DateTime.UtcNow - StartDateTime;
            IsRunning = false;
        }

        /// <summary>Waits until the specified <see cref="Elapsed" /> time is reached.</summary>
        /// <param name="elapsed">The elapsed timespan.</param>
        /// <exception cref="System.Exception">StopWatch is not running!.</exception>
        public void Wait(TimeSpan elapsed)
        {
            while (true)
            {
                if (!IsRunning)
                {
                    throw new Exception("StopWatch is not running!");
                }

                var waitTime = elapsed - Elapsed;
                if (waitTime > TimeSpan.Zero)
                {
#if NETSTANDARD13
                    System.Threading.Tasks.Task.Delay(waitTime).Wait();
#else
                    Thread.Sleep(waitTime);
#endif
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>Checks for equality with another stopwatch.</summary>
        /// <param name="obj">The other stopwatch.</param>
        /// <returns>True if the stopwatches are equal.</returns>
        public override bool Equals(object obj) => obj is IStopWatch other && (other.StartDateTime == StartDateTime) && (other.IsRunning == IsRunning);

        /// <summary>Gets the hashcode for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => base.GetHashCode();

        #endregion

        #region Members

        /// <summary>Creates a copy of the <see cref="IStopWatch" />.</summary>
        /// <returns>Returns a copy of the <see cref="IStopWatch" />.</returns>
        public object Clone()
        {
            var result = new DateTimeStopWatch
            {
                StartDateTime = StartDateTime,
                elapsed = elapsed,
                IsRunning = IsRunning
            };
            return result;
        }

        #endregion
    }
}
