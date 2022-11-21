
using System;
using System.Linq;
using System.Threading;

namespace Vtb.Trade.Grpc.Common
{
    public class AsyncProcessor
    {
        private readonly Action<CE> Processing;
        public AsyncProcessor(Action<CE> processing, int timeout = 1000, int packetSize = 1000)
        {
            Processing = processing;
            m_timeout = timeout;
            m_packetSize = packetSize;
            m_buffer = NewBuffer;
            m_terminate = false;
            m_waitLock = new object();
        }

        public void Process(CE value)
        {
            var result = m_buffer;
            lock (m_waitLock)
            {
                Monitor.Pulse(m_waitLock);

                if (m_buffer.Count() < m_packetSize)
                    m_buffer.Append(value);
                else
                {
                    m_buffer = NewBuffer.Append(value);
                }
            }

            if (result.Count() >= m_packetSize)
            {
                Processing(result);
            }

        }

        public void Monitoring()
        {
            while (!m_terminate)
            {
                var result = default(CE);
                lock (m_waitLock)
                {
                    if (!Monitor.Wait(m_waitLock, m_timeout) && m_buffer.Count() > 0)
                    {
                        result = m_buffer;
                        m_buffer = NewBuffer;
                    }
                }

                if (result != null && result.Count() > 0)
                {
                    Processing(result);
                }
            }
        }
        public void Terminate()
        {
            lock (m_waitLock)
            {
                m_terminate = true;
                Monitor.Pulse(m_waitLock);
            }
            Thread.Sleep(m_timeout);
        }

        private CE NewBuffer => 0.CreateParameters(m_packetSize);
        private volatile bool m_terminate;
        private readonly int m_timeout;
        private readonly int m_packetSize;
        private readonly object m_waitLock = new object();
        private volatile CE m_buffer;

    }
}