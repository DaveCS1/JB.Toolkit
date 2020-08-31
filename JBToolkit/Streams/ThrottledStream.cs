using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;

namespace JBToolkit.Streams
{
    /// <summary>
    /// Throttle a stream -> Very useful for throttle HttpWebRequest uploads / downloads.
    /// 
    /// Usage 1 - Without percentage complete and bytes per second output:
    /// 
    ///    var stream = request.GetRequestStream();
    ///    var throttledStream = new ThrottledStream(new MemoryStream(fileBytes), uploadSpeedInBytesPerSecond);
    ///    throttledStream.CopyTo(stream);
    ///    stream.Close();
    ///    
    /// Usage 2 - With percentage complete and bytes per second output:
    /// 
    ///    var stream = request.GetRequestStream();
    ///    var output = new ThrottledStream.ThrottleStreamOutput();
    ///    var throttledStream = new ThrottledStream(new MemoryStream(fileBytes), totalBytes, ref output, 1000000);
    ///    throttledStream.CopyTo(stream);
    ///    stream.Close();
    /// 
    /// For use with an event:
    /// 
    ///    var throttledStream = new ThrottledStream(new MemoryStream(fileBytes), totalBytes, 1000000);
    ///    throttledStream.OnThrottleStreamOutput += YourHandlerMethod;    
    /// </summary>
    public class ThrottledStream : Stream
    {
        public delegate void ThrottleStreamOutputHandler(object source, ThrottleStreamOutputEventArgs e);
        public event ThrottleStreamOutputHandler OnThrottleStreamOutput;

        /// <summary>
        /// Object which can be passed into the constructor which stores the current 
        /// 'bytes per second' and 'percentage complete'. Can also utilise the 'ThrottleStreamOutputEvent'
        /// </summary>
        public class ThrottleStreamOutput
        {
            public double BytesPerSecond { get; set; } = 0.0;
            public double PercentageComplete { get; set; } = 0.0;
        }

        public class ThrottleStreamOutputEventArgs : EventArgs
        {
            public double BytesPerSecond { get; private set; }

            public double PercentgeComplete { get; private set; }

            public ThrottleStreamOutputEventArgs(double bytesPerSecond, double percentageComplete)
            {
                BytesPerSecond = bytesPerSecond;
                PercentgeComplete = percentageComplete;
            }
        }

        #region Properties

        private int maxBytesPerSecond;
        /// <summary>
        /// Number of Bytes that are allowed per second
        /// </summary>
        public int MaxBytesPerSecond
        {
            get { return maxBytesPerSecond; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("MaxBytesPerSecond has to be > 0");

                maxBytesPerSecond = value;
            }
        }

        public ThrottleStreamOutput Output { get; set; } = null;

        #endregion


        #region Private Members

        private int processed;
        private int processedTotal = 0;
        System.Timers.Timer resettimer;
        AutoResetEvent wh = new AutoResetEvent(true);
        private Stream parent;
        private Stopwatch stopWatch = new Stopwatch();
        private int fileBytes;

        #endregion

        /// <summary>
        /// Creates a new Stream with Databandwith cap
        /// </summary>
        /// <param name="parentStream"></param>
        /// <param name="maxBytesPerSecond"></param>
        public ThrottledStream(
            Stream parentStream,
            ref ThrottleStreamOutput throttleStreamOutput,
            int totalFileBytes,
            int maxBytesPerSecond = int.MaxValue)
        {
            Output = throttleStreamOutput;
            fileBytes = totalFileBytes;

            MaxBytesPerSecond = maxBytesPerSecond;
            parent = parentStream;
            processed = 0;
            resettimer = new System.Timers.Timer
            {
                Interval = 1000
            };
            resettimer.Elapsed += Resettimer_Elapsed;
            stopWatch.Start();
            resettimer.Start();
        }

        /// <summary>
        /// Creates a new Stream with Databandwith cap
        /// </summary>
        /// <param name="parentStream"></param>
        /// <param name="maxBytesPerSecond"></param>
        public ThrottledStream(
            Stream parentStream,
            int totalFileBytes,
            int maxBytesPerSecond = int.MaxValue)
        {
            fileBytes = totalFileBytes;

            MaxBytesPerSecond = maxBytesPerSecond;
            parent = parentStream;
            processed = 0;
            resettimer = new System.Timers.Timer
            {
                Interval = 1000
            };
            resettimer.Elapsed += Resettimer_Elapsed;
            resettimer.Start();
        }

        protected void Throttle(int bytes)
        {
            try
            {
                processedTotal += bytes;
                SetOutput();
                processed += bytes;
                if (processed >= maxBytesPerSecond)
                    wh.WaitOne();
            }
            catch
            { }
        }

        private void Resettimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            processed = 0;
            wh.Set();
        }

        private void SetOutput()
        {
            double percentage = processedTotal / (double)fileBytes * 100;
            if (percentage > 100)
                percentage = 100;

            double bytesPerSecond = processedTotal / stopWatch.Elapsed.TotalSeconds;

            if (Output != null)
            {
                Output.BytesPerSecond = bytesPerSecond;
                Output.PercentageComplete = percentage;
            }

            OnThrottleStreamOutput?.Invoke(this, new ThrottleStreamOutputEventArgs(
                bytesPerSecond,
                percentage
            ));
        }

        #region Stream-Overrides

        public override void Close()
        {
            resettimer.Stop();
            resettimer.Close();
            base.Close();
        }
        protected override void Dispose(bool disposing)
        {
            resettimer.Dispose();
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get { return parent.CanRead; }
        }

        public override bool CanSeek
        {
            get { return parent.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return parent.CanWrite; }
        }

        public override void Flush()
        {
            parent.Flush();
        }

        public override long Length
        {
            get { return parent.Length; }
        }

        public override long Position
        {
            get
            {
                return parent.Position;
            }
            set
            {
                parent.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Throttle(count);
            return parent.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return parent.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            parent.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Throttle(count);
            parent.Write(buffer, offset, count);
        }

        #endregion
    }
}
