using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Vtb.Trade.Grpc.Common
{
    public class CallbackEnumerator<T> : IEnumerator<T>, IEnumerator
    {
        public IEnumerable<T> Items
        {
            get 
            {
                while (MoveNext())
                {
                    yield return Current;
                } 
            }
        }

        private readonly object m_waitLock;
        private readonly ConcurrentQueue<T> m_currentQueue;

        public CallbackEnumerator()
        {
            m_waitLock = new object();
            m_terminated = false;
            m_currentQueue = new ConcurrentQueue<T>();
        }

        public void NewValue(T value)
        {
            lock (m_waitLock)
            {
                if (!m_terminated)
                {
                    m_currentQueue.Enqueue(value);
                    Monitor.Pulse(m_waitLock);
                }
            }
        }
        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            lock (m_waitLock)
            {
                m_currentQueue.Clear();
                m_terminated = true;
                Monitor.Pulse(m_waitLock);
            }
        }

        private volatile bool m_terminated;
        public bool MoveNext()
        {
            if (m_currentQueue.TryDequeue(out var current))
            {
                Current = current;
                return true;
            }
            else
            {
                while (!m_terminated)
                {
                    lock (m_waitLock)
                    {
                        if (Monitor.Wait(m_waitLock, 1000))
                        {
                            if (m_currentQueue.TryDequeue(out current))
                            {
                                Current = current;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
