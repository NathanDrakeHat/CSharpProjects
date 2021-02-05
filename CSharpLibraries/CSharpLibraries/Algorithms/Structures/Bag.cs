#nullable disable

using System.Collections;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Structures
{
    public class Bag<TItem> : IEnumerable<TItem>
    {
        private class Node
        {
            internal readonly TItem Item;
            internal readonly Node Next;

            public Node()
            {
            }

            public Node(TItem i, Node n)
            {
                Item = i;
                Next = n;
            }
        }

        private Node _first;
        public int Count { get; private set; }

        public Bag()
        {
            _first = null;
            Count = 0;
        }

        public bool IsEmpty() => _first == null;

        public void Add(TItem item)
        {
            Node old = _first;
            _first = new Node(item, old);
            Count++;
        }


        public IEnumerator<TItem> GetEnumerator()
        {
            var ptr = _first;
            while (ptr != null)
            {
                var res = ptr.Item;
                ptr = ptr.Next;
                yield return res;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}