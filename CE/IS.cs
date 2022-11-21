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

    public abstract class IdentitySelectorContext { };
    public abstract class E : IdentitySelectorContext { }
    public struct IS<Ctx> where Ctx: IdentitySelectorContext
    {
        public static Func<string, int> GetIdentity;
        public static Func<int, string> GetText;

        public enum VType { None, Text, Identity, Field };
        private object m_text;
        private int m_identity;
        private VType m_valueType;

        public IS(CE.FieldValue field)
        {
            m_identity = default;

            if (field is CE.FieldValue)
            {
                m_text = field;
                m_valueType = VType.Field;
            }
            else
            {
                m_text = default;
                m_valueType = VType.None;
            }
        }

        public IS(string text)
            : this(default, text)
        {
        }

        public IS(int identity)
            : this(identity, default)
        {
        }

        public IS(int identity, string text)
        {
            m_identity = identity;
            m_text = text;
            m_valueType = identity != default ? VType.Identity : 
                text != default ? VType.Text : VType.None;
        }

        public bool IsNull
        {
            get 
            {
                switch ((int)m_valueType)
                {
                    case (int)VType.None:
                        return true;
                    case (int)VType.Identity:
                        return m_identity == 0;
                    case (int)VType.Text:
                        return !(m_text is string);
                    case (int)VType.Field:
                        return !(m_text is CE.FieldValue);
                    default:
                        return false;
                }
            }
        }

        public VType ValueType => m_valueType;

        public string Text
        {
            get 
            {
                switch ((int)m_valueType)
                {
                    case (int)VType.Text:
                        return (string)m_text;
                    case (int)VType.Identity:
                        return (GetText?.Invoke(m_identity)) ?? default;
                    case (int)VType.Field:
                        var f = Field;
                        return  f != null && f.ValueType == CE.ValueType.AsString ?
                            f.AsString : (GetText?.Invoke(f.AsInteger) ?? default);
                    default:
                        return default;
                }
            }
            set
            {
                m_text = value;
                m_valueType = VType.Text;
            }
        }

        public int Identity
        {
            get
            {
                switch ((int)m_valueType)
                {
                    case (int)VType.Identity:
                        return m_identity;
                    case (int)VType.Text:
                        return (GetIdentity?.Invoke((string)m_text)) ?? default;
                    case (int)VType.Field:
                        var f = Field;
                        return f.ValueType == CE.ValueType.AsInteger ?
                            f.AsInteger : (GetIdentity?.Invoke(f.AsString) ?? default);
                    default:
                        return default;
                }
            }
            set
            {
                m_identity = value;
                m_valueType = VType.Identity;
            }
        }

        public IS<Ctx> ConvertToText()
        {
            switch ((int)m_valueType)
            {
                case (int)VType.Identity:
                    m_text = GetText(m_identity);
                    m_valueType = VType.Text;
                    break;
                case (int)VType.Field:
                    if (m_text is CE.FieldValue field)
                    {
                        if (field.ValueType == CE.ValueType.AsInteger)
                        {
                            field.AsString = GetText(field.AsInteger);
                        }
                    }
                    break;
            }
            return this;
        }

        public IS<Ctx> ConvertToIdentity()
        {
            if (GetIdentity != null)
            {
                switch ((int)m_valueType)
                {
                    case (int)VType.Text:
                        m_identity = GetIdentity((string)m_text);
                        m_valueType = VType.Identity;
                        break;
                    case (int)VType.Field:
                        if (m_text is CE.FieldValue field)
                        {
                            if (field.ValueType == CE.ValueType.AsString)
                            {
                                field.AsInteger = GetIdentity(field.AsString);
                            }
                        }
                        break;
                }
            }
            return this;
        }

        public CE.FieldValue Field { get => m_text as CE.FieldValue;  }

        public static implicit operator IS<Ctx>(CE.FieldValue value)
            => new IS<Ctx>(value);

        public static implicit operator CE.FieldValue (IS<Ctx> value)
            => value.m_valueType == VType.Field ? value.m_text as CE.FieldValue : default;

        public static implicit operator IS<Ctx>(string value) => new IS<Ctx>(value);
        public static implicit operator IS<Ctx>(int value) => new IS<Ctx>(value);

        public static implicit operator string (IS<Ctx> value) => value.Text;        
        public static implicit operator int (IS<Ctx> value) => value.Identity;
    }

    public static class IdentityFieldExtension
    {
        public static CE.FieldValue AsField<Ctx>(this IS<Ctx> value, int fieldNo) where Ctx : IdentitySelectorContext
        {
            return (int)value.ValueType switch
            {
                (int)IS<Ctx>.VType.Text => CE.FieldValue.Create(fieldNo, value.Text),
                (int)IS<Ctx>.VType.Identity => CE.FieldValue.Create(fieldNo, value.Identity),
                (int)IS<Ctx>.VType.Field => value.Field.Clone(fieldNo),
                (int)IS<Ctx>.VType.None => default,
                _ => throw new Exception("no coverstion from other type"),
            };
        }

        public static IEnumerable<PropertyInfo> IdentityFields<TAdapter, Ctx>() 
            where TAdapter : CE_Adapter
            where Ctx : IdentitySelectorContext
            => IdentityFields(typeof(TAdapter), typeof(IS<Ctx>) );

        public static IEnumerable<PropertyInfo> IdentityFields(this Type type, Type @is)
        {
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.PropertyType == @is)
                    yield return propertyInfo;
            }
        }

        public static Action<TInstance, Func<IS<Ctx>, IS<Ctx>>> Converter<TInstance, Ctx>()
            where TInstance : CE_Adapter
            where Ctx : IdentitySelectorContext
        {
            var instance = Expression.Parameter(typeof(TInstance), "instance");
            var func = Expression.Parameter(typeof(Func<IS<Ctx>, IS<Ctx>>), "func");

            var methodBody = Expression.Block(
                IdentityFields<TInstance, Ctx>().Select(pi =>
                    Expression.IfThen(Expression.Not(
                        Expression.Property(Expression.Property(instance, pi), "IsNull")),
                        Expression.Assign(Expression.Property(instance, pi),
                            Expression.Invoke(func, Expression.Property(instance, pi)))
                    )
                )
            );

            var expr = Expression.Lambda<Action<TInstance, Func<IS<Ctx>, IS<Ctx>>>>
                (methodBody, instance, func);

            return expr.Compile();
        }

        public static Action<TInstance> Converter<TInstance, Ctx>(this string conversion)
            where TInstance : CE_Adapter
            where Ctx : IdentitySelectorContext
        {
            if (!string.IsNullOrEmpty(conversion))
            {
                var converter = Converter<TInstance, Ctx>();
                Func<IS<Ctx>, IS<Ctx>> convertFunction = conversion switch
                {
                    "id" => (IS<Ctx> id) => id.ConvertToIdentity(),
                    "text" => (IS<Ctx> id) => id.ConvertToText(),
                    _ => id => id
                };

                return instance => converter(instance, convertFunction);
            }

            return null;
        }
    }

    public static class IdentityExtenstions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetIdentityText(this int identity) => IS<E>.GetText(identity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIdentity(this string text) => IS<E>.GetIdentity(text);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetIdentityText(this CE.FieldValue field)
            => (int)field.ValueType switch
            {
                (int)CE.ValueType.AsString => field.AsString,
                (int)CE.ValueType.AsInteger => field.AsInteger.GetIdentityText(),
                (int)CE.ValueType.AsLong => ((int)field.AsLong).GetIdentityText(),
                _ => throw new Exception($"Неправильный тип поля {field.ValueType} для получения идентификатора"),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIdentity(this CE.FieldValue field)
            => (int)field.ValueType switch
            {
                (int)CE.ValueType.AsString => field.AsString.GetIdentity(),
                (int)CE.ValueType.AsInteger => field.AsInteger,
                (int)CE.ValueType.AsLong => (int)field.AsLong,
                _ => throw new Exception($"Неправильный тип поля {field.ValueType} для получения идентификатора"),
            };
    }
}
