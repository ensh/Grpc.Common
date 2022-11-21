namespace Vtb.Trade.Grpc.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Google.Protobuf;
    using Google.Protobuf.Collections;
    using Google.Protobuf.Reflection;

    public sealed partial class CE : IMessage<CE>
    {
        public enum ValueType
        {
            None = 0,
            AsDouble = 2,
            AsInteger = 3,
            AsLong = 4,
            AsBoolean = 5,
            AsString = 6,
            AsRepeat = 7,
            AsEntity = 8,
            AsChar = 9,
            AsDateTime = 10
        }

        public CE() { }
        public CE(CE other) : this()
        {
            m_number = other.m_number;
            m_fields = other.m_fields.Clone();
            _unknownFields = UnknownFieldSet.Clone(other._unknownFields);
        }

        public static CE Create(FieldValue value)
        {
            var result = new CE();
            result.Fields.Add(value);
            return result;
        }

        public static CE Create(params FieldValue[] values)
        {
            var result = new CE();
            result.Fields.Capacity = values.Length;
            result.Fields.Add(values);
            return result;
        }

        public static CE Create(int entityType, params FieldValue[] values)
        {
            var result = new CE() { EntityType = entityType };
            result.Fields.Capacity = values.Length;
            result.Fields.Add(values);
            return result;
        }
        public static CE Create(IList<FieldValue> values)
        {
            var result = new CE();
            result.Fields.Capacity = values.Count;
            result.Fields.Add(values);
            return result;
        }

        public static CE Create(int entityType, IList<FieldValue> values)
        {
            var result = new CE() { EntityType = entityType };
            result.Fields.Capacity = values.Count;
            result.Fields.Add(values);
            return result;
        }
        public CE Clone() => new CE(this);

        public const int EntityTypeFieldNumber = 1;
        private int m_number;
        public int EntityType { get => m_number; set => m_number = value; }

        public const int FieldsFieldNumber = 2;
        private static readonly FieldCodec<FieldValue> _repeated_fields_codec = FieldCodec.ForMessage(18, FieldValue.Parser);

        private readonly RepeatedField<FieldValue> m_fields = new RepeatedField<FieldValue>();

        public RepeatedField<FieldValue> Fields { get => m_fields; }

        public override bool Equals(object other) => Equals(other as CE);

        public bool Equals(CE other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (EntityType != other.EntityType) return false;
            if (!m_fields.Equals(other.m_fields)) return false;
            return Equals(_unknownFields, other._unknownFields);
        }
        public override int GetHashCode()
        {
            int hash = 1;
            if (EntityType != 0) hash ^= EntityType;
            hash ^= m_fields.GetHashCode();
            if (_unknownFields != null)
            {
                hash ^= _unknownFields.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            var result = string.Join(Environment.NewLine, this.StringsWithTabs());
            return result;
        }

        public IEnumerable<string> Strings
        {
            get => Fields.SelectMany(f => f.Strings)
                .Prepend(string.Concat("=", EntityType.ToString()))
                .Prepend("{").Append("}");
        }

        [StructLayout(LayoutKind.Explicit, Size = sizeof(double))]
        private struct InternalValue
        {
            [FieldOffset(0)]
            public bool asBool;
            [FieldOffset(0)]
            public char asChar;
            [FieldOffset(0)]
            public double asDouble;
            [FieldOffset(0)]
            public int asInt;
            [FieldOffset(0)]
            public uint asUInt;
            [FieldOffset(0)]
            public long asLong;
            [FieldOffset(0)]
            public ulong asULong;
        }

        #region protobuf
        private static readonly MessageParser<CE> _parser = new MessageParser<CE>(() => new CE());
        private UnknownFieldSet _unknownFields;
        public static MessageParser<CE> Parser { get => _parser; }
        public static MessageDescriptor Descriptor { get => null; }
        MessageDescriptor IMessage.Descriptor { get => Descriptor; }

        public void WriteTo(CodedOutputStream output)
        {
            if (EntityType != 0)
            {
                output.WriteRawTag(8);
                output.WriteInt32(EntityType);
            }
            m_fields.WriteTo(output, _repeated_fields_codec);
            if (_unknownFields != null)
            {
                _unknownFields.WriteTo(output);
            }
        }

        public int CalculateSize()
        {
            int size = 0;
            if (EntityType != 0)
            {
                size += 1 + CodedOutputStream.ComputeInt32Size(EntityType);
            }
            size += m_fields.CalculateSize(_repeated_fields_codec);
            if (_unknownFields != null)
            {
                size += _unknownFields.CalculateSize();
            }
            return size;
        }

        public void MergeFrom(CE other)
        {
            if (other == null)
            {
                return;
            }
            if (other.EntityType != 0)
            {
                EntityType = other.EntityType;
            }
            m_fields.Add(other.m_fields);
            _unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
        }

        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        _unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
                        break;
                    case 8:
                        {
                            EntityType = input.ReadInt32();
                            break;
                        }
                    case 18:
                        {
                            m_fields.AddEntriesFrom(input, _repeated_fields_codec);
                            break;
                        }
                }
            }
        }
        #endregion
    }

    public static class CE_Format_Util
    {
        public static IEnumerable<string> StringsWithTabs(this CE entity)
            => entity.Strings.WithTabs();

        public static IEnumerable<string> WithTabs(this IEnumerable<string> strings)
        {
            string tabs = string.Empty;

            foreach (var str in strings)
            {
                if (str.StartsWith("}") && tabs.Length > 0)
                    tabs = tabs.Substring(0, tabs.Length - 1);

                yield return string.Concat(tabs, str);

                if (str.StartsWith("{"))
                    tabs = string.Concat(tabs, "\t");
            }
        }

        public static string AsFlatString(this CE entity)
            //=> entity.ToString().Replace(Environment.NewLine, " ").Replace("\t", " ");
            => string.Join(" ", entity.Strings);

        public static string AsFlatString(this CE.Repeat repeat)
            => string.Join(" ", repeat.Strings);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAdapter(this CE entity)
            => entity.Fields.Count > 0 && entity.Fields[0].Number == CE_Adapter.iMaskFieldNo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyAdapter(this CE entity)
            => !entity.IsAdapter() || (
                entity.Fields[0].AsRepeat.Items.Count == 1 && 
                entity.Fields[0].AsRepeat.Items[0].AsLong == 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyAdapterFast(this CE entity)
            => entity.Fields[0].AsRepeat.Items.Count == 1 &&
            entity.Fields[0].AsRepeat.Items[0].AsLong == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmptyAdapterFast(this CE entity)
            => entity.Fields[0].AsRepeat.Items[0].AsLong != 0;
    }
}
