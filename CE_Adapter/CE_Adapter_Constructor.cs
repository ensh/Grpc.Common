using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vtb.Trade.Grpc.Common
{
    public static class CE_Adapter_Constructor
    {
        public static string ConstName(this string name) => string.Join("", name
                .Select((c, i) => char.IsUpper(c) && i > 0 ? "_" + c : char.ToUpper(c).ToString())
                .Prepend("i"));

        public static IEnumerable<string> Constants(this string[] names)
        => Enumerable.Range(1, names.Length - 1).Select(i =>
           {
                var name = names[i].Split(",").Take(2).Last();
                var nameConst = name.ConstName();
                return $"public const int {nameConst} = {i - 1};";
           });

        public static string AsType(this string @type)
        {
            switch (@type)
            {
                case "bool": return "?.AsBoolean ?? default";
                case "char": return "?.AsChar ?? default";
                case "int": return "?.AsInteger ?? default";
                case "long": return "?.AsLong ?? default";
                case "double": return "?.AsDouble ?? default";
                case "dt": return "?.AsDateTime ?? default";
                case "string": return "?.AsString ?? default";
                default: return "";
            }
        }

        public static IEnumerable<string> Properties(this string[] names)
        {
            const int @type = 0;
            const int @name = 1;
            foreach (var fieldDef in Enumerable.Range(1, names.Length - 1).Select(i => names[i]))
            {
                var def = fieldDef.Split(",").Take(2).ToArray();
                def = (def.Length < 2) ? new[] { "string", def[0] } : def;

                var nameConst = def[@name].ConstName();

                yield return $"public {def[@type]} {def[name]}{Environment.NewLine}{{";
                yield return $"get => this[{nameConst}]{def[type].AsType()};";
                yield return $"set => this[{nameConst}] = CE.FieldValue.Create({nameConst} + 1, value);";
                yield return $"}}{Environment.NewLine}";
            }
        }

        public static string GenerateAdapterText(this string definition)
        {
            var def = definition.Split(";");

            var classDefinition = string.Concat(
                $"public class {def[0]}: CE_Adapter{Environment.NewLine}{{{Environment.NewLine}",
                $"public const int {def[0]}_EntityType = 1;{Environment.NewLine}",
                $"public {def[0]}_Adapter() : base({def[0]}_EntityType) {{ ((CE)this).Fields.Capacity = {def.Length - 1}; }}{Environment.NewLine}",
                $"public {def[0]}_Adapter(CE data) : base(data.EntityType == {def[0]}_EntityType ? data : new CE {{ EntityType = {def[0]}_EntityType }}) {{ }}{Environment.NewLine}",
                $"public static implicit operator {def[0]}_Adapter(CE entity) => new {def[0]}_Adapter(entity);{Environment.NewLine}"
            );

            var constantDefinition = string.Join(Environment.NewLine, def.Constants());
            var propertyDefinition = string.Join(Environment.NewLine, def.Properties());

            return string.Join(Environment.NewLine,
                classDefinition, constantDefinition, propertyDefinition, "}", "");
        }
    }
}
