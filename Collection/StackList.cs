using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public class StackList<T> : IEnumerable<T>
    {
        public static StackList<T> Empty = new StackList<T>(default);

        public StackList(T value) => this.value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StackList<T> operator +(StackList<T> source, T value)
            => source.Add(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StackList<T>(T value)
            => Empty.Add(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StackList<T> Add(T value) => new StackList<T>(value) { Next = this };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> Values
        {
            get
            {
                var values = this;
                do
                {
                    yield return values.value;
                    values = values.Next;
                } while (values is StackList<T> && !ReferenceEquals(values, Empty));
            }
        }

        private readonly T value;
        private StackList<T> Next;
    }
}