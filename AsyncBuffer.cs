using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Vtb.Trade.Grpc.Common
{
    public class AsyncBuffer<T> : IDisposable
    {
        private SemaphoreSlim m_semaphore = new SemaphoreSlim(0, 1);
        private int Size { get; init; }
        private int Delay { get; init; }
        private Stopwatch m_start = new Stopwatch();
        private volatile int m_count = 0;
        private QueueList<T> m_buffer = new QueueList<T>();

        public AsyncBuffer()
        {
            m_start.Reset();
        }

        ~AsyncBuffer()
        {
            m_buffer = null;
            if (m_semaphore != null)
            {
                m_semaphore.Dispose();
                m_semaphore = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            m_buffer = null;
            m_semaphore.Dispose();
            m_semaphore = null;

            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Append(T value)
        {
            if (m_buffer != null)
            {
                m_buffer += value;
                if (Interlocked.Increment(ref m_count) == 1)
                    m_start = Stopwatch.StartNew();
                if (m_semaphore.CurrentCount == 0)
                    m_semaphore.Release();
            }
        }

        public async Task<IEnumerable<T>> GetValues()
        {
            while (true)
            {
                var timeout = (int)Math.Max(0, (Delay - m_start.ElapsedMilliseconds));
                if (timeout > 0 && await m_semaphore.WaitAsync(timeout))
                {
                    if (Interlocked.Add(ref m_count, 0) > Size)
                        return Values;
                }
                else
                {
                    if (Interlocked.Add(ref m_count, 0) > 0)
                        return Values;
                }
            }
        }

        public async Task ProcessValues(Action<IEnumerable<T>> onProcess)
        {
            while (true)
            {
                var timeout = (int)Math.Max(0, (Delay - m_start.ElapsedMilliseconds));
                if (await m_semaphore.WaitAsync(timeout))
                {
                    if (Interlocked.Add(ref m_count, 0) > Size)
                        onProcess?.Invoke(Values);
                }
                else
                {
                    if (Interlocked.Add(ref m_count, 0) > 0)
                        onProcess?.Invoke(Values);
                }
            }
        }

        private IEnumerable<T> Values
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                var buffer = m_buffer;

                if (buffer == null)
                    return Enumerable.Empty<T>();

                m_count = 0;
                m_start.Reset();
                m_buffer = new QueueList<T>();

                return buffer.Values;
            }
        }
    }
}
