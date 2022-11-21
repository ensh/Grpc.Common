using Grpc.Net.Client;

using System;

namespace Vtb.Trade.Grpc.Common
{
    public interface IProcessingBase { };
    public abstract class CommonClientBase<IProcessing> where IProcessing : IProcessingBase
    {
        public abstract string Name { get; }
        public abstract bool Start(Func<GrpcChannel> getChannel, Action<IProcessing> setProcessing, int packetSize);
    }

    public abstract class ManagerBase<IProcessing> where IProcessing : IProcessingBase
    {
        public abstract void SetProcessing(IProcessing processing);
    }

    public class CommonClientConnector<TClient, TManager, IProc>
        where IProc : IProcessingBase
        where TClient : CommonClientBase<IProc>
        where TManager : ManagerBase<IProc>, new()
    {
        public CommonClientConnector(TClient client, Func<GrpcChannel> getChannel,
            Action<string> writeLog, int packetSize)
        {
            m_writeLog = writeLog;
            m_getChannel = getChannel;
            m_packetSize = packetSize;
            onGet = () =>
            {
                var manager = new TManager();

                Action<IProc> setProcessing = manager.SetProcessing;
                setProcessing += processing => Reconnect(client, manager, processing);

                m_writeLog($"Start {client.Name} connection");
                while (!client.Start(m_getChannel, setProcessing, m_packetSize))
                {
                    m_writeLog($"{client.Name} connection timeout");
                }

                m_writeLog($"{client.Name} connected");
                return manager;
            };
        }

        private void Reconnect(TClient client, TManager manager, IProc currentProcessing)
        {
            if (currentProcessing == null)
            {
                m_writeLog($"{client.Name} connection lost");

                Action<IProc> setConversion = manager.SetProcessing;
                setConversion += transactions => Reconnect(client, manager, transactions);

                m_writeLog($"Start {client.Name} reconnection");
                while (!client.Start(m_getChannel, setConversion, m_packetSize))
                {
                    m_writeLog($"{client.Name} connection timeout");
                }

                m_writeLog($"{client.Name} connection esteblished");
            }
        }

        private int m_packetSize;
        private Func<TManager> onGet;

        private Func<GrpcChannel> m_getChannel;
        private Action<string> m_writeLog;

        // ленивая инициализация
        public static implicit operator TManager(CommonClientConnector<TClient, TManager, IProc> connector)
            => connector.onGet();
    }
}