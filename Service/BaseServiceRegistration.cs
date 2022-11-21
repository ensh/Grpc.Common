using System;
using System.Dynamic;
using Grpc.Core;
using Google.Protobuf.Reflection;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class TServiceNameProvider
    { 
        public abstract string ServiceName { get; }
    }

    public static class BaseServiceRegistration<TNameProvider> where TNameProvider : TServiceNameProvider, new()
    {
        public static string ServiceName => new TNameProvider().ServiceName;

        public static ServerServiceDefinition BindService(BaseGrpcService serviceImpl)
            => ServiceName.BindService(serviceImpl);

        public static void BindService(ServiceBinderBase serviceBinder, BaseGrpcService serviceImpl)
            => ServiceName.BindService(serviceBinder, serviceImpl);

        public static ServiceDescriptor Descriptor { get => null; }
    }

    public static class BaseServiceRegistration
    {
        public static readonly object Locker;

        public static string ServiceName { get; set; }

        public static ServerServiceDefinition BindService(BaseGrpcService serviceImpl)
            => ServiceName.BindService(serviceImpl);

        public static void BindService(ServiceBinderBase serviceBinder, BaseGrpcService serviceImpl)
            => ServiceName.BindService(serviceBinder, serviceImpl);

        public static ServiceDescriptor Descriptor { get => null; }

        static BaseServiceRegistration() => Locker = new object();
    }
}
