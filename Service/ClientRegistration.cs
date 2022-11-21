using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Vtb.Trade.Grpc.Common
{
    public static class ClientRegistration
    { 
        static void __Helper_SerializeMessage(IMessage message, SerializationContext context)
        {
            if (message is IBufferMessage)
            {
                context.SetPayloadLength(message.CalculateSize());
                MessageExtensions.WriteTo(message, context.GetBufferWriter());
                context.Complete();
                return;
            }
            context.Complete(MessageExtensions.ToByteArray(message));
        }

        static class __Helper_MessageCache<T>
        {
            public static readonly bool IsBufferMessage = IntrospectionExtensions.GetTypeInfo(typeof(IBufferMessage)).IsAssignableFrom(typeof(T));
        }

        static T __Helper_DeserializeMessage<T>(DeserializationContext context, MessageParser<T> parser) where T : IMessage<T>
        {
            if (__Helper_MessageCache<T>.IsBufferMessage)
            {
                return parser.ParseFrom(context.PayloadAsReadOnlySequence());
            }
            return parser.ParseFrom(context.PayloadAsNewBuffer());
        }

        static readonly Marshaller<CE> __Marshaller_main_Data = Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, CE.Parser));
        static readonly Marshaller<Empty> __Marshaller_google_protobuf_Empty = Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, Empty.Parser));

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

        public static ServiceDescriptor Descriptor { get => null; }
    }
}