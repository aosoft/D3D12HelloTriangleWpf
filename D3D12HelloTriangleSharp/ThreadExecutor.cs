using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace D3D12HelloTriangleSharp
{
    public sealed class ThreadExecutor : IDisposable
    {
        private readonly Channel<Action> _channel;
        private readonly Task _taskLoop;

        public ThreadExecutor()
        {
            _channel = Channel.CreateUnbounded<Action>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = false
            });

            _taskLoop = Task.Run(MainLoop);
        }

        public void BeginInvoke(Action fn) => _channel.Writer.TryWrite(fn);
        
        private async Task MainLoop()
        {
            var r = _channel.Reader;
            while (await r.WaitToReadAsync().ConfigureAwait(false))
            {
                while (r.TryRead(out var fn))
                {
                    fn();
                }
            }
        }
        
        public void Dispose()
        {
            _channel.Writer.Complete();
            _taskLoop.Wait();
        }
    }
}