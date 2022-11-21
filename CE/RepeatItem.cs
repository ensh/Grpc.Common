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
        public sealed class RepeatItem : IMessage<RepeatItem>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(bool value)
                => new RepeatItem { AsBoolean = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(char value)
                => new RepeatItem { AsChar = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(double value)
                => new RepeatItem { AsDouble = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(decimal value)
                => new RepeatItem { AsDouble = (double)value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(float value)
                => new RepeatItem { AsDouble = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(string value)
                => new RepeatItem { AsString = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(DateTime value)
                => new RepeatItem { AsDateTime = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(int value)
                => new RepeatItem { AsInteger = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(uint value)
                => new RepeatItem { AsUInteger = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(long value)
                => new RepeatItem { AsLong = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(ulong value)
                => new RepeatItem { AsULong = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static RepeatItem Create(CE value)
                => new RepeatItem { AsEntity = value };

            public RepeatItem() { }

            public RepeatItem(RepeatItem other) : this()
            {
                switch ((int)other.m_type)
                {
                    case (int)ValueType.AsBoolean:
                        AsBoolean = other.m_value.asBool;
                        break;
                    case (int)ValueType.AsChar:
                        AsChar = other.m_value.asChar;
                        break;
                    case (int)ValueType.AsDateTime:
                        AsDateTime = other.AsDateTime;
                        break;
                    case (int)ValueType.AsDouble:
                        AsDouble = other.m_value.asDouble;
                        break;
                    case (int)ValueType.AsEntity:
                        AsEntity = other.AsEntity.Clone();
                        break;
                    case (int)ValueType.AsInteger:
                        AsInteger = other.m_value.asInt;
                        break;
                    case (int)ValueType.AsLong:
                        AsLong = other.m_value.asLong;
                        break;
                    case (int)ValueType.AsString:
                        AsString = (string)other.m_object;
                        break;
                }

                _unknownFields = UnknownFieldSet.Clone(other._unknownFields);
            }

            public RepeatItem Clone() => new RepeatItem(this);

            public bool AsNone
            {
                get { return m_type == ValueType.None; }
                set { if (value) m_type = ValueType.None; }
            }

            public bool AsBoolean
            {
                get { return m_type == ValueType.AsBoolean ? m_value.asBool : false; }
                set { m_value.asBool = value; m_type = ValueType.AsBoolean; }
            }

            public double AsDouble
            {
                get { return m_type == ValueType.AsDouble ? m_value.asDouble : 0D; }
                set { m_value.asDouble = value; m_type = ValueType.AsDouble; }
            }

            public int AsInteger
            {
                get { return m_type == ValueType.AsInteger ? m_value.asInt : 0; }
                set { m_value.asInt = value; m_type = ValueType.AsInteger; }
            }

            public uint AsUInteger
            {
                get { return m_type == ValueType.AsInteger ? m_value.asUInt : 0; }
                set { m_value.asUInt = value; m_type = ValueType.AsInteger; }
            }

            public long AsLong
            {
                get { return m_type == ValueType.AsLong || m_type == ValueType.AsDateTime ? m_value.asLong : 0L; }
                set { m_value.asLong = value; m_type = ValueType.AsLong; }
            }

            public ulong AsULong
            {
                get { return m_type == ValueType.AsLong ? m_value.asULong : 0UL; }
                set { m_value.asULong = value; m_type = ValueType.AsLong; }
            }

            public string AsString
            {
                get { return m_type == ValueType.AsString ? (string)m_object : (m_object is string ? (string)m_object : ""); }
                set { m_object = ProtoPreconditions.CheckNotNull(value, "value"); m_type = ValueType.AsString; }
            }

            public CE AsEntity
            {
                get { return m_type == ValueType.AsEntity ? (CE)m_object : null; }
                set { m_object = value; m_type = value == null ? ValueType.None : ValueType.AsEntity; }
            }

            public object Object
            {
                get => m_object; set { m_object = value; }
            }

            public char AsChar
            {
                get { return m_type == ValueType.AsChar ? m_value.asChar : default(char); }
                set { m_value.asChar = value; m_type = ValueType.AsChar; }
            }

            public DateTime AsDateTime
            {
                get { return m_type == ValueType.AsDateTime ? new DateTime(m_value.asLong) : default(DateTime); }
                set { m_value.asLong = value.Ticks; m_type = ValueType.AsDateTime; }
            }

            private InternalValue m_value;
            private object m_object;
            private ValueType m_type = ValueType.None;
            public ValueType ItemType { get => m_type; }

            public void ClearValue() { m_type = ValueType.None; m_value = default(InternalValue); }

            public override bool Equals(object other) => Equals(other as RepeatItem);
            public bool Equals(RepeatItem other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }

                if (m_type != other.m_type) return false;

                switch ((int)m_type)
                {
                    case (int)ValueType.AsBoolean:
                        return m_value.asBool == other.m_value.asBool;
                    case (int)ValueType.AsChar:
                        return m_value.asChar == other.m_value.asChar;
                    case (int)ValueType.AsDouble:
                        return ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(m_value.asDouble, other.m_value.asDouble);
                    case (int)ValueType.AsEntity:
                        return object.Equals(AsEntity, other.AsEntity);
                    case (int)ValueType.AsInteger:
                        return m_value.asInt == other.m_value.asInt;
                    case (int)ValueType.AsDateTime:
                    case (int)ValueType.AsLong:
                        return m_value.asLong == other.m_value.asLong;
                    case (int)ValueType.AsString:
                        return string.CompareOrdinal((string)m_object, (string)other.m_object) == 0;
                }

                return Equals(_unknownFields, other._unknownFields);
            }

            public override int GetHashCode()
            {
                int hash = 1;

                switch ((int)m_type)
                {
                    case (int)ValueType.AsBoolean:
                        hash ^= m_value.asBool.GetHashCode();
                        break;
                    case (int)ValueType.AsChar:
                        hash ^= m_value.asChar.GetHashCode();
                        break;
                    case (int)ValueType.AsDouble:
                        hash ^= ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(m_value.asDouble);
                        break;
                    case (int)ValueType.AsEntity:
                        hash ^= m_object.GetHashCode();
                        break;
                    case (int)ValueType.AsInteger:
                        hash ^= m_value.asInt;
                        break;
                    case (int)ValueType.AsDateTime:
                    case (int)ValueType.AsLong:
                        hash ^= m_value.asLong.GetHashCode();
                        break;
                    case (int)ValueType.AsString:
                        hash ^= m_object.GetHashCode();
                        break;
                }

                hash ^= (int)m_type;

                if (_unknownFields != null)
                {
                    hash ^= _unknownFields.GetHashCode();
                }
                return hash;
            }

            public override string ToString()
            {
                switch ((int)m_type)
                {
                    case (int)ValueType.AsBoolean:
                        return string.Concat("AsBoolean=", m_value.asBool.ToString());
                    case (int)ValueType.AsChar:
                        return string.Concat("AsChar=", m_value.asChar);
                    case (int)ValueType.AsDateTime:
                        return string.Concat("AsDateTime=", AsDateTime.ToString());
                    case (int)ValueType.AsDouble:
                        return string.Concat("AsDouble=", m_value.asDouble.ToString("G", CultureInfo.InvariantCulture));
                    case (int)ValueType.AsEntity:
                        return string.Concat("AsEntity=", AsEntity.ToString());
                    case (int)ValueType.AsInteger:
                        return string.Concat("AsInteger=", m_value.asInt.ToString());
                    case (int)ValueType.AsLong:
                        return string.Concat("AsLong=", m_value.asLong.ToString());
                    case (int)ValueType.AsString:
                        return string.Concat("AsString=", (string)m_object);
                }

                return "";
            }

            public IEnumerable<string> Strings
            {
                get
                {
                    switch ((int)m_type)
                    {
                        case (int)ValueType.AsBoolean:
                            yield return string.Concat(m_value.asBool.ToString(), ", ");
                            break;
                        case (int)ValueType.AsChar:
                            yield return string.Concat(m_value.asChar, ", ");
                            break;
                        case (int)ValueType.AsDateTime:
                            yield return string.Concat(AsDateTime.ToString(), ", ");
                            break;
                        case (int)ValueType.AsDouble:
                            yield return string.Concat(m_value.asDouble.ToString("G", CultureInfo.InvariantCulture), ", ");
                            break;
                        case (int)ValueType.AsEntity:
                            yield return "AsEntity=";
                            foreach (var str in AsEntity.Strings)
                                yield return str;
                            yield return ", ";
                            break;
                        case (int)ValueType.AsInteger:
                            yield return string.Concat(m_value.asInt.ToString(), ", ");
                            break;
                        case (int)ValueType.AsLong:
                            yield return string.Concat(m_value.asLong.ToString(), ", ");
                            break;
                        case (int)ValueType.AsString:
                            yield return string.Concat((string)m_object, ", ");
                            break;
                    }
                }
            }

            #region protobuf
            private static readonly MessageParser<RepeatItem> _parser = new MessageParser<RepeatItem>(() => new RepeatItem());
            private UnknownFieldSet _unknownFields;
            public static MessageParser<RepeatItem> Parser { get => _parser; }
            public static MessageDescriptor Descriptor { get => null; }
            MessageDescriptor IMessage.Descriptor { get => Descriptor; }

            public void WriteTo(CodedOutputStream output)
            {
                switch ((int)m_type)
                {
                    case (int)ValueType.AsDouble:
                        output.WriteRawTag(17);
                        output.WriteDouble(m_value.asDouble);
                        break;
                    case (int)ValueType.AsInteger:
                        output.WriteRawTag(24);
                        output.WriteInt32(m_value.asInt);
                        break;
                    case (int)ValueType.AsLong:
                        output.WriteRawTag(32);
                        output.WriteInt64(m_value.asLong);
                        break;
                    case (int)ValueType.AsBoolean:
                        output.WriteRawTag(40);
                        output.WriteBool(m_value.asBool);
                        break;
                    case (int)ValueType.AsString:
                        output.WriteRawTag(50);
                        output.WriteString((string)m_object);
                        break;
                    case (int)ValueType.AsEntity:
                        output.WriteRawTag(66);
                        output.WriteMessage((CE)m_object);
                        break;
                    case (int)ValueType.AsChar:
                        output.WriteRawTag(72);
                        output.WriteUInt32(m_value.asChar);
                        break;
                    case (int)ValueType.AsDateTime:
                        output.WriteRawTag(80);
                        output.WriteInt64(m_value.asLong);
                        break;
                }

                if (_unknownFields != null)
                {
                    _unknownFields.WriteTo(output);
                }
            }

            public int CalculateSize()
            {
                int size = 0;

                switch ((int)m_type)
                {
                    case (int)ValueType.AsBoolean:
                        size += 1 + 1;
                        break;
                    case (int)ValueType.AsChar:
                        size += 1 + CodedOutputStream.ComputeUInt32Size(m_value.asChar);
                        break;
                    case (int)ValueType.AsDouble:
                        size += 1 + 8;
                        break;
                    case (int)ValueType.AsEntity:
                        size += 1 + CodedOutputStream.ComputeMessageSize((CE)m_object);
                        break;
                    case (int)ValueType.AsInteger:
                        size += 1 + CodedOutputStream.ComputeInt32Size(m_value.asInt);
                        break;
                    case (int)ValueType.AsDateTime:
                    case (int)ValueType.AsLong:
                        size += 1 + CodedOutputStream.ComputeInt64Size(m_value.asLong);
                        break;
                    case (int)ValueType.AsString:
                        size += 1 + CodedOutputStream.ComputeStringSize((string)m_object);
                        break;
                }

                if (_unknownFields != null)
                {
                    size += _unknownFields.CalculateSize();
                }
                return size;
            }

            public void MergeFrom(RepeatItem other)
            {
                if (other == null)
                {
                    return;
                }
                switch ((int)other.m_type)
                {
                    case (int)ValueType.AsBoolean:
                        AsBoolean = other.m_value.asBool;
                        break;
                    case (int)ValueType.AsChar:
                        AsChar = other.m_value.asChar;
                        break;
                    case (int)ValueType.AsDateTime:
                        AsDateTime = other.AsDateTime;
                        break;
                    case (int)ValueType.AsDouble:
                        AsDouble = other.m_value.asDouble;
                        break;
                    case (int)ValueType.AsInteger:
                        AsInteger = other.m_value.asInt;
                        break;
                    case (int)ValueType.AsLong:
                        AsLong = other.m_value.asLong;
                        break;
                    case (int)ValueType.AsString:
                        AsString = (string)other.m_object;
                        break;
                    case (int)ValueType.AsEntity:
                        if (AsEntity == null)
                        {
                            AsEntity = new CE();
                        }
                        AsEntity.MergeFrom((CE)other.m_object);
                        break;
                }

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
                        case 17:
                            {
                                AsDouble = input.ReadDouble();
                                break;
                            }
                        case 24:
                            {
                                AsInteger = input.ReadInt32();
                                break;
                            }
                        case 32:
                            {
                                AsLong = input.ReadInt64();
                                break;
                            }
                        case 40:
                            {
                                AsBoolean = input.ReadBool();
                                break;
                            }
                        case 50:
                            {
                                AsString = input.ReadString();
                                break;
                            }
                        case 66:
                            {
                                CE subBuilder = new CE();
                                if (m_type == ValueType.AsEntity)
                                {
                                    subBuilder.MergeFrom(AsEntity);
                                }
                                input.ReadMessage(subBuilder);
                                AsEntity = subBuilder;
                                break;
                            }
                        case 72:
                            {
                                AsChar = (char)input.ReadUInt32();
                                break;
                            }
                        case 80:
                            {
                                AsDateTime = new DateTime(input.ReadInt64());
                                break;
                            }

                    }
                }
            }
            #endregion
        }
    }
}
