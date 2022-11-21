using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Vtb.Trade.Grpc.Common
{
    public interface IGrpcService
    {
        Task<Empty> Command(CE request, ServerCallContext context);
        Task<CE> Query(CE request, ServerCallContext context);
        Task<Empty> Outcome(IAsyncStreamReader<CE> requestStream, ServerCallContext context);
        Task<CE> OutcomeEx(IAsyncStreamReader<CE> requestStream, ServerCallContext context);
        Task Duplex(IAsyncStreamReader<CE> requestStream, IServerStreamWriter<CE> responseStream, ServerCallContext context);
        Task Income(Empty request, IServerStreamWriter<CE> responseStream, ServerCallContext context);
        Task IncomeEx(CE request, IServerStreamWriter<CE> responseStream, ServerCallContext context);
    }
}
