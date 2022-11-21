using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Vtb.Trade.Grpc.Common
{

    public interface INumber2Name
    {
        INumber2Name ForEntity(int entityType);
        string Name { get; }
        string this[int fieldNumber] { get; }
    }

    public static class CE2json
    {
        public static string Entity2Json(this CE entity, INumber2Name fieldNames)
        {
            fieldNames = fieldNames.ForEntity(entity.EntityType);
            return string.Concat("{", 
            (
                fieldNames.Name,
                string.Join(",", entity.Fields.SelectMany(f => f.FieldStrings(fieldNames)))
            ).JsonClass(), "}");
        }

        public static IEnumerable<string> RepeatStrings(this CE.RepeatItem item, INumber2Name fieldNames)
        {
            switch (item.ItemType)
            {
                case CE.ValueType.AsBoolean:
                    yield return item.AsBoolean.ToString();
                    break;
                case CE.ValueType.AsChar:
                    yield return @"""" + item.AsChar.ToString() +@"""";
                    break;
                case CE.ValueType.AsDateTime:
                    yield return @"""" + item.AsDateTime.ToString() + @"""";
                    break;
                case CE.ValueType.AsDouble:
                    yield return item.AsDouble.ToString("G", CultureInfo.InvariantCulture);
                    break;
                case CE.ValueType.AsEntity:
                    if (item.AsEntity is CE entity)
                    {
                        yield return entity.Entity2Json(fieldNames);
                    }
                    break;
                case CE.ValueType.AsInteger:
                    yield return item.AsInteger.ToString();
                    break;
                case CE.ValueType.AsLong:
                    yield return item.AsLong.ToString();
                    break;
                case CE.ValueType.AsString:
                    yield return @"""" + item.AsString +@"""";
                    break;
            }
        }

        private static string JsonClass(this (string, string) value)
            => string.Concat(@"""", value.Item1, @""": {", value.Item2, "}");

        private static string JsonValue(this (string, string) value, bool quotes = false)
            => quotes ? string.Concat(@"""", value.Item1, @""":""", value.Item2, @"""")
            : string.Concat(@"""", value.Item1, @""":", value.Item2);

        public static IEnumerable<string> FieldStrings(this CE.FieldValue field, INumber2Name fieldNames)
        {
            switch (field.ValueType)
            {
                case CE.ValueType.AsBoolean:
                    yield return (fieldNames[field.Number], field.AsBoolean.ToString()).JsonValue();
                    break;
                case CE.ValueType.AsChar:
                    yield return (fieldNames[field.Number], field.AsChar.ToString()).JsonValue(true);
                    break;
                case CE.ValueType.AsDateTime:
                    yield return (fieldNames[field.Number], field.AsDateTime.ToString()).JsonValue(true);
                    break;
                case CE.ValueType.AsDouble:
                    yield return (fieldNames[field.Number], field.AsDouble.ToString("G", CultureInfo.InvariantCulture)).JsonValue();
                    break;
                case CE.ValueType.AsEntity:
                    if (field.AsEntity is CE entity)
                    {
                        yield return (fieldNames[field.Number], entity.Entity2Json(fieldNames)).JsonValue();
                    }
                    break;
                case CE.ValueType.AsInteger:
                    yield return (fieldNames[field.Number], field.AsInteger.ToString()).JsonValue();
                    break;
                case CE.ValueType.AsLong:
                    yield return (fieldNames[field.Number], field.AsLong.ToString()).JsonValue();
                    break;
                case CE.ValueType.AsRepeat:
                    if (field.AsRepeat is CE.Repeat repeat)
                    {
                        if (field.Number != CE_Adapter.iMaskFieldNo)
                        {
                            yield return (fieldNames[field.Number],
                                string.Concat("[",
                                    string.Join(",",
                                        repeat.Items.SelectMany(r => r.RepeatStrings(fieldNames))),
                                "]")
                            ).JsonValue();
                        }
                    }
                    break;

                case CE.ValueType.AsString:
                    yield return (fieldNames[field.Number], field.AsString).JsonValue(true);
                    break;
            }
        }
    }
}
