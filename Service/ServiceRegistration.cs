using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Vtb.Trade.Grpc.Common
{
    public static class ServiceRegistration
    {
        public static readonly Marshaller<CE> __Marshaller_main_Data = Marshallers.Create((arg) => MessageExtensions.ToByteArray(arg), CE.Parser.ParseFrom);
        public static readonly Marshaller<Empty> __Marshaller_google_protobuf_Empty = Marshallers.Create((arg) => MessageExtensions.ToByteArray(arg), Empty.Parser.ParseFrom);

        public static Method<CE, Empty> Command(this string serviceName) 
            => new Method<CE, Empty>(
                MethodType.Unary, serviceName, "Command", __Marshaller_main_Data, __Marshaller_google_protobuf_Empty);

        public static Method<CE, CE> Query(this string serviceName) 
            => new Method<CE, CE>(
                MethodType.Unary, serviceName, "Query", __Marshaller_main_Data, __Marshaller_main_Data);

        public static Method<CE, Empty> Outcome(this string serviceName) 
            => new Method<CE, Empty>(
                MethodType.ClientStreaming, serviceName, "Outcome", __Marshaller_main_Data, __Marshaller_google_protobuf_Empty);

        public static Method<CE, CE> OutcomeEx(this string serviceName) 
            => new Method<CE, CE>(
                MethodType.ClientStreaming, serviceName, "OutcomeEx", __Marshaller_main_Data, __Marshaller_main_Data);

        public static Method<CE, CE> Duplex(this string serviceName) 
            => new Method<CE, CE>(
                MethodType.DuplexStreaming, serviceName, "Duplex", __Marshaller_main_Data, __Marshaller_main_Data);

        public static Method<Empty, CE> Income(this string serviceName)
            => new Method<Empty, CE>(
                MethodType.ServerStreaming, serviceName, "Income", __Marshaller_google_protobuf_Empty, __Marshaller_main_Data);

        public static Method<CE, CE> IncomeEx(this string serviceName) 
            => new Method<CE, CE>(
                MethodType.ServerStreaming, serviceName, "IncomeEx", __Marshaller_main_Data, __Marshaller_main_Data);

        public static ServerServiceDefinition BindService(this string serviceName, IGrpcService serviceImpl)
        {
            return ServerServiceDefinition.CreateBuilder()
                .AddMethod(Command(serviceName), serviceImpl.Command)
                .AddMethod(Query(serviceName), serviceImpl.Query)
                .AddMethod(Outcome(serviceName), serviceImpl.Outcome)
                .AddMethod(OutcomeEx(serviceName), serviceImpl.OutcomeEx)
                .AddMethod(Duplex(serviceName), serviceImpl.Duplex)
                .AddMethod(Income(serviceName), serviceImpl.Income)
                .AddMethod(IncomeEx(serviceName), serviceImpl.IncomeEx).Build();
        }

        public static void BindService(this string serviceName, ServiceBinderBase serviceBinder, IGrpcService serviceImpl)
        {
            serviceBinder.AddMethod(Command(serviceName), serviceImpl == null ? null : new UnaryServerMethod<CE, Empty>(serviceImpl.Command));
            serviceBinder.AddMethod(Query(serviceName), serviceImpl == null ? null : new UnaryServerMethod<CE, CE>(serviceImpl.Query));
            serviceBinder.AddMethod(Outcome(serviceName), serviceImpl == null ? null : new ClientStreamingServerMethod<CE, Empty>(serviceImpl.Outcome));
            serviceBinder.AddMethod(OutcomeEx(serviceName), serviceImpl == null ? null : new ClientStreamingServerMethod<CE, CE>(serviceImpl.OutcomeEx));
            serviceBinder.AddMethod(Duplex(serviceName), serviceImpl == null ? null : new DuplexStreamingServerMethod<CE, CE>(serviceImpl.Duplex));
            serviceBinder.AddMethod(Income(serviceName), serviceImpl == null ? null : new ServerStreamingServerMethod<Empty, CE>(serviceImpl.Income));
            serviceBinder.AddMethod(IncomeEx(serviceName), serviceImpl == null ? null : new ServerStreamingServerMethod<CE, CE>(serviceImpl.IncomeEx));
        }

        public static ServiceDescriptor Descriptor
        {
            get => null;
        }
    }
}
