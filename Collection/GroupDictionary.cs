using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class GroupDictionary<TKey, TValue>
    {
        public GroupDictionary()
        {
            m_dictionary = new ConcurrentDictionary<TKey, GroupItem<TValue>>();
        }

        public GroupDictionary(int capacity)
        {
            m_dictionary = new ConcurrentDictionary<TKey, GroupItem<TValue>>(Environment.ProcessorCount, capacity);
        }

        public abstract TKey GetKey(TValue value);
        public abstract bool EqualValue(TValue x, TValue y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TValue value)
        {
            var key = GetKey(value);
            m_dictionary.AddOrUpdate(key, k => value, 
                (k, old) =>
                {
                    foreach (var oldItem in old.Items())
                    {
                        if (EqualValue(oldItem, value))
                        {
                            oldItem.Value = value;
                            return old;
                        }
                    }

                    return value.Prepand(old);
                });
        }

        public void Add(params TValue[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Add(values[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey key) => m_dictionary.ContainsKey(key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TValue value) => Contains(GetKey(value));

        public IEnumerable<TValue> this[TKey key]
        {
            get 
            {
                if (m_dictionary.TryGetValue(key, out var groupItem))
                {
                    foreach (var value in groupItem.Values())
                        yield return value;
                }
            }
        }

        public IEnumerable<TKey> Keys
        {
            get 
            {
                foreach (var item in m_dictionary)
                    yield return item.Key;
            }
        }

        public IEnumerable<TValue> Values
        {
            get 
            {
                foreach (var groupItem in m_dictionary) 
                {
                    foreach (var itemValue in groupItem.Value.Values())
                    {
                        yield return itemValue;
                    }
                }
            }
        }

        private ConcurrentDictionary<TKey, GroupItem<TValue>> m_dictionary;
    }

    internal class GroupItem<TValue>
    {
        public TValue Value;
        public GroupItem<TValue> Next;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TValue(GroupItem<TValue> item) => item.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GroupItem<TValue>(TValue value) => new GroupItem<TValue> { Value = value };
    }

    internal static class GroupItemUtils
    {
        internal static IEnumerable<TValue> Values<TValue>(this GroupItem<TValue> startValue)
        {
            while (startValue != null)
            {
                yield return startValue;
                startValue = startValue.Next;
            }
        }

        internal static IEnumerable<GroupItem<TValue>> Items<TValue>(this GroupItem<TValue> startValue)
        {
            while (startValue != null)
            {
                yield return startValue;
                startValue = startValue.Next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GroupItem<TValue> Prepand<TValue>(this TValue value, GroupItem<TValue> next)
            => new GroupItem<TValue> { Value = value, Next = next, };
    }
}
