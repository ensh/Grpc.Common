using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class CommonClient<IProc> : CommonClientBase<IProc> where IProc : IProcessingBase
    {
        protected readonly ILogger<CommonClient<IProc>> m_logger;
        public int ConnectionTimeout { get; set; } = 30_000;

        public CommonClient(ILogger<CommonClient<IProc>> logger)
        {
            m_logger = logger;
        }

        public override bool Start(Func<GrpcChannel> createChannel, Action<IProc> onConnect, int packetSize)
        {
            using var connected = new ManualResetEvent(false);
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await InternalStart(createChannel, onConnect, () => connected.Set(), packetSize); ;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        m_logger?.LogError(ex, $"{Name}.InternalStart\r\n");
                    }
                }, TaskCreationOptions.LongRunning);

                return connected.WaitOne(ConnectionTimeout);
            }
            catch (Exception exception)
            {
                m_logger?.LogError(exception, $"{Name}.Start\r\n");
                return false;
            }
            finally
            {
                connected.Dispose();
            }
        }

        private async Task InternalStart(Func<GrpcChannel> createChannel, Action<IProc> onConnect,
            Action onConnected, int packetSize)
        {
            using var channel = createChannel();

            m_logger?.LogInformation($"Startig {Name} at {channel.Target}");

            await channel.WaitForStateChangedAsync(ConnectivityState.Connecting);

            if (channel.State != ConnectivityState.TransientFailure ||
                channel.State != ConnectivityState.Shutdown)
            {
                onConnect(InitProcessing(channel, packetSize));
                onConnected();

                m_logger?.LogInformation($"Started {Name} at {channel.Target}");

                try
                {
                    while (channel.State != ConnectivityState.TransientFailure ||
                        channel.State != ConnectivityState.Shutdown)
                    {
                        await channel.WaitForStateChangedAsync(channel.State);
                    }
                    m_logger?.LogInformation($"Finished {Name} at {channel.Target}, state is {channel.State}.");
                }
                catch (OperationCanceledException) { }
                catch (Exception exception)
                {
                    m_logger.LogError(exception, $"{Name}.InternalStart");
                }
            }

            onConnect(default);
        }

        protected abstract IProc InitProcessing(GrpcChannel channel, int packetSize);
    }
}
