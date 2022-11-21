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
        public sealed class Repeat : IMessage<Repeat>
        {
            public Repeat() { }
            public Repeat(Repeat other) : this()
            {
                items_ = other.items_.Clone();
                _unknownFields = UnknownFieldSet.Clone(other._unknownFields);
            }

            public static Repeat Create(params RepeatItem[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values);
                return result;
            }

            public static Repeat Create(params int[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params decimal[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params double[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params long[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params float[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params string[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }
            public static Repeat Create(params CE[] values)
            {
                var result = new Repeat();
                result.Items.Capacity = values.Length;
                result.Items.Add(values.Select(v => RepeatItem.Create(v)));
                return result;
            }

            public Repeat Clone() => new Repeat(this);

            private readonly RepeatedField<RepeatItem> items_ = new RepeatedField<RepeatItem>();
            public RepeatedField<RepeatItem> Items { get => items_; }

            public override bool Equals(object other) => Equals(other as Repeat);

            public bool Equals(Repeat other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }
                if (!items_.Equals(other.items_)) return false;
                return Equals(_unknownFields, other._unknownFields);
            }

            public override int GetHashCode()
            {
                int hash = 1;
                hash ^= items_.GetHashCode();
                if (_unknownFields != null)
                {
                    hash ^= _unknownFields.GetHashCode();
                }
                return hash;
            }

            public override string ToString()
            {
                var result = string.Join(Environment.NewLine,
                    Items.Select(f => f.ToString())
                    .Prepend("{")
                    .Append("}")
                );
                return result;
            }

            public IEnumerable<string> Strings
            {
                get => Items.SelectMany(f => f.Strings)
                    .Prepend("{").Append("}");
            }

            #region protobuf

            private static readonly MessageParser<Repeat> _parser = new MessageParser<Repeat>(() => new Repeat());
            private UnknownFieldSet _unknownFields;
            public static MessageParser<Repeat> Parser { get { return _parser; } }

            public static MessageDescriptor Descriptor { get => null; }

            MessageDescriptor IMessage.Descriptor { get => Descriptor; }

            public const int ItemsFieldNumber = 1;
            private static readonly FieldCodec<RepeatItem> _repeated_items_codec = FieldCodec.ForMessage(10, RepeatItem.Parser);

            public void WriteTo(CodedOutputStream output)
            {
                items_.WriteTo(output, _repeated_items_codec);
                if (_unknownFields != null)
                {
                    _unknownFields.WriteTo(output);
                }
            }

            public int CalculateSize()
            {
                int size = 0;
                size += items_.CalculateSize(_repeated_items_codec);
                if (_unknownFields != null)
                {
                    size += _unknownFields.CalculateSize();
                }
                return size;
            }

            public void MergeFrom(Repeat other)
            {
                if (other == null)
                {
                    return;
                }
                items_.Add(other.items_);
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
                        case 10:
                            {
                                items_.AddEntriesFrom(input, _repeated_items_codec);
                                break;
                            }
                    }
                }
            }

            #endregion

        }
    }
}
