using Google.Protobuf.WellKnownTypes;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class SortedGroupDictionary<TGroupKey, TSortKey, TValue, TInfo>
    {
        public SortedGroupDictionary()
        {
            m_dictionary = new ConcurrentDictionary<TGroupKey, SortedGroup<TInfo, TValue>>();
        }

        public SortedGroupDictionary(int capacity)
        {
            m_dictionary = new ConcurrentDictionary<TGroupKey, SortedGroup<TInfo, TValue>>(Environment.ProcessorCount, capacity);
        }

        public abstract TGroupKey GetGroupKey(TValue value);
        public abstract TSortKey GetSortKey(TValue value);
        public abstract IComparer<TValue> GetValueComparer();
        public abstract IComparer<TSortKey> GetSortKeyComparer();

        public Func<TInfo, TValue, TInfo> UpdateGroupInfo;
        public void AddOrReplace(TValue value)
        {
            var key = GetGroupKey(value);
            m_dictionary.AddOrUpdate(key,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new ReplaceAllocator<TValue>(value, default, default).Allocate(),
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new ReplaceAllocator<TValue>(value, old.Values, GetValueComparer()).Allocate(),
                });
        }

        public void Append(TValue prevValue, TValue value)
        {
            var key = GetGroupKey(value);
            m_dictionary.AddOrUpdate(key,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new ReplaceAllocator<TValue>(value, default, default).Allocate(),
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new LastAppendAllocator<TValue>(old.Values, GetValueComparer())
                    { PrevValue = prevValue, LastValue = value}.Allocate(),
                });
        }

        public void AddOrUpdate(TValue value, Func<TValue, TValue, TValue> update)
            => AddOrUpdate(GetGroupKey(value), value, update);
        
        public void AddOrUpdate(TGroupKey groupKey, TValue value, Func<TValue, TValue, TValue> update)
        {
            m_dictionary.AddOrUpdate(groupKey,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new ReplaceAllocator<TValue>(value, default, default).Allocate(),
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new AddOrUpdateAllocator<TValue>(old.Values, GetValueComparer())
                    {
                        Value = value,
                        Update = old => update(old, value)
                    }.Allocate(),
                });
        }

        public void AddOrUpdate(IEnumerable<TValue> values, Func<TValue, TValue, TValue> update)
        {
            foreach (var valuesGroup in values.GroupBy(GetGroupKey))
            {
                AddOrUpdate(valuesGroup.Key, valuesGroup.ToArray(), update);
            }
        }

        public void AddOrUpdate(TGroupKey groupKey, IEnumerable<TValue> values, Func<TValue, TValue, TValue> update)
        {
            m_dictionary.AddOrUpdate(groupKey,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new ValuesAllocator<TValue>(values, default, GetValueComparer())
                        .Allocate()
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new ValuesAllocator<TValue>(values, old.Values, GetValueComparer())
                    {
                        Update = update
                    }.Allocate()
                });
        }

        public void Update<TUpdate>(IEnumerable<TUpdate> updates, Func<TUpdate, TGroupKey> getGroupKey, 
            Func<TUpdate, TSortKey> getSortKey, Func<TUpdate, TValue> create, Func<TValue, TUpdate, TValue> update)
        {
            foreach (var updateGroup in updates.GroupBy(getGroupKey))
            {
                Update(updateGroup.Key, updateGroup.ToArray(), getSortKey, create, update);
            }
        }

        public void Update<TUpdate>(TGroupKey groupKey, IEnumerable<TUpdate> updates, 
            Func<TUpdate, TSortKey> getSortKey, Func<TUpdate, TValue> create, Func<TValue, TUpdate, TValue> update)
        {
            var searchComparer = SearchComparer(default);
            m_dictionary.AddOrUpdate(groupKey,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new UpdatesAllocator<TSortKey, TUpdate, TValue>(updates, default, _ => GetValueComparer())
                    {
                        GetKey = getSortKey,
                        Create = create, 
                        Update = update,
                        SortComparer = GetValueComparer()
                    }.Allocate()
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new UpdatesAllocator<TSortKey, TUpdate, TValue>(updates, old.Values,
                        key => searchComparer.GetComparer(key))
                    {
                        GetKey = getSortKey,
                        Create = create,
                        Update = update,
                        SortComparer = GetValueComparer()
                    }.Allocate()
                });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TGroupKey key) => m_dictionary.ContainsKey(key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TValue value) => Contains(GetGroupKey(value));

        public IEnumerable<TValue> this[TGroupKey key]
        {
            get => new Enumerable(m_dictionary.TryGetValue(key, out var groupItem) ?
                groupItem.Values : Array.Empty<TValue>());
        }

        public (bool isnull, TValue value) this[TGroupKey groupKey, TSortKey sortKey]
        {
            get
            {
                if (m_dictionary.TryGetValue(groupKey, out var groupItems))
                {
                    var i = Array.BinarySearch(groupItems.Values, default, SearchComparer(sortKey));
                    if (i >= 0)
                        return (false, groupItems.Values[i]);
                }

                return (true, default);
            }
        }

        public IEnumerable<TValue> ValuesFrom(TGroupKey groupKey, TSortKey sortKey)
        {
            if (m_dictionary.TryGetValue(groupKey, out var groupItems))
            {
                var i = Array.BinarySearch(groupItems.Values, default, SearchComparer(sortKey));

                if (i < 0) i = ~i;

                for (; i < groupItems.Values.Length; i++)
                {
                    yield return groupItems.Values[i];
                }
            }

            yield break;
        }

        public SortedArrayCursor<TValue> GetCursor(TGroupKey groupKey)
            => new SortedArrayCursor<TValue>(m_dictionary[groupKey].Values, GetValueComparer(),
                values => GroupInfoValuesUpdate(groupKey, values));

        private void GroupInfoValuesUpdate(TGroupKey groupKey, TValue[] values)
        {
            var groupInfo = m_dictionary[groupKey];
            groupInfo.Values = values;
            m_dictionary[groupKey] = groupInfo;
        }            

        public void Remove(TGroupKey groupKey, TSortKey removeKey)
        {
            var searchComparer = SearchComparer(removeKey);
            m_dictionary.AddOrUpdate(groupKey,
                k => new SortedGroup<TInfo, TValue>
                {
                    Values = new TValue[0]
                },
                (k, old) => new SortedGroup<TInfo, TValue>
                {
                    GroupInfo = old.GroupInfo,
                    Values = new RemoveAllocator<TValue>(old.Values, searchComparer).Allocate(),
                });
        }

        private SearchComparer<TSortKey, TValue> SearchComparer(TSortKey key)
            => new SearchComparer<TSortKey, TValue>
            {
                SearchKey = key,
                Comparer = GetSortKeyComparer(),
                GetKey = GetSortKey
            };


        public IEnumerable<TGroupKey> Keys
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
                    foreach (var itemValue in groupItem.Value.Values)
                    {
                        yield return itemValue;
                    }
                }
            }
        }

        public GroupInfoHelper<TGroupKey, TInfo, TValue> GroupInfo
        {
            get => new GroupInfoHelper<TGroupKey, TInfo, TValue>(m_dictionary);
        }

        public class GroupInfoHelper<TGroupInfoKey, TGroupInfo, TSortValue>
        {
            private static ConcurrentDictionary<TGroupInfoKey, SortedGroup<TGroupInfo, TSortValue>> s_dictionary;
            internal GroupInfoHelper(ConcurrentDictionary<TGroupInfoKey, SortedGroup<TGroupInfo, TSortValue>> dictionary)
            {
                s_dictionary ??= dictionary;
            }

            public TGroupInfo this[TGroupInfoKey key]
            {
                get => s_dictionary.TryGetValue(key, out var group) ? group.GroupInfo : default;
                set
                {
                    if (s_dictionary.TryGetValue(key, out var group))
                    {
                        s_dictionary.TryUpdate(key,
                            new SortedGroup<TGroupInfo, TSortValue>
                            {
                                GroupInfo = value, Values = group.Values
                            }, group);
                    }
                    else
                    {
                        s_dictionary.TryAdd(key, new SortedGroup<TGroupInfo, TSortValue>
                        {
                            GroupInfo = value, Values = Array.Empty<TSortValue>()
                        });
                    }
                } 
            }
        }

        internal struct SortedGroup<TGroupInfo, TSortValue> : IEquatable<SortedGroup<TGroupInfo, TSortValue>>
        {
            public TGroupInfo GroupInfo;
            public TSortValue[] Values;

            public bool Equals(SortedGroup<TGroupInfo, TSortValue> other)
                => Values == other.Values;
        }

        internal struct Enumerable : IEnumerable<TValue>, IEnumerable
        {
            private TValue[] m_values;

            public Enumerable(TValue[] values) => m_values = values;

            public IEnumerator<TValue> GetEnumerator() => new Enumerator(m_values);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        internal struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private TValue[]m_values;
            private int m_index;
            private TValue m_current;

            public Enumerator(TValue[] values)
            {
                m_values = values;
                m_index = 0;
                m_current = default;
            }

            public TValue Current => m_current;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (m_index < m_values.Length)
                {
                    m_current = m_values[m_index];
                    m_index++;

                    return true;
                }
                return false;
            }

            void IEnumerator.Reset()
            {
                m_index = 0;
                m_current = default;
            }
        }

        private ConcurrentDictionary<TGroupKey, SortedGroup<TInfo, TValue>> m_dictionary;
    }

    public class SortedArrayCursor<TValue>
    {
        private TValue[] m_values;
        private readonly Action<TValue[]> m_onUpdate;
        private readonly IComparer<TValue> m_comparer;
        private int m_position;
        public SortedArrayCursor(TValue[] values, IComparer<TValue> comparer, Action<TValue[]> onUpdate)
        {
            m_values = values;
            m_position = 0;
            m_onUpdate = onUpdate;
            m_comparer = comparer;
        }
        public int Length => m_values.Length;
        public TValue this[int i] { get => m_values[i]; set => m_values[i] = value; }

        public bool Move(TValue value)
        {
            m_position = Array.BinarySearch(m_values, value, m_comparer);
            return m_position >= 0;            
        }
        public TValue Current
        {
            get => m_position >= 0 ? m_values[m_position] : default;
            set
            {
                if (m_position >= 0)
                    m_values[m_position] = value;
                else
                {
                    m_position = ~m_position;
                    if (m_position == m_values.Length)
                    {
                        Array.Resize(ref m_values, m_values.Length + 1);
                        m_values[^1] = value;
                        m_onUpdate?.Invoke(m_values);
                    }
                    else
                    {
                        var values = new TValue[m_values.Length + 1];
                        Array.Copy(m_values, 0, values, 0, m_position);
                        values[m_position] = value;
                        Array.Copy(m_values, m_position, values, m_position + 1, m_values.Length - m_position);
                        m_values = values;
                        m_onUpdate?.Invoke(m_values);
                    }
                }
            }
        }
    }

    public abstract class Allocator<TValue>
    {
        public Func<TValue[]> Allocate { get; protected set; }
    }

    public class ReplaceAllocator<TValue> : Allocator<TValue>
    {
        public ReplaceAllocator(TValue value, TValue[] source, IComparer<TValue> comparer)
        {
            base.Allocate = () => Allocate(value, source, comparer);
        }

        private new TValue[] Allocate(TValue value, TValue[] source, IComparer<TValue> comparer)
        {
            if (source != null && source.Length != 0)
            {
                var i = Array.BinarySearch(source, value, comparer);
                if (i >= 0)
                {
                    source[i] = value;
                }
                else
                {
                    source.CopyTo(source = new TValue[source.Length + 1], 1);
                    source[0] = value;
                    Array.Sort(source, comparer); 
                }

                return source;
            }
            else
            {
                return new[] { value };
            }
        }
    }

    public class RemoveAllocator<TValue> : Allocator<TValue>
    {
        public RemoveAllocator(TValue[] source, IComparer<TValue> comparer)
        {
            base.Allocate = () => Allocate(source, comparer);
        }

        private new TValue[] Allocate(TValue[] source, IComparer<TValue> comparer)
        {
            var i = Array.BinarySearch(source, default, comparer);
            if (i >= 0)
            {
                var @new = new TValue[source.Length - 1];
                Array.Copy(source, @new, i);
                Array.Copy(source, i + 1, @new, i, @new.Length - i);
                source = @new;
            }
            return source;
        }
    }

    public class ValuesAllocator<TValue> : Allocator<TValue>
    {
        public Func<TValue, TValue, TValue> Update;
        public ValuesAllocator(IEnumerable<TValue> values, TValue[] source, IComparer<TValue> comparer)
        {
            base.Allocate = () => Allocate(values, source, comparer);
        }

        private new TValue[] Allocate(IEnumerable<TValue> values, TValue[] source, IComparer<TValue> comparer)
        {
            if (source != null && source.Length != 0)
            {
                foreach (var value in values)
                {
                    source = new AddOrUpdateAllocator<TValue>(source, comparer)
                    {
                        Value = value, Update = old => Update(old, value)
                    }.Allocate();
                }

                return source;
            }
            else
            {
                source = values.ToArray();
                Array.Sort(source, comparer);
                return source;
            }
        }
    }

    public class UpdatesAllocator<TKey, TUpdate, TValue> : Allocator<TValue>
    {
        public Func<TUpdate, TKey> GetKey;
        public Func<TUpdate, TValue> Create;
        public IComparer<TValue> SortComparer;
        public Func<TValue, TUpdate, TValue> Update;
        public UpdatesAllocator(IEnumerable<TUpdate> updates, TValue[] source, Func<TKey, IComparer<TValue>> getComparer)
        {
            base.Allocate = () => Allocate(updates, source, getComparer);
        }

        private new TValue[] Allocate(IEnumerable<TUpdate> updates, TValue[] source, Func<TKey, IComparer<TValue>> getComparer)
        {
            if (source != null && source.Length != 0)
            {
                // вычислить новый 
                var newValues = UpdatesAggregator(NewValues(updates, source, getComparer).ToArray())
                    .ToArray();

                if (newValues.Length > 0)
                {
                    source.CopyTo(source = new TValue[source.Length + newValues.Length], newValues.Length);
                    newValues.CopyTo(source, 0);
                    Array.Sort(source, SortComparer);
                }

                return source;
            }
            else
            {
                source = UpdatesAggregator(updates).ToArray();
                Array.Sort(source, SortComparer);
                return source;
            }
        }

        private IEnumerable<TUpdate> NewValues(IEnumerable<TUpdate> updates, TValue[] source, Func<TKey, IComparer<TValue>> getComparer)
        {
            foreach (var update in updates)
            {
                var i = Array.BinarySearch(source, default, getComparer(GetKey(update)));
                if (i >= 0)
                {
                    source[i] = Update(source[i], update);
                }
                else
                {
                    yield return update;
                }
            }
        }

        private IEnumerable<TValue> UpdatesAggregator(IEnumerable<TUpdate> updates)
        {
            foreach (var updateGroup in updates.GroupBy(GetKey))
            {
                using var updatesEnumerator = updateGroup.GetEnumerator();

                updatesEnumerator.MoveNext();
                var value = Create(updatesEnumerator.Current);

                while (updatesEnumerator.MoveNext())
                    Update(value, updatesEnumerator.Current);

                yield return value;
            }
        }
    }

    public class SearchComparer<TKey, TValue> : IComparer<TValue>
    {
        public TKey SearchKey;
        public IComparer<TKey> Comparer;
        public Func<TValue, TKey> GetKey;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(TValue x, TValue y) => Comparer.Compare(GetKey(x), SearchKey);

        public IComparer<TValue> GetComparer(TKey searchKey)
        {
            SearchKey = searchKey;
            return this;
        }
    }

    public class AddOrUpdateAllocator<TValue> : Allocator<TValue>
    {
        public TValue Value;
        public Func<TValue> Create;
        public Func<TValue, TValue> Update;

        public AddOrUpdateAllocator(TValue[] source, IComparer<TValue> comparer)
        {
            base.Allocate = () => Allocate(source, comparer);
        }

        private new TValue[] Allocate(TValue[] source, IComparer<TValue> comparer)
        {
            if (source != null && source.Length != 0)
            {
                var i = Array.BinarySearch(source, Value, comparer);
                if (i >= 0)
                {
                    source[i] = Update(source[i]);
                }
                else
                {
                    source.CopyTo(source = new TValue[source.Length + 1], 1);
                    source[0] = Value ?? Create();
                    Array.Sort(source, comparer);
                }

                return source;
            }
            else
            {
                return new[] { Value ?? Create() };
            }
        }
    }

    public class LastAppendAllocator<TValue> : Allocator<TValue>
    {
        public TValue PrevValue;
        public TValue LastValue;
        public Func<TValue> Create;
        public Func<TValue, TValue> Update;

        public LastAppendAllocator(TValue[] source, IComparer<TValue> comparer)
        {
            base.Allocate = () => Allocate(source, comparer);
        }

        private new TValue[] Allocate(TValue[] source, IComparer<TValue> comparer)
        {
            var i = Array.BinarySearch(source, PrevValue, comparer);
            if (i >= 0)
            {
                i++;
                if (i < source.Length)
                    Array.Copy(source, i, source, i - 1, source.Length - i);
                source[source.Length-1] = LastValue;
            }
            else
                throw new Exception("!!!!");
            return source;
        }
    }
}
