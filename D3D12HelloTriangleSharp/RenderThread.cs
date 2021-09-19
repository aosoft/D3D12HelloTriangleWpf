using System;
using System.Diagnostics;
using System.Threading;

namespace D3D12HelloTriangleSharp
{
    public sealed class RenderThread : IDisposable
    {
        private static readonly TimeSpan _ticksPerFrame = new TimeSpan(TimeSpan.TicksPerSecond / 60);
        private readonly Thread _thread;
        private readonly Action _callback;
        private bool _loop = true;

        public RenderThread(Action callback)
        {
            _callback = callback;
            _thread = new Thread(ThreadMain);
            _thread.Start();
        }
        
        public void Dispose()
        {
            _loop = false;
            _thread.Join();
        }

        private void ThreadMain()
        {
            var sw = new Stopwatch();
            while (_loop)
            {
                sw.Restart();
                _callback();
                var t = new TimeSpan(_ticksPerFrame.Ticks - sw.ElapsedTicks);
                if (t > TimeSpan.Zero)
                {
                    Thread.Sleep(t);
                }
            }
        }
    }
}