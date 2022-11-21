using Google.Protobuf;
using Google.Protobuf.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vtb.Trade.Grpc.Common
{
    public abstract class RepeatCollection<TItem> : ICollection<TItem>
    {
        protected RepeatedField<CE.RepeatItem> Items;

        public RepeatCollection(CE.Repeat repeat) => Items = repeat?.Items ?? new RepeatedField<CE.RepeatItem>();

        public int Count => Items.Count;

        public bool IsReadOnly => false;

        public abstract void Add(TItem item);

        public void Clear() => Items.Clear();

        public abstract bool Contains(TItem item);

        public abstract void CopyTo(TItem[] array, int arrayIndex);

        public abstract IEnumerator<TItem> GetEnumerator();

        public abstract bool Remove(TItem item);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public sealed class RepeatItemCollection : RepeatCollection<CE.RepeatItem>
    {
        public RepeatItemCollection(CE.Repeat repeat) : base(repeat) { }
        public static implicit operator RepeatItemCollection(CE.FieldValue field)
            => new RepeatItemCollection(field.AsRepeat);

        public override bool Contains(CE.RepeatItem item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item.Equals(Items[i]))
                    return true;
            return false;
        }

        public override void CopyTo(CE.RepeatItem[] array, int arrayIndex)
        {
            for (int i = 0, j = arrayIndex; i < Items.Count && j < array.Length; i++, j++)
                array[i] = Items[i];
        }

        public override void Add(CE.RepeatItem item) => Items.Add(item);

        public override IEnumerator<CE.RepeatItem> GetEnumerator() => Items.GetEnumerator();

        public override bool Remove(CE.RepeatItem item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item.Equals(Items[i]))
                {
                    Items.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }

    public sealed class RepeatIntCollection : RepeatCollection<int>
    {
        public RepeatIntCollection(CE.Repeat repeat) : base(repeat) { }
        public static implicit operator RepeatIntCollection(CE.FieldValue field)
            => new RepeatIntCollection(field.AsRepeat);

        public override bool Contains(int item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsInteger)
                    return true;
            return false;
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            for (int i = 0, j = arrayIndex; i < Items.Count && j < array.Length; i++, j++)
                array[i] = Items[i].AsInteger;
        }

        public override void Add(int item) => Items.Add(CE.RepeatItem.Create(item));

        public override IEnumerator<int> GetEnumerator() => Items
            .Select(item => item.AsInteger).GetEnumerator();

        public override bool Remove(int item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsInteger)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }

    public sealed class RepeatLongCollection : RepeatCollection<long>
    {
        public RepeatLongCollection(CE.Repeat repeat) : base(repeat) { }
        public static implicit operator RepeatLongCollection(CE.FieldValue field)
            => new RepeatLongCollection(field.AsRepeat);

        public override bool Contains(long item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsLong)
                    return true;
            return false;
        }

        public override void CopyTo(long[] array, int arrayIndex)
        {
            for (int i = 0, j = arrayIndex; i < Items.Count && j < array.Length; i++, j++)
                array[i] = Items[i].AsLong;
        }

        public override void Add(long item) => Items.Add(CE.RepeatItem.Create(item));

        public override IEnumerator<long> GetEnumerator() => Items
            .Select(item => item.AsLong).GetEnumerator();

        public override bool Remove(long item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsLong)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }

    public sealed class RepeatStringCollection : RepeatCollection<string>
    {
        public RepeatStringCollection(CE.Repeat repeat) : base(repeat) { }
        public static implicit operator RepeatStringCollection(CE.FieldValue field)
            => new RepeatStringCollection(field.AsRepeat);

        public override bool Contains(string item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsString)
                    return true;
            return false;
        }

        public override void CopyTo(string[] array, int arrayIndex)
        {
            for (int i = 0, j = arrayIndex; i < Items.Count && j < array.Length; i++, j++)
                array[i] = Items[i].AsString;
        }

        public override void Add(string item) => Items.Add(CE.RepeatItem.Create(item));

        public override IEnumerator<string> GetEnumerator() => Items
            .Select(item => item.AsString).GetEnumerator();

        public override bool Remove(string item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (item == Items[i].AsString)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }

    public sealed class RepeatDecimalCollection : RepeatCollection<decimal>
    {
        public RepeatDecimalCollection(CE.Repeat repeat) : base(repeat) { }
        public static implicit operator RepeatDecimalCollection(CE.FieldValue field)
            => new RepeatDecimalCollection(field.AsRepeat);

        public override bool Contains(decimal item)
        {
            var itest = (double)item;
            for (int i = 0; i < Items.Count; i++)
                if (itest == Items[i].AsDouble)
                    return true;
            return false;
        }

        public override void CopyTo(decimal[] array, int arrayIndex)
        {
            for (int i = 0, j = arrayIndex; i < Items.Count && j < array.Length; i++, j++)
                array[i] = (decimal)Items[i].AsDouble;
        }

        public override void Add(decimal item) => Items.Add(CE.RepeatItem.Create(item));

        public override IEnumerator<decimal> GetEnumerator() => Items
            .Select(item => (decimal)item.AsDouble).GetEnumerator();

        public override bool Remove(decimal item)
        {
            var titem = (double)item;
            for (int i = 0; i < Items.Count; i++)
                if (titem == Items[i].AsDouble)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }
}
