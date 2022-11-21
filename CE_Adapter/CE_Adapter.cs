namespace Vtb.Trade.Grpc.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Reflection;

    public class CE_Adapter
    {
        public const int iMaskFieldNo = 257; // в маске помещается только 256 флагов полей
        public CE_Adapter() : this(0) { }
        public CE_Adapter(int entityType) : this(new CE { EntityType = entityType }) { }
        public CE_Adapter(CE entity) { Init(entity);  }
        public virtual void RegIdentity(Action<CE.FieldValue> identityRegistration) { }
        public virtual void Reset()
        {
            var entityType = m_entity.EntityType;
            var capacity = m_entity.Fields.Capacity;
            Init(new CE { EntityType = entityType });
            m_entity.Fields.Capacity = capacity;
        }
        private void Init(CE entity)
        {
            m_entity = entity;

            if (m_entity.Fields.Count == 0 || m_entity.Fields[0].Number != iMaskFieldNo)
                m_entity.Fields.Insert(0, (iMaskFieldNo, CE.Repeat.Create(0U) ));
            m_mask = m_entity.Fields[0];
        }
        public bool IsEmpty => m_mask.AsRepeat.Items.Count == 1 && m_mask.AsRepeat.Items[0].AsLong == 0;
        public bool IsNotEmpty => m_mask.AsRepeat.Items.Count > 1 || m_mask.AsRepeat.Items[0].AsLong != 0;
        public bool CheckIndex(int index)=> CheckIndex(index, out _);        
        public bool CheckIndex(int index, out (CE.RepeatItem mask, int prev_count) maskInfo)
        {
            maskInfo = GetMask(index);
            return 0 != (maskInfo.mask.AsULong & (1UL << (index % 64)));
        }

        private bool GetOrdinal(int index, out (int ordinal, CE.RepeatItem mask) ordinalInfo)
        {
            var maskInfo = GetMask(index);
            return GetOrdinal(index, ref maskInfo, out ordinalInfo);
        }

        private bool GetOrdinal(int index, ref (CE.RepeatItem mask, int prev_count) maskInfo, out (int ordinal, CE.RepeatItem mask) ordinalInfo)
        {
            ordinalInfo = (default, maskInfo.mask);
            index %= 64;

            ulong check_mask = (2UL << index) - 1;

            var mask = maskInfo.mask.AsULong;
            if ((mask &= check_mask) == 0 && maskInfo.prev_count == 0)
                return false;

            var ordinal = BitOperations.PopCount(mask) + maskInfo.prev_count;
            ordinalInfo = (ordinal, maskInfo.mask);
            return true;
        }
        private (CE.RepeatItem mask, int prev_count) GetMask(int index)
        {
            var mask = m_mask.AsRepeat.Items[0];

            if (index < 64)
                return (mask, 0);

            if (index < 128)
            {
                if (m_mask.AsRepeat.Items.Count < 2)
                {
                    m_mask.AsRepeat.Items.Add(mask = CE.RepeatItem.Create(0L));
                }
                else
                    mask = m_mask.AsRepeat.Items[1];

                return (mask, BitOperations.PopCount(m_mask.AsRepeat.Items[0].AsULong));
            }

            if (index < 192)
            {
                while (m_mask.AsRepeat.Items.Count < 3)
                {
                    m_mask.AsRepeat.Items.Add(CE.RepeatItem.Create(0L));
                }
                mask = m_mask.AsRepeat.Items[2];

                return (mask, BitOperations.PopCount(m_mask.AsRepeat.Items[0].AsULong)
                    + BitOperations.PopCount(m_mask.AsRepeat.Items[1].AsULong));
            }

            if (index < 256)
            {
                while (m_mask.AsRepeat.Items.Count < 4)
                {
                    m_mask.AsRepeat.Items.Add(CE.RepeatItem.Create(0L));
                }
                
                mask = m_mask.AsRepeat.Items[3];

                return (mask, BitOperations.PopCount(m_mask.AsRepeat.Items[0].AsULong)
                    + BitOperations.PopCount(m_mask.AsRepeat.Items[1].AsULong)
                    + BitOperations.PopCount(m_mask.AsRepeat.Items[2].AsULong));
            }

            throw new ArgumentOutOfRangeException();
        }

        public CE.FieldValue this[int index]
        {
            get => CheckIndex(index, out var maskInfo) && GetOrdinal(index, ref maskInfo, out var ordinalInfo) ? 
                m_entity.Fields[ordinalInfo.ordinal] : default;
            set
            {
                if (value != null)
                {
                    if (GetOrdinal(index, out var ordinalInfo))
                    {
                        if ((ordinalInfo.mask.AsULong & (1UL << index)) == 0)
                            m_entity.Fields.Insert(ordinalInfo.ordinal + 1, value);
                        else
                            m_entity.Fields[ordinalInfo.ordinal] = value;
                    }
                    else
                        m_entity.Fields.Insert(1, value);

                    ordinalInfo.mask.AsULong |= 1UL << (index % 64);
                }
            }
        }

        public IEnumerable<int> FilledFields
        {
            get 
            {
                var offset = 0;
                var items = m_mask.AsRepeat.Items;
                for(int i = 0, N = items.Count; i < N; i ++, offset += 64)
                {
                    var mask = items[i].AsULong;
                    if (mask != 0)
                    {
                        ulong m = 1;
                        for (int j = 0; j < 64; j++, m = 1UL << j)
                        {
                            if ((mask & m) == 0) continue;

                            yield return j + offset;
                        }
                    }
                }
            }
        }

        private CE m_entity;
        private CE.FieldValue m_mask;

        public CE Entity { get => m_entity; }
        public static implicit operator CE (CE_Adapter value) => value.m_entity;

        public override string ToString() => m_entity.ToString();
        public string AsFlatToString() => m_entity.AsFlatString();
    }

    public static class CE_Adapter_Extensions
    {
        public static CE_Adapter AsAdapter(this CE entity) => new CE_Adapter(entity);

        public static IEnumerable<FieldInfo> FieldIndexConstants<TAdapter>() where TAdapter : CE_Adapter
            => typeof(TAdapter).FieldIndexConstants();

        public static IEnumerable<FieldInfo> FieldIndexConstants(this Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(int)
                && fi.Name.StartsWith("i"));

        public static string CE_Adapter_MetaInfo(this Type type)
            => string.Join(Environment.NewLine,
                type.FieldIndexConstants().Select(
                    fi => $"\"{(int)fi.GetRawConstantValue()+1}, {fi.Name.Substring(1)}, {fi.GetPropertyType()}\",")
                .Prepend($"\"{type.Name}\":[")
                .Append("]")
                );

        private static PropertyInfo GetProperty(this FieldInfo propertyConstant)
            // пропустим i
            => propertyConstant.ReflectedType.GetProperty(propertyConstant.Name.Substring(1));

        private static string GetPropertyType(this FieldInfo fieldInfo)
            => fieldInfo.GetProperty().PropertyType.GetParamType();

        public static string GetParamType(this Type type)
        {
            if (type == typeof(string))
                return "string";
            else
            {
                if (type == typeof(int))
                    return "int";
                else
                {
                    if (type == typeof(long))
                        return "long";
                    else
                    {
                        if (type == typeof(DateTime))
                            return "DateTime";
                        else
                        {
                            if (type == typeof(double))
                                return "double";
                            else
                            {
                                if (type == typeof(char))
                                    return "char";
                                else
                                {
                                    if (type == typeof(bool))
                                        return "bool";
                                    else
                                    {
                                        if (type.GetGenericTypeDefinition() == typeof(IS<>))
                                            return "string";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new Exception($"Тип {type} параметра не определен!");
        }

    }
}
