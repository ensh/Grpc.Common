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
        public sealed class FieldValue : IMessage<FieldValue>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, bool value)
                => new FieldValue { Number = number, AsBoolean = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, char value)
                => new FieldValue { Number = number, AsChar = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, double value)
                => new FieldValue { Number = number, AsDouble = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, decimal value)
                => new FieldValue { Number = number, AsDouble = (double)value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, float value)
                => new FieldValue { Number = number, AsDouble = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, string value)
                => value == default(string) ? default : new FieldValue { Number = number, AsString = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, DateTime value)
                => new FieldValue { Number = number, AsDateTime = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, int value)
                => new FieldValue { Number = number, AsInteger = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, uint value)
                => new FieldValue { Number = number, AsUInteger = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, long value)
                => new FieldValue { Number = number, AsLong = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, ulong value)
                => new FieldValue { Number = number, AsULong = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, CE value)
                => value == default(CE) ? default : new FieldValue { Number = number, AsEntity = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FieldValue Create(int number, Repeat value)
                => value == default(Repeat) ? default : new FieldValue { Number = number, AsRepeat = value };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, bool) value)
                => new FieldValue { Number = value.Item1, AsBoolean = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, char) value)
                => new FieldValue { Number = value.Item1, AsChar = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, DateTime) value)
                => new FieldValue { Number = value.Item1, AsDateTime = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, int) value)
                => new FieldValue { Number = value.Item1, AsInteger = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, uint) value)
                => new FieldValue { Number = value.Item1, AsUInteger = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, long) value)
                => new FieldValue { Number = value.Item1, AsLong = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, ulong) value)
                => new FieldValue { Number = value.Item1, AsULong = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, decimal) value)
                => new FieldValue { Number = value.Item1, AsDouble = (double)value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, double) value)
                => new FieldValue { Number = value.Item1, AsDouble = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, float) value)
                => new FieldValue { Number = value.Item1, AsDouble = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, string) value)
                => value.Item2 == default(string) ? default : new FieldValue { Number = value.Item1, AsString = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, CE) value)
                => value.Item2 == default(CE) ? default : new FieldValue { Number = value.Item1, AsEntity = value.Item2 };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator FieldValue((int, Repeat) value)
                => value.Item2 == default(Repeat) ? default : new FieldValue { Number = value.Item1, AsRepeat = value.Item2 };

            public FieldValue() { }

            public FieldValue(FieldValue other) : this()
            {
                m_number = other.m_number;
                switch ((int)other.m_type)
                {
                    case (int)ValueType.AsRepeat:
                        AsRepeat = ((Repeat)other.m_object).Clone();
                        break;
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

            public FieldValue Clone() => new FieldValue(this);
            public FieldValue Clone(int number) => new FieldValue(this) { m_number = number };

            public const int NumberFieldNumber = 1;
            private int m_number;
            public int Number { get => m_number; set => m_number = value; }

            public const int AsDoubeFieldNumber = 2;
            public double AsDouble
            {
                get => m_type == ValueType.AsDouble ? m_value.asDouble : 0D;
                set { m_value.asDouble = value; m_type = ValueType.AsDouble; }
            }

            public const int AsIntegerFieldNumber = 3;
            public int AsInteger
            {
                get => m_type == ValueType.AsInteger ? m_value.asInt : 0;
                set { m_value.asInt = value; m_type = ValueType.AsInteger; }
            }

            public uint AsUInteger
            {
                get => m_type == ValueType.AsInteger ? m_value.asUInt : 0;
                set { m_value.asUInt = value; m_type = ValueType.AsInteger; }
            }

            public const int AsLongFieldNumber = 4;
            public long AsLong
            {
                get => m_type == ValueType.AsLong || m_type == ValueType.AsDateTime ? m_value.asLong : 0L;
                set { m_value.asLong = value; m_type = ValueType.AsLong; }
            }

            public ulong AsULong
            {
                get => m_type == ValueType.AsLong ? m_value.asULong : 0UL;
                set { m_value.asULong = value; m_type = ValueType.AsLong; }
            }

            public const int AsBoolFieldNumber = 5;
            public bool AsBoolean
            {
                get => m_type == ValueType.AsBoolean ? m_value.asBool : false;
                set { m_value.asBool = value; m_type = ValueType.AsBoolean; }
            }

            public const int AsStringFieldNumber = 6;
            public string AsString
            {
                get { return m_type == ValueType.AsString ? (string)m_object : (m_object is string ? (string)m_object : ""); }
                set { m_object = ProtoPreconditions.CheckNotNull(value, "value"); m_type = ValueType.AsString; }
            }

            public const int AsArrayFieldNumber = 7;
            public Repeat AsRepeat
            {
                get => m_type == ValueType.AsRepeat ? (Repeat)m_object : null;
                set { m_object = value; m_type = value == null ? ValueType.None : ValueType.AsRepeat; }
            }

            public const int AsEntityFieldNumber = 8;
            public CE AsEntity
            {
                get => m_type == ValueType.AsEntity ? (CE)m_object : null;
                set { m_object = value; m_type = value == null ? ValueType.None : ValueType.AsEntity; }
            }

            public const int AsCharFieldNumber = 9;
            public char AsChar
            {
                get => m_type == ValueType.AsChar ? m_value.asChar : default(char);
                set { m_value.asChar = value; m_type = ValueType.AsChar; }
            }

            public const int AsDateTimeFieldNumber = 10;
            public DateTime AsDateTime
            {
                get { return m_type == ValueType.AsDateTime ? new DateTime(m_value.asLong) : default(DateTime); }
                set { m_value.asLong = value.Ticks; m_type = ValueType.AsDateTime; }
            }

            private InternalValue m_value;
            private object m_object;

            private ValueType m_type = ValueType.None;
            public ValueType ValueType { get => m_type; }

            public void ClearValue() { m_type = ValueType.None; m_value = default(InternalValue); }

            public override bool Equals(object other) => Equals(other as FieldValue);

            public bool Equals(FieldValue other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }

                if (m_number != other.m_number) return false;
                if (m_type != other.m_type) return false;

                switch ((int)m_type)
                {
                    case (int)ValueType.AsBoolean:
                        return AsBoolean == other.m_value.asBool;
                    case (int)ValueType.AsChar:
                        return AsChar == other.m_value.asChar;
                    case (int)ValueType.AsDouble:
                        return ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(m_value.asDouble, other.m_value.asDouble);
                    case (int)ValueType.AsEntity:
                        return AsEntity.Equals(other.AsEntity);
                    case (int)ValueType.AsInteger:
                        return AsInteger == other.m_value.asInt;
                    case (int)ValueType.AsDateTime:
                    case (int)ValueType.AsLong:
                        return AsLong == other.AsLong;
                    case (int)ValueType.AsString:
                        return string.CompareOrdinal((string)m_object, (string)other.m_object) == 0;
                }

                return Equals(_unknownFields, other._unknownFields);
            }

            public override int GetHashCode()
            {
                int hash = 1;
                if (m_number != 0) hash ^= m_number;

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
                    case (int)ValueType.AsEntity:
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
                        return string.Concat(Number.ToString(), ".AsBoolean=", m_value.asBool.ToString());
                    case (int)ValueType.AsChar:
                        return string.Concat(Number.ToString(), ".AsChar=", m_value.asChar);
                    case (int)ValueType.AsDateTime:
                        return string.Concat(Number.ToString(), ".AsDateTime=", AsDateTime.ToString());
                    case (int)ValueType.AsDouble:
                        return string.Concat(Number.ToString(), ".AsDouble=", m_value.asDouble.ToString("G", CultureInfo.InvariantCulture));
                    case (int)ValueType.AsEntity:
                        return string.Concat(Number.ToString(), ".AsEntity=", m_object.ToString());
                    case (int)ValueType.AsInteger:
                        return string.Concat(Number.ToString(), ".AsInteger=", m_value.asInt.ToString());
                    case (int)ValueType.AsLong:
                        return string.Concat(Number.ToString(), ".AsLong=", m_value.asLong.ToString());
                    case (int)ValueType.AsRepeat:
                        return string.Concat(Number.ToString(), ".AsRepeat=", ((Repeat)m_object).ToString());
                    case (int)ValueType.AsString:
                        return string.Concat(Number.ToString(), ".AsString=", (string)m_object);
                }

                return "";
            }

            public string StringValue
            {
                get
                {
                    switch ((int)m_type)
                    {
                        case (int)ValueType.AsBoolean:
                            return m_value.asBool.ToString();
                        case (int)ValueType.AsChar:
                            return m_value.asChar.ToString();
                        case (int)ValueType.AsDateTime:
                            return AsDateTime.ToString();
                        case (int)ValueType.AsDouble:
                            return m_value.asDouble.ToString("G", CultureInfo.InvariantCulture);
                        case (int)ValueType.AsEntity:
                            return m_object.ToString();
                        case (int)ValueType.AsInteger:
                            return m_value.asInt.ToString();
                        case (int)ValueType.AsLong:
                            return m_value.asLong.ToString();
                        case (int)ValueType.AsRepeat:
                            return ((Repeat)m_object).ToString();
                        case (int)ValueType.AsString:
                            return (string)m_object;
                    }

                    return "";
                }
            }

            public IEnumerable<string> Strings
            {
                get
                {
                    switch ((int)m_type)
                    {
                        case (int)ValueType.AsBoolean:
                            yield return string.Concat(Number.ToString(), ".AsBoolean=", m_value.asBool.ToString());
                            break;
                        case (int)ValueType.AsChar:
                            yield return string.Concat(Number.ToString(), ".AsChar=", m_value.asChar);
                            break;
                        case (int)ValueType.AsDateTime:
                            yield return string.Concat(Number.ToString(), ".AsDateTime=", AsDateTime.ToString());
                            break;
                        case (int)ValueType.AsDouble:
                            yield return string.Concat(Number.ToString(), ".AsDouble=", m_value.asDouble.ToString("G", CultureInfo.InvariantCulture));
                            break;
                        case (int)ValueType.AsEntity:
                            yield return string.Concat(Number.ToString(), ".AsEntity=");
                            foreach (var str in AsEntity.Strings)
                                yield return str;
                            break;
                        case (int)ValueType.AsInteger:
                            yield return string.Concat(Number.ToString(), ".AsInteger=", m_value.asInt.ToString());
                            break;
                        case (int)ValueType.AsLong:
                            yield return string.Concat(Number.ToString(), ".AsLong=", m_value.asLong.ToString());
                            break;
                        case (int)ValueType.AsRepeat:
                            yield return string.Concat(Number.ToString(), ".AsRepeat=");
                            foreach (var str in AsRepeat.Strings)
                                yield return str;
                            break;
                        case (int)ValueType.AsString:
                            yield return string.Concat(Number.ToString(), ".AsString=", (string)m_object);
                            break;
                    }
                }
            }

            #region protobuf
            private static readonly MessageParser<FieldValue> _parser = new MessageParser<FieldValue>(() => new FieldValue());
            private UnknownFieldSet _unknownFields;

            public static MessageParser<FieldValue> Parser { get => _parser; }
            public static MessageDescriptor Descriptor { get => null; }
            MessageDescriptor IMessage.Descriptor { get => Descriptor; }

            public void WriteTo(CodedOutputStream output)
            {
                if (m_number != 0)
                {
                    output.WriteRawTag(8);
                    output.WriteInt32(m_number);
                }

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
                    case (int)ValueType.AsRepeat:
                        output.WriteRawTag(58);
                        output.WriteMessage((Repeat)m_object);
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
                if (Number != 0)
                {
                    size += 1 + CodedOutputStream.ComputeInt32Size(Number);
                }

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
                    case (int)ValueType.AsRepeat:
                        size += 1 + CodedOutputStream.ComputeMessageSize((Repeat)m_object);
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

            public void MergeFrom(FieldValue other)
            {
                if (other == null)
                {
                    return;
                }
                if (other.Number != 0)
                {
                    Number = other.Number;
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
                    case (int)ValueType.AsEntity:
                        if (AsEntity == null)
                        {
                            AsEntity = new CE();
                        }
                        AsEntity.MergeFrom(other.AsEntity);
                        break;
                    case (int)ValueType.AsInteger:
                        AsInteger = other.m_value.asInt;
                        break;
                    case (int)ValueType.AsLong:
                        AsLong = other.m_value.asLong;
                        break;
                    case (int)ValueType.AsRepeat:
                        if (AsRepeat == null)
                        {
                            AsRepeat = new Repeat();
                        }
                        AsRepeat.MergeFrom(other.AsRepeat);
                        break;
                    case (int)ValueType.AsString:
                        AsString = (string)other.m_object;
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
                        case 8:
                            {
                                Number = input.ReadInt32();
                                break;
                            }
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
                        case 58:
                            {
                                Repeat subBuilder = new Repeat();
                                if (m_type == ValueType.AsRepeat)
                                {
                                    subBuilder.MergeFrom(AsRepeat);
                                }
                                input.ReadMessage(subBuilder);
                                AsRepeat = subBuilder;
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
