#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static CSharpLibraries.Extensions.Extension;

[assembly: InternalsVisibleTo("CSarpLibrariesTest")]
namespace CSharpLibraries.Algorithms.Structures
{
    
    // ReSharper disable once UnusedType.Global
    public sealed class FibonacciHeap<TKey, TValue> where TValue : notnull
    {
        #region InnerClass
        internal sealed class Node
        {
            internal TKey Key;
            internal readonly TValue Value;
            internal Node Parent;
            internal Node ChildList; // int linked, circular list
            internal Node Left;
            internal Node Right;
            internal int Degree; // number of children
            internal bool Mark; // whether the node had lost a child when it be made another node's child

            internal Node(TKey key, TValue val)
            {
                Key = key;
                Value = val;
                Left = this;
                Right = this;
            }

            public override string ToString() => $"key:{Key}";
        }

        #endregion


        internal Node RootList;
        public int Count { get; internal set; } // number of nodes
        private readonly Dictionary<TValue, Node> _valueToNodeMap = new();
        private readonly Func<TKey, TKey, int> _keyComparer;
        private int UpperBound => (int) (Math.Log(Count) / Math.Log(2));

        public FibonacciHeap(Func<TKey, TKey, int> keyComparer)
        {
            _keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
        }

        private void Insert(Node x)
        {
            Count++;
            _valueToNodeMap[x.Value] = x;
            if (RootList == null) RootList = x;
            else
            {
                AddNodeToList(x, RootList); // add x to root list
                if (_keyComparer(x.Key, RootList.Key) < 0) RootList = x;
            }
        }

        public void Insert(TKey key, TValue val) => Insert(new Node(key, val));


        public TValue ExtractMin()
        {
            if (Count <= 0) throw new InvalidOperationException("Null heap.");
            var z = RootList;
            var childList = z!.ChildList; // add root_list's children list to root list
            var p = childList;
            if (p != null)
            {
                do
                {
                    var t = p.Right;
                    p.Parent = null;
                    AddNodeToList(p, RootList!);
                    p = t;
                } while (!ReferenceEquals(p, childList));
            }

            RootList = z.Right;
            RemoveNodeFromList(z);
            if (ReferenceEquals(z, RootList)) RootList = null;
            else Consolidate();
            Count--;
            _valueToNodeMap.Remove(z.Value);
            return z.Value;
        }

        private void Consolidate()
        {
            var array = new Node[UpperBound + 1];
            var w = RootList;
            if (w == null) return;

            var dict = new HashSet<Node>();
            do
            {
                // for w in root list start
                var x = w; // x current node
                var next = x.Right;
                int d = x.Degree;
                while (array[d] != null)
                {
                    var y = array[d]; // y stored node
                    if (_keyComparer(x.Key, y!.Key) > 0)
                    {
                        // exchange pointer
                        var t = x;
                        x = y;
                        y = t;
                    }

                    LinkTo(y, x);
                    array[d] = null;
                    d++;
                }

                array[d] = x;
                // for w in root list end
                dict.Add(w);
                w = next;
            } while (!dict.Contains(w));

            RootList = null;
            for (int i = 0; i <= UpperBound; i++)
            {
                var t = array[i];
                if (t != null)
                {
                    if (RootList == null)
                    {
                        t.Right = t;
                        t.Left = t;
                        RootList = t;
                    }
                    else
                    {
                        AddNodeToList(t, RootList);
                        if (_keyComparer(t.Key, RootList.Key) < 0) RootList = t;
                    }
                }
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public FibonacciHeap<TKey, TValue> Union(FibonacciHeap<TKey, TValue> f2)
        {
            f2.RequireNotNullArg(nameof(f2));
            var res = new FibonacciHeap<TKey, TValue>(_keyComparer) {RootList = RootList};
            var f1Right = RootList!.Right; // concatenate two root list
            var f2Left = f2.RootList!.Left;
            RootList.Right = f2.RootList;
            f2.RootList.Left = RootList;
            f1Right.Left = f2Left;
            f2Left.Right = f1Right;

            if (_keyComparer(f2.RootList.Key, RootList.Key) < 0) res.RootList = f2.RootList;
            res.Count = Count + f2.Count;
            RootList = null;
            return res;
        }


        public void DecreaseKey(TValue val, TKey newKey)
        {
            var x = _valueToNodeMap[val];
            newKey.RequireNotNullArg(nameof(newKey));
            if (_keyComparer(newKey, x.Key) > 0)
                throw new InvalidOperationException(
                    "New key should smaller than original key");
            x.Key = newKey;
            var y = x.Parent;
            if (y != null)
            {
                if (_keyComparer(x.Key, y.Key) < 0)
                {
                    Cut(x, y);
                    CascadingCut(y);
                }
            }

            if (_keyComparer(x.Key, RootList!.Key) <= 0) RootList = x;
        }

        internal void DecreaseKey(Node x, TKey newKey)
        {
            // move x to root list
            // set Parent mark true if Parent mark is false
            // else successively move true mark parents to root list
            x.Key = newKey;
            var y = x.Parent;
            if (y != null)
            {
                if (_keyComparer(x.Key, y.Key) < 0)
                {
                    Cut(x, y);
                    CascadingCut(y);
                }
            }

            if (_keyComparer(x.Key, RootList!.Key) <= 0) RootList = x;
        }

        private void Cut(Node a, Node b)
        {
            RemoveNodeFromList(a);
            b.Degree--;
            AddNodeToList(a, RootList!);
            a.Mark = false;
        }

        private void CascadingCut(Node y)
        {
            var z = y.Parent;
            if (z != null)
            {
                if (!y.Mark) y.Mark = true;
                else
                {
                    Cut(y, z);
                    CascadingCut(z);
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void Delete(Node x)
        {
            DecreaseKey(x, RootList!.Key);
            ExtractMin();
        }

        public bool Contains(TValue x) => _valueToNodeMap.ContainsKey(x);


        private static void AddNodeToList(Node x, Node list)
        {
            x.Parent = list.Parent;
            var listLeft = list.Left;
            list.Left = x;
            x.Right = list;
            listLeft.Right = x;
            x.Left = listLeft;
        }

        private static void RemoveNodeFromList(Node z)
        {
            var zRight = z.Right;
            var zLeft = z.Left;
            if (z.Parent != null)
            {
                if (ReferenceEquals(z.Parent.ChildList, z))
                {
                    if (!ReferenceEquals(z.Right, z))
                    {
                        z.Parent.ChildList = z.Right;
                        z.Right.Parent = z.Parent;
                    }
                    else
                    {
                        z.Parent.ChildList = null;
                        z.Right = z;
                        z.Left = z;
                        z.Parent = null;
                        return;
                    }
                }
            }

            zRight.Left = zLeft;
            zLeft.Right = zRight;

            z.Right = z;
            z.Left = z;
            z.Parent = null;
        }

        private static void LinkTo(Node l, Node m)
        {
            // larger, minor
            RemoveNodeFromList(l);
            m.Degree++;
            if (m.ChildList == null)
            {
                m.ChildList = l;
                l.Parent = m;
            }
            else AddNodeToList(l, m.ChildList);

            l.Mark = false;
        }
    }
}