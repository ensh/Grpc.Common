namespace Vtb.Trade.Grpc.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Reflection;

    public class CE_MultiAdapter : CE_Adapter
    {
        public const int iRepeat = 0;

        public CE_MultiAdapter(int entityType) : base(entityType) { }
        public CE_MultiAdapter(CE data) : base(data) { }

        public void Register<T>(int entityType) where T : CE_Adapter
            => Register(entityType, typeof(T));

        public void Register(int entityType, Type type)
        {
            if (!m_adapters.ContainsKey(entityType))
            {
                m_adapters = m_adapters.Add(entityType, new AdapterCreator(type));
            }
        }

        public void Register(IEnumerable<(int entityType, Type type)> regInfos)
        {
            foreach (var regInfo in regInfos)
            {
                Register(regInfo.entityType, regInfo.type);
            }
        }

        public CE_Adapter CreateAdapter(int entityType)
            => m_adapters.TryGetValue(entityType, out var creator) ? creator.ForSend(entityType) : default;

        public int Count 
        {
            get => this[iRepeat]?.AsRepeat?.Items.Count ?? default;
            set
            {
                if (Repeat.Items.Capacity < value + 1)
                    Repeat.Items.Capacity = value + 1;
            }
        }

        public IEnumerable<CE_Adapter> Adapters
        {
            get 
            {
                var repeat = this[iRepeat]?.AsRepeat ?? new CE.Repeat();
                foreach (var item in repeat.Items.Where(i => i.ItemType == CE.ValueType.AsEntity))
                {
                    if (m_adapters.TryGetValue(item.AsEntity.EntityType, out var creator))
                    {
                        yield return creator.ToReceive(item.AsEntity);
                    }
                }
            }

            set => Repeat.Items.Add(value.Select(a => CE.RepeatItem.Create(a)));
        }

        public void Add(CE_Adapter value)
        {
            Repeat.Items.Add(CE.RepeatItem.Create(value));
        }

        private CE.Repeat Repeat
        {
            get 
            {
                var repeat = this[iRepeat]?.AsRepeat ?? default;
                if (repeat == default)
                {
                    this[iRepeat] = CE.FieldValue.Create(iRepeat+1, repeat = new CE.Repeat());
                }
                return repeat;
            }
        }

        private ImmutableDictionary<int, AdapterCreator> m_adapters = ImmutableDictionary<int, AdapterCreator>.Empty;

        private struct AdapterCreator
        {
            public readonly Func<int, CE_Adapter> ForSend;
            public readonly Func<CE, CE_Adapter> ToReceive;

            public AdapterCreator(Type adapterType)
                : this(GetSendCreator(adapterType), GetReceiveCreator(adapterType))
            { 
            }

            public AdapterCreator(Func<int, CE_Adapter> forSend, Func<CE, CE_Adapter> toReceive)
            {
                ForSend = forSend;
                ToReceive = toReceive;
            }

            private static Func<int, CE_Adapter> GetSendCreator(Type adapterType)
            {
                var entityType = Expression.Parameter(typeof(int), "et");
                var constructor = adapterType.GetConstructor(new[] { typeof(int) });

                if (constructor is ConstructorInfo)
                {
                    var getter = Expression.Lambda<Func<int, CE_Adapter>>(
                        Expression.Convert(Expression.New(constructor, entityType), typeof(CE_Adapter)), entityType);

                    return getter.Compile();
                }

                constructor = adapterType.GetConstructor(new Type[] { });

                if (constructor is ConstructorInfo)
                {
                    var getter = Expression.Lambda<Func<int, CE_Adapter>>(
                        Expression.Convert(Expression.New(constructor), typeof(CE_Adapter)), entityType);

                    return getter.Compile();
                }

                throw new Exception($"отсутствие конструктора {adapterType}");
            }

            private static Func<CE, CE_Adapter> GetReceiveCreator(Type adapterType)
            {
                var entity = Expression.Parameter(typeof(CE), "e");
                var constructor = adapterType.GetConstructor(new[] { typeof(CE) });

                if (constructor is ConstructorInfo)
                {
                    var getter = Expression.Lambda<Func<CE, CE_Adapter>>(
                    Expression.Convert(Expression.New(constructor, entity), typeof(CE_Adapter)), entity);

                    return getter.Compile();
                }

                throw new Exception($"отсутствие конструктора {adapterType}");
            }

            public static implicit operator AdapterCreator((Func<int, CE_Adapter> forSend, Func<CE, CE_Adapter> toReceive) creator)
                => new AdapterCreator(creator.forSend, creator.toReceive);
        }
    }
}
