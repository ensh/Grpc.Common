using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Vtb.Trade.Grpc.Common
{
    public class ServiceClient : ClientBase<ServiceClient>
    {
        public ServiceClient(ChannelBase channel) : base(channel)
        {
        }
        public ServiceClient(CallInvoker callInvoker) : base(callInvoker)
        {
        }
        protected ServiceClient() : base()
        {
        }
        protected ServiceClient(ClientBaseConfiguration configuration) : base(configuration)
        {
        }

        public virtual string ServiceName { get => ""; }

        public virtual Empty Command(CE request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Command(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual Empty Command(CE request, CallOptions options)
        {
            return CallInvoker.BlockingUnaryCall(ClientRegistration.Command(ServiceName), null, options, request);
        }
        public virtual AsyncUnaryCall<Empty> CommandAsync(CE request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CommandAsync(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncUnaryCall<Empty> CommandAsync(CE request, CallOptions options)
        {
            return CallInvoker.AsyncUnaryCall(ClientRegistration.Command(ServiceName), null, options, request);
        }
        public virtual CE Query(CE request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Query(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual CE Query(CE request, CallOptions options)
        {
            return CallInvoker.BlockingUnaryCall(ClientRegistration.Query(ServiceName), null, options, request);
        }
        public virtual AsyncUnaryCall<CE> QueryAsync(CE request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return QueryAsync(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncUnaryCall<CE> QueryAsync(CE request, CallOptions options)
        {
            return CallInvoker.AsyncUnaryCall(ClientRegistration.Query(ServiceName), null, options, request);
        }
        public virtual AsyncClientStreamingCall<CE, Empty> Outcome(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Outcome(new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncClientStreamingCall<CE, Empty> Outcome(CallOptions options)
        {
            return CallInvoker.AsyncClientStreamingCall(ClientRegistration.Outcome(ServiceName), null, options);
        }
        public virtual AsyncClientStreamingCall<CE, CE> OutcomeEx(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OutcomeEx(new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncClientStreamingCall<CE, CE> OutcomeEx(CallOptions options)
        {
            return CallInvoker.AsyncClientStreamingCall(ClientRegistration.OutcomeEx(ServiceName), null, options);
        }
        public virtual AsyncDuplexStreamingCall<CE, CE> Duplex(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Duplex(new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncDuplexStreamingCall<CE, CE> Duplex(CallOptions options)
        {
            return CallInvoker.AsyncDuplexStreamingCall(ClientRegistration.Duplex(ServiceName), null, options);
        }
        public virtual AsyncServerStreamingCall<CE> Income(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Income(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncServerStreamingCall<CE> Income(Empty request, CallOptions options)
        {
            return CallInvoker.AsyncServerStreamingCall(ClientRegistration.Income(ServiceName), null, options, request);
        }
        public virtual AsyncServerStreamingCall<CE> IncomeEx(CE request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return IncomeEx(request, new CallOptions(headers, deadline, cancellationToken));
        }
        public virtual AsyncServerStreamingCall<CE> IncomeEx(CE request, CallOptions options)
        {
            return CallInvoker.AsyncServerStreamingCall(ClientRegistration.IncomeEx(ServiceName), null, options, request);
        }
        
        protected override ServiceClient NewInstance(ClientBaseConfiguration configuration)
        {
            return new ServiceClient(configuration);
        }
    }
}
