using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public class CE_Param
    {
        private readonly CE.RepeatItem Value;

        public CE_Param(string name, bool value)
            : this(name, new CE.RepeatItem { AsBoolean = value })
        {
        }

        public CE_Param(string name, int value)
            : this (name, new CE.RepeatItem { AsInteger = value })
        { 
        }

        public CE_Param(string name, string value)
            : this(name, new CE.RepeatItem { AsString = value })
        {
        }

        public CE_Param(string name, DateTime value)
            : this(name, new CE.RepeatItem { AsDateTime = value })
        {
        }

        public CE_Param(string name)
            : this(name, new CE.RepeatItem() )
        {
        }

        public CE_Param(string name, CE.RepeatItem value)
        {
            Name = name;
            Value = value;
        }
        
        public CE.ValueType ParamType { get => Value.ItemType; }

        public string Name { get; private set; }

        public IEnumerable<CE.RepeatItem> Items
        {
            get 
            {
                yield return new CE.RepeatItem { AsString = Name };
                yield return Value.Clone();
            }
        }

        public static implicit operator int(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return p.Value.AsInteger;
                case CE.ValueType.AsLong:
                    return (int)p.Value.AsLong;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator long(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return p.Value.AsInteger;
                case CE.ValueType.AsLong:
                    return p.Value.AsInteger;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator DateTime(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsDateTime:
                    return p.Value.AsDateTime;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }


        public static implicit operator bool(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsBoolean:
                    return p.Value.AsBoolean;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator string(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsString:
                    return p.Value.AsString;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator decimal(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return p.Value.AsInteger;
                case CE.ValueType.AsLong:
                    return p.Value.AsLong;
                case CE.ValueType.AsDouble:
                    return (decimal)p.Value.AsDouble;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator double(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return p.Value.AsInteger;
                case CE.ValueType.AsLong:
                    return p.Value.AsLong;
                case CE.ValueType.AsDouble:
                    return p.Value.AsDouble;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public static implicit operator float(CE_Param p)
        {
            switch (p.Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return p.Value.AsInteger;
                case CE.ValueType.AsLong:
                    return p.Value.AsLong;
                case CE.ValueType.AsDouble:
                    return (float)p.Value.AsDouble;
            }

            throw new Exception($"несоответствие типа параметра {p.Name} {p.Value.ItemType}");
        }

        public T AsEnum<T>() where T : Enum
        {
            switch (Value.ItemType)
            {
                case CE.ValueType.AsInteger:
                    return (T)Enum.ToObject(typeof(T), Value.AsInteger);
                case CE.ValueType.AsLong:
                    return (T)Enum.ToObject(typeof(T), Value.AsLong);

            }

            throw new Exception($"несоответствие типа параметра {Name} {Value.ItemType}");
        }

        public object AsObject()
        {
            switch ((int)ParamType)
            {
                case (int)CE.ValueType.AsBoolean:
                    return Value.AsBoolean;
                case (int)CE.ValueType.AsChar:
                    return Value.AsChar;
                case (int)CE.ValueType.AsDateTime:
                    return Value.AsDateTime;
                case (int)CE.ValueType.AsDouble:
                    return Value.AsDouble;
                case (int)CE.ValueType.AsInteger:
                    return Value.AsInteger;
                case (int)CE.ValueType.AsLong:
                    return Value.AsLong;
                case (int)CE.ValueType.AsString:
                    return Value.AsString;
            }
            throw new Exception("Тип преобразования не определен");
        }
    }

    public static class CE_Param_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE CreateParameters(this int number, int capasity = 0)
        {
            var result = new CE();
            CE.Repeat repeat;
            result.Fields.Add( (number, repeat = new CE.Repeat()) );
            if (capasity != 0)
                repeat.Items.Capacity = capasity;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE EntityType(this CE entity, int entityType)
        {
            entity.EntityType = entityType;
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this CE entity) => entity.Repeat().Items.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, CE param)
        {
            if (entity.Repeat(out var repeat))
                repeat.Items.Add(CE.RepeatItem.Create(param));
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, CE.RepeatItem param)
        {
            if (entity.Repeat(out var repeat))
                repeat.Items.Add(param);
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, char param)
            => entity.Append(CE.RepeatItem.Create(param));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, double param)
            => entity.Append(CE.RepeatItem.Create(param));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, int param)
            => entity.Append(CE.RepeatItem.Create(param));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, long param)
            => entity.Append(CE.RepeatItem.Create(param));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, string param)
            => entity.Append(CE.RepeatItem.Create(param));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, IEnumerable<CE.RepeatItem> items)
        {
            if (entity.Repeat(out var repeat))
                repeat.Items.Add(items);
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE Append(this CE entity, IEnumerable<CE> items)
        {
            if (entity.Repeat(out var repeat))
                repeat.Items.Add(items.Select(item => CE.RepeatItem.Create(item)));
            return entity;
        }

        public static bool IsRepeat(this CE entity)
            => entity.Fields.Count > 0 && entity.Fields[0].ValueType == CE.ValueType.AsRepeat;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Root(this CE entity, out CE.FieldValue root)
        {
            if (entity.IsRepeat())
            {
                root = entity.Fields[0];
                return true;
            }
            root = default;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Repeat(this CE entity, out CE.Repeat repeat)
        {
            if (entity.IsRepeat())
            {
                repeat = entity.Fields[0].AsRepeat;
                return true;
            }
            repeat = default;
            return false;
        }

        private static readonly CE.Repeat s_emptyRepeat = new CE.Repeat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CE.Repeat Repeat(this CE entity)
            => entity.Repeat(out var repeat) ? repeat : s_emptyRepeat;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE.RepeatItem> RepeatItems(this CE entity)
            => entity.Repeat().Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE_Param> Params(this CE entity, int skip = 0)
        {
            using var itemsEnumerator = entity.RepeatItems()
                .Skip(skip).GetEnumerator();

            while (true)
            {
                if (itemsEnumerator.MoveNext() && itemsEnumerator.Current.ItemType == CE.ValueType.AsString)
                {
                    var name = itemsEnumerator.Current.AsString;
                    if (itemsEnumerator.MoveNext())
                    {
                        yield return new CE_Param(name, itemsEnumerator.Current);
                    }
                    else
                        yield break;
                }
                else
                    yield break;
            }            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE_Adapter> Adapters(this CE entity)
            => entity.RepeatItems()
                .Where(e => e.ItemType == CE.ValueType.AsEntity)
                .Select(e => new CE_Adapter(e.AsEntity));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE_Adapter> Adapters(this CE.Repeat repeat)
            => repeat.Items
                .Where(e => e.ItemType == CE.ValueType.AsEntity)
                .Select(e => new CE_Adapter(e.AsEntity));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE> Entities(this CE entity)
            => entity.RepeatItems()
                .Where(e => e.ItemType == CE.ValueType.AsEntity)
                .Select(e => e.AsEntity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<CE> Entities(this CE.Repeat repeat)
            => repeat.Items
                .Where(e => e.ItemType == CE.ValueType.AsEntity)
                .Select(e => e.AsEntity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> Strings(this CE entity)
            => entity.RepeatItems()
                .Where(e => e.ItemType == CE.ValueType.AsString)
                .Select(e => e.AsString);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> Strings(this CE.Repeat repeat)
            => repeat.Items
                .Where(e => e.ItemType == CE.ValueType.AsString)
                .Select(e => e.AsString);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> Integers(this CE entity)
            => entity.RepeatItems()
                .Where(e => e.ItemType == CE.ValueType.AsInteger)
                .Select(e => e.AsInteger);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> Integers(this CE.Repeat repeat)
            => repeat.Items
                .Where(e => e.ItemType == CE.ValueType.AsInteger)
                .Select(e => e.AsInteger);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<char> Chars(this CE.Repeat repeat)
            => repeat.Items
                .Where(e => e.ItemType == CE.ValueType.AsChar)
                .Select(e => e.AsChar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CE entity)
            => entity.Repeat().Items.Count == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this CE entity)
            => entity.Repeat().Items.Count > 0;
    }
}