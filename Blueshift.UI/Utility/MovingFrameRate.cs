using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blueshift.UI.Utility
{
    public class MovingFrameRate
    {
        private readonly int _capacity;
        private readonly Queue<TimeSpan> _timestamps;
        private readonly Stopwatch _stopwatch;
        private bool _isCached;
        private TimeSpan _lastTimestamp;
        private double _frameRate;
        private TimeSpan _frameTime;

        public MovingFrameRate(int capacity)
        {
            _capacity = capacity;
            _timestamps = new Queue<TimeSpan>(capacity);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            // Having atleast one timestamp allows us not to have to check whether we can peek/dequeue.
            _timestamps.Enqueue(_stopwatch.Elapsed);
        }

        public long Frame { get; private set; }

        public double FrameRate
        {
            get
            {
                EnsureCached();
                return _frameRate;
            }
        }

        public TimeSpan FrameTime
        {
            get
            {
                EnsureCached();
                return _frameTime;
            }
        }

        public void Update()
        {
            Frame++;
            TimeSpan timestamp = _stopwatch.Elapsed;
            AddTimestamp(timestamp);
            _lastTimestamp = timestamp;
            _isCached = false;
        }

        private void AddTimestamp(TimeSpan timestamp)
        {
            if (_timestamps.Count >= _capacity)
            {
                _timestamps.Dequeue();
            }

            _timestamps.Enqueue(timestamp);
        }

        private void EnsureCached()
        {
            if (!_isCached)
            {
                _isCached = true;
                Recalculate();
            }
        }

        private void Recalculate()
        {
            TimeSpan elapsed = _lastTimestamp - _timestamps.Peek();

            _frameTime = new TimeSpan(elapsed.Ticks / _timestamps.Count);
            _frameRate = elapsed.TotalSeconds <= 0.00001 ? 0 : _timestamps.Count / elapsed.TotalSeconds;
        }
    }
}
