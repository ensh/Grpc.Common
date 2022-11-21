using System.Threading.Tasks;

using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Vtb.Trade.Grpc.Common;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class BaseGrpcService : IGrpcService
    {
        public virtual Task<Empty> Command(CE request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task<CE> Query(CE request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task<Empty> Outcome(IAsyncStreamReader<CE> requestStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task<CE> OutcomeEx(IAsyncStreamReader<CE> requestStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task Duplex(IAsyncStreamReader<CE> requestStream, IServerStreamWriter<CE> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task Income(Empty request, IServerStreamWriter<CE> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public virtual Task IncomeEx(CE request, IServerStreamWriter<CE> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }
    }
}
