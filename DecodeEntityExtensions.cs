namespace Vtb.Trade.Grpc.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class DecodeEntityExtensions
    {
        private static IEnumerable<Type> GetTypesInNamespace<TAdapter>(this Assembly assembly, string nameSpace) where TAdapter : CE_Adapter
            => assembly.GetTypes().Where(t => 
                    string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && 
                    t.IsSubclassOf(typeof(TAdapter)));
       
        public static IDictionary<int, Func<CE, CE_Adapter>> GetDecodeDictionary<TAdapter>(this string nameSpace, IEnumerable<Assembly> assemblies) where TAdapter : CE_Adapter
        {
            var resultDictionary = new Dictionary<int, Func<CE, CE_Adapter>>();

            foreach (var definition in assemblies
                .SelectMany(a => a.GetTypesInNamespace<TAdapter>(nameSpace))
                .Select(t => new { entityType = t.EntityType(), creator = t.Creator() })
                .GroupBy(def => def.entityType)
            )
            {
                resultDictionary[definition.Key] = definition.First().creator;
            }

            return resultDictionary;
        }

        private static int EntityType(this Type type)
        {
            try
            {
                if (type.GetConstructor(new Type[] { }) is ConstructorInfo)
                {
                    return (Activator.CreateInstance(type) is CE_Adapter instance) ? instance.Entity.EntityType : 0;
                } 
            }
            catch
            {
            }

            return default;
        }

        private static Func<CE, CE_Adapter> Creator(this Type type)
        {
            if (type.GetConstructor(new[] { typeof(CE) }) is ConstructorInfo constructor)
            {
                var entity = Expression.Parameter(typeof(CE), "e");
                var getter = Expression.Lambda<Func<CE, CE_Adapter>>(
                Expression.Convert(Expression.New(constructor, entity), typeof(CE_Adapter)), entity);

                return getter.Compile();
            }

            return entity => new CE_Adapter(entity);
        }
    }
}
