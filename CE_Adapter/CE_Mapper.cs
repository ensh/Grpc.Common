using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Vtb.Trade.Grpc.Common.Mapper
{
    public static class CE_Mapper<TAdapter, TInstance> where TInstance : new() where TAdapter : CE_Adapter
    {
        static CE_Mapper()
        {
            s_instanceGetter = InstanceGetter();
            s_adapterGetter = AdapterGetter();
            s_instanceUpdater = InstanceUpdater();
        }

        private static Func<TAdapter, TInstance> s_instanceGetter;
        private static Action<TAdapter, TInstance> s_instanceUpdater;
        private static Func<TInstance, TAdapter> s_adapterGetter;

        public static TInstance GetInstance(TAdapter adapter) => s_instanceGetter(adapter);
        public static void UpdateInstance(TAdapter adapter, TInstance instance) => s_instanceUpdater(adapter, instance);
        public static TAdapter GetAdapter(TInstance result) => s_adapterGetter(result);

        public static Func<TAdapter, TInstance> InstanceGetter()
        {
            const BindingFlags propertyBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var propertyMatch = typeof(TAdapter).GetProperties(propertyBindingFlags | BindingFlags.GetProperty)
                .Join(typeof(TInstance).GetProperties(propertyBindingFlags | BindingFlags.SetProperty), p => p.Name, p => p.Name,
                (src, dest) => new { src, dest });

            var adapterParameter = Expression.Parameter(typeof(TAdapter), "a");
            var propertyBinding = propertyMatch.Select(pm => Expression.Bind(pm.dest,
                Expression.Convert(Expression.Property(adapterParameter, pm.src), pm.dest.PropertyType)));

            var init = Expression.MemberInit(Expression.New(typeof(TInstance)), propertyBinding);

            var funcExpression = Expression.Lambda<Func<TAdapter, TInstance>>(init, adapterParameter);

            return funcExpression.Compile();
        }

        public static Func<TInstance, TAdapter> AdapterGetter()
        {
            const BindingFlags propertyBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var propertyMatch = typeof(TInstance).GetProperties(propertyBindingFlags | BindingFlags.GetProperty)
                .Join(typeof(TAdapter).GetProperties(propertyBindingFlags | BindingFlags.SetProperty), p => p.Name, p => p.Name,
                (src, dest) => new { src, dest });

            var instanceParameter = Expression.Parameter(typeof(TInstance), "i");
            var propertyBinding = propertyMatch.Select(pm => Expression.Bind(pm.dest,
                Expression.Convert(Expression.Property(instanceParameter, pm.src), pm.dest.PropertyType)));

            var init = Expression.MemberInit(Expression.New(typeof(TAdapter)), propertyBinding);

            var funcExpression = Expression.Lambda<Func<TInstance, TAdapter>>(init, instanceParameter);

            return funcExpression.Compile();
        }

        public static Action<TAdapter, TInstance> InstanceUpdater()
        {
            const BindingFlags propertyBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

            var metaInfo = Utils.MetaInfoProperty<TAdapter>().GetValue(null) as (int index, PropertyInfo prop)[];

            if (metaInfo is (int index, PropertyInfo prop)[])
            {
                var propertyMatch = typeof(TAdapter).GetProperties(propertyBindingFlags | BindingFlags.GetProperty)
                    .Join(typeof(TInstance).GetProperties(propertyBindingFlags | BindingFlags.SetProperty), p => p.Name, p => p.Name,
                        (src, dest) => new { src, dest })
                    .Join(metaInfo, p => p.src.Name, p => p.prop.Name, (mt, mi) => new { idx = mi.index, mt.src, mt.dest });

                var adapterParameter = Expression.Parameter(typeof(TAdapter), "a");
                var instanceParameter = Expression.Parameter(typeof(TInstance), "i");

                var caseVariants = propertyMatch.Select(pm =>
                    Expression.SwitchCase(
                        Expression.Block(typeof(void),
                            Expression.Assign(
                                Expression.Property(instanceParameter, pm.dest),
                                Expression.Property(adapterParameter, pm.src))
                        ),
                        Expression.Constant(pm.idx, typeof(int))
                    )).ToArray();

                var breakLabel = Expression.Label("break");
                var filledEnumerator = Expression.Variable(typeof(IEnumerator<int>), "filled");
                var initFilledEnumerator = Expression.Assign(filledEnumerator, adapterParameter.FilledFields<TAdapter>().Enumerator());

                var tryStatment = Expression.TryFinally(
                Expression.Loop(
                      Expression.IfThenElse(filledEnumerator.MoveNext()
                          , Expression.Switch(filledEnumerator.Current(), caseVariants)
                          , Expression.Break(breakLabel)
                      )
                      , breakLabel
                    ),
                    filledEnumerator.Dispose()
                );

                var block = Expression.Block(new[] { filledEnumerator }, 
                    new Expression[] 
                    { 
                        initFilledEnumerator, 
                        tryStatment
                    });

                var actionExpression = Expression.Lambda<Action<TAdapter, TInstance>>(block, adapterParameter, instanceParameter);

                return actionExpression.Compile();
            }
            return null;
        }
    }

    internal static class Utils
    {
        public static PropertyInfo MetaInfoProperty<TAdapter>() => typeof(TAdapter).GetProperty("MetaInfo"); 
        public static PropertyInfo FilledFieldsProperty<TAdapter>() => typeof(TAdapter).GetProperty("FilledFields");
        public static PropertyInfo FieldsProperty { get => typeof(CE).GetProperty("Fields"); }
        public static MethodInfo GetEnumeratorMethod { get => typeof(IEnumerable<int>).GetMethod("GetEnumerator"); }
        public static MethodInfo MoveNextMethod { get => typeof(IEnumerator).GetMethod("MoveNext"); }
        public static PropertyInfo CurrentProperty { get => typeof(IEnumerator<int>).GetProperty("Current"); }
        public static MethodInfo DisposeMethod { get => typeof(IDisposable).GetMethod("Dispose"); }

        public static Expression Fields(this Expression entity) => Expression.Property(entity, FieldsProperty);
        public static Expression Enumerator(this Expression enumerable) => Expression.Call(enumerable, GetEnumeratorMethod);
        public static Expression Dispose(this Expression enumerator) => Expression.Call(Expression.Convert(enumerator, typeof(IDisposable)), DisposeMethod);
        public static Expression MoveNext(this Expression enumerable) => Expression.Call(Expression.Convert(enumerable, typeof(IEnumerator)), MoveNextMethod);
        public static Expression Current(this Expression enumerator) => Expression.Property(enumerator, CurrentProperty);
        public static Expression MetaInfo<TAdapter>() => Expression.Property(null, MetaInfoProperty<TAdapter>());
        public static Expression FilledFields<TAdapter>(this Expression adapter) => Expression.Property(adapter, FilledFieldsProperty<TAdapter>());

    }
}
