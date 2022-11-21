using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Vtb.Trade.Grpc.Common
{
    public static partial class CommonExecution
    {
        public static readonly CE SendBuffer = new();

        private static async Task WriteAsync(this IServerStreamWriter<CE> responseStream, CE entity, WriteOptions writeOptions)
        {
            responseStream.WriteOptions = writeOptions;
            await responseStream.WriteAsync(entity);
        }

        public static async Task IncomeEx(this IEnumerable<CE> items, IServerStreamWriter<CE> responseStream, 
            Action identityPost = null, int packetSize = 100, bool streamDefaultWriteOptions = false)
        {
            var writeOptions = streamDefaultWriteOptions ? WriteOptions.Default : new WriteOptions(0);

            await items.IncomeEx(e => responseStream.WriteAsync(e, writeOptions),
                identityPost, packetSize);
        }

        public static async Task IncomeEx(this IEnumerable<CE> items, Func<CE, Task> send,
            Action identityPost = null, int packetSize = 100)
        {
            int counter = 0;
            var result = 0.CreateParameters(packetSize);

            async Task Send(CE resultEntity)
            {
                identityPost?.Invoke();
                await send(resultEntity);
            }

            foreach (var item in items)
            {
                var current = result;
                // пустой объект для немедленной отправки!
                if (ReferenceEquals(item, SendBuffer))
                {
                    if (current.Count() > 0)
                    {
                        result = 0.CreateParameters(packetSize);
                        counter = 0;

                        await Send(current);
                    }
                }
                else
                {
                    if (++counter % packetSize == 0)
                    {
                        result = 0.CreateParameters(packetSize);
                        current.Append(item);

                        await Send(current);                        
                    }
                    else
                        current.Append(item);
                }
            }

            if (result.Count() > 0)
            {
                await Send(result);
            }
        }

        public static IEnumerable<CE> IncomePacket(this IEnumerable<CE_Adapter> items, int packetSize = 100)
            => items.Select(i => i.Entity).IncomePacket(packetSize);

        public static IEnumerable<CE> IncomePacket(this IEnumerable<CE> items, int packetSize = 100)
        {
            int counter = 0;
            var result = 0.CreateParameters(packetSize);

            foreach (var item in items)
            {
                if (++counter % packetSize == 0)
                {
                    result.Append(item);
                    yield return result;

                    result = 0.CreateParameters(packetSize);
                }
                else
                    result.Append(item);
            }

            if (result.Count() > 0)
            {
                yield return result;
            }
        }
    }
}
