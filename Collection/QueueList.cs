using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public class QueueList<T> : IEnumerable<T>
    {
        public QueueList()
        {
            Head = Last = default;
            InternalAdd = AddFirst;
        }

        private QueueList(Node head, Node last)
        {
            Head = head; Last = last;
            InternalAdd = AddNext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueueList<T> Add(T value) => InternalAdd(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> Values
        {
            get
            {
                var current = Head;
                while (current.Next != null)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueueList<T> operator +(QueueList<T> queue, T value)
            => queue.Add(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator QueueList<T>(T value)
            => new QueueList<T>().Add(value);

        private Func<T, QueueList<T>> InternalAdd;

        private QueueList<T> AddFirst(T value)
        {
            var last = new Node();
            return new QueueList<T>(new Node { Value = value, Next = last }, last);
        }

        private QueueList<T> AddNext(T value)
        {
            var last = new Node();
            Last.Value = value;
            Last.Next = last;

            return new QueueList<T>(Head, last);
        }

        private Node Head;
        private Node Last;

        class Node
        {
            public T Value;
            public Node Next;
        }
    }
}
