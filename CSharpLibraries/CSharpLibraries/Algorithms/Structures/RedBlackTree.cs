#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CSharpLibraries.Extensions.Extension;

[assembly: InternalsVisibleTo("CSharpLibrariesTest")]

namespace CSharpLibraries.Algorithms.Structures
{
    public sealed class RedBlackTree<TKey, TValue> : IEnumerable<Tuple<TKey, TValue>>
    {
        #region InnerClass

        internal sealed class Node
        {
            // ReSharper disable once FieldCanBeMadeReadOnly.Global
            internal TKey Key;
            internal TValue Value;
            internal NodeColor NodeColor;
            internal Node Parent;
            internal Node Left;
            internal Node Right;

            internal Node(NodeColor nodeColor) => NodeColor = nodeColor;

            internal Node(TKey key, TValue val)
            {
                NodeColor = NodeColor.Red;
                Key = key;
                Value = val;
            }


            internal bool IsRed() => NodeColor == NodeColor.Red;
            internal bool IsBlack() => NodeColor == NodeColor.Black;

            internal void SetBlack() => NodeColor = NodeColor.Black;
            internal void SetRed() => NodeColor = NodeColor.Red;
        }

        internal enum NodeColor
        {
            Red,
            Black
        }

        #endregion


        public int Count { get; private set; }
        internal Node Root;
        private readonly Func<TKey, TKey, int> _kComparator;
        private readonly Node _sentinel = new Node(NodeColor.Black); // sentinel: denote leaf and parent of root


        /// <summary>
        /// 
        /// </summary>
        /// <param name="kComparator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedBlackTree(Func<TKey, TKey, int> kComparator)
        {
            _kComparator = kComparator ?? throw new ArgumentNullException(nameof(kComparator));
            Root = _sentinel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low">include</param>
        /// <param name="high">include</param>
        /// <returns></returns>
        public List<TKey> KeyRangeSearch(TKey low, TKey high)
        {
            low.RequireNotNullArg(nameof(low));
            var res = new List<TKey>();
            if (!NotNullTree()) return res;
            KeyRangeSearch(Root, low, high, res);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Minimum key</returns>
        /// <exception cref="InvalidOperationException">Null tree</exception>
        public TKey MinKey()
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            return GetMinimum(Root).Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TValue ValueOfMinKey()
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            return GetMinimum(Root).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TKey MaxKey()
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            return GetMaximum(Root).Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TValue ValueOfMaxKey()
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            return GetMaximum(Root).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int GetHeight()
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            return GetHeight(Root, 0) - 1;
        }

        public IEnumerator<Tuple<TKey, TValue>> GetEnumerator()
        {
            var n = Root;
            if (n == null || n == _sentinel)
            {
                yield break;
            }

            var stack = new Stack<Node>(); // stack is important to understand this algorithm
            bool poppedBefore = false;
            while (n != null)
            {
                if (n.Left != _sentinel && !poppedBefore) // if popped before, walk to right
                {
                    stack.Push(n);
                    n = n.Left;
                }
                else
                {
                    var t = n;
                    if (n.Right != _sentinel)
                    {
                        n = n.Right;
                        poppedBefore = false;
                    }
                    else
                    {
                        if (stack.Count != 0)
                        {
                            n = stack.Pop();
                            poppedBefore = true;
                        }
                        else n = null;
                    }

                    yield return new Tuple<TKey, TValue>(t.Key, t.Value);
                }
            }
        }

        public IEnumerable<Tuple<TKey, TValue>> GetReverseEnumerator()
        {
            var n = Root;
            if (n == null || n == _sentinel)
            {
                yield break;
            }

            var stack = new Stack<Node>();
            bool popped = false;
            while (n != null)
            {
                if (n.Right != _sentinel && !popped)
                {
                    stack.Push(n);
                    n = n.Right;
                }
                else
                {
                    var t = n;
                    if (n.Left != _sentinel)
                    {
                        n = n.Left;
                        popped = false;
                    }
                    else
                    {
                        if (stack.Count != 0)
                        {
                            n = stack.Pop();
                            popped = true;
                        }
                        else n = null;
                    }

                    yield return new Tuple<TKey, TValue>(t.Key, t.Value);
                }
            }
        }

        /// <summary>
        /// Insert a node with key and val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Insert(TKey key, TValue val)
        {
            key.RequireNotNullArg(nameof(key));
            var n = Search(Root, key);
            if (n != null)
            {
                n.Value = val;
            }
            else
            {
                Insert(new Node(key, val));
            }

            Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TValue Search(TKey key)
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            key.RequireNotNullArg(nameof(key));
            var res = Search(Root, key);
            if (res == null) throw new InvalidOperationException("No suck key.");
            else return res.Value;
        }

        public bool Contains(TKey key) => Search(Root, key) != null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Set(TKey key, TValue newValue)
        {
            key.RequireNotNullArg(nameof(key));
            var node = Search(Root, key);
            if (node == null) throw new InvalidOperationException("No such key.");
            node.Value = newValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Delete(TKey key)
        {
            if (!NotNullTree()) throw new InvalidOperationException("Null tree.");
            key.RequireNotNullArg(nameof(key));
            var node = Search(Root, key);
            if (node == null) return;
            Delete(node);
            Count--;
        }

        private void KeyRangeSearch(Node n, TKey low, TKey high, List<TKey> l)
        {
            if (n == _sentinel) return;
            if (_kComparator(n.Key, low) >= 0 && _kComparator(n.Key, high) <= 0)
            {
                l.Add(n.Key);
            }

            if (_kComparator(n.Key, low) > 0)
            {
                KeyRangeSearch(n.Left, low, high, l);
            }

            if (_kComparator(n.Key, high) < 0)
            {
                KeyRangeSearch(n.Right, low, high, l);
            }
        }

        private bool NotNullTree() => Root != _sentinel;


        private void ResetRoot(Node r)
        {
            Root = r;
            Root.Parent = _sentinel;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int GetHeight(Node n, int height)
        {
            if (n != _sentinel)
            {
                int leftMax = GetHeight(n.Left, height + 1);
                int rightMax = GetHeight(n.Right, height + 1);
                return Math.Max(leftMax, rightMax);
            }

            return height;
        }

        private void Insert(Node n)
        {
            if (!NotNullTree())
            {
                ResetRoot(n);
                Root.Right = _sentinel;
                Root.Left = _sentinel;
            }
            else
            {
                var store = _sentinel;
                var ptr = Root;
                while (ptr != _sentinel)
                {
                    store = ptr;
                    ptr = _kComparator(n.Key, ptr.Key) < 0 ? ptr.Left : ptr.Right;
                }

                n.Parent = store;
                if (_kComparator(n.Key, store.Key) < 0) store.Left = n;
                else store.Right = n;
                n.Left = _sentinel;
                n.Right = _sentinel;
            }

            InsertFixUp(n);
        }

        private void InsertFixUp(Node ptr)
        {
            while (ptr.Parent!.IsRed())
            {
                if (ptr.Parent == ptr.Parent.Parent.Left)
                {
                    var right = ptr.Parent.Parent.Right;
                    if (right.IsRed())
                    {
                        // case1: sibling is red
                        ptr.Parent.SetBlack();
                        right.SetBlack();
                        ptr.Parent.Parent.SetRed();
                        ptr = ptr.Parent.Parent;
                        continue;
                    }
                    else if (ptr == ptr.Parent.Right)
                    {
                        //case 2 convert to case 3
                        ptr = ptr.Parent;
                        LeftRotate(ptr);
                    }

                    ptr.Parent.SetBlack(); // case3
                    ptr.Parent.Parent.SetRed();
                    RightRotate(ptr.Parent.Parent); // ptr.getParent will be black and then break
                    ptr = ptr.Parent;
                }
                else
                {
                    var left = ptr.Parent.Parent.Left;
                    if (left.IsRed())
                    {
                        ptr.Parent.SetBlack();
                        left.SetBlack();
                        ptr.Parent.Parent.SetRed();
                        ptr = ptr.Parent.Parent;
                        continue;
                    }
                    else if (ptr == ptr.Parent.Left)
                    {
                        ptr = ptr.Parent;
                        RightRotate(ptr);
                    }

                    ptr.Parent.SetBlack();
                    ptr.Parent.Parent.SetRed();
                    LeftRotate(ptr.Parent.Parent);
                    ptr = ptr.Parent;
                }
            }

            Root.SetBlack();
        }

        private void Delete(Node target)
        {
            var ptr = target;
            var ptrColor = ptr.NodeColor;
            Node fixUp;
            if (ptr.Left == _sentinel)
            {
                fixUp = target.Right;
                Transplant(target, fixUp);
            }
            else if (ptr.Right == _sentinel)
            {
                fixUp = target.Left;
                Transplant(target, fixUp);
            }
            else
            {
                ptr = GetSuccessor(target);
                ptrColor = ptr.NodeColor;
                fixUp = ptr.Right;
                // in case of sentinel refer to target
                if (ptr.Parent == target) fixUp.Parent = ptr;
                else
                {
                    Transplant(ptr, ptr.Right);
                    ptr.Right = target.Right;
                    target.Right.Parent = ptr;
                }

                Transplant(target, ptr);
                ptr.Left = target.Left;
                target.Left.Parent = ptr;
                ptr.NodeColor = target.NodeColor;
            }

            if (ptrColor == NodeColor.Black) DeleteFixUp(fixUp);
        }

        private void DeleteFixUp(Node fixUp)
        {
            while (fixUp != Root && fixUp.IsBlack())
            {
                Node sibling;
                if (fixUp == fixUp.Parent.Left)
                {
                    sibling = fixUp.Parent.Right;
                    if (sibling.IsRed())
                    {
                        // case1:sibling is black, convert to case 2, 3 or 4
                        sibling.SetBlack(); // , which denote that sibling is black
                        fixUp.Parent.SetRed();
                        LeftRotate(fixUp.Parent);
                        sibling = fixUp.Parent.Right;
                    }

                    if (sibling.Left.IsBlack() && sibling.Right.IsBlack())
                    {
                        // case2: sibling children is black
                        sibling.SetRed();
                        fixUp = fixUp.Parent;
                        continue;
                    }
                    else if (sibling.Right.IsBlack())
                    {
                        // case3: sibling left red, right black. convert case4
                        sibling.Left.SetBlack();
                        sibling.SetRed();
                        RightRotate(sibling);
                        sibling = fixUp.Parent.Right;
                    }

                    sibling.NodeColor = fixUp.Parent.NodeColor; // case4: sibling right red
                    fixUp.Parent.SetBlack();
                    sibling.Right.SetBlack();
                    LeftRotate(fixUp.Parent);
                }
                else
                {
                    sibling = fixUp.Parent.Left;
                    if (sibling.IsRed())
                    {
                        sibling.SetBlack();
                        fixUp.Parent.SetRed();
                        RightRotate(fixUp.Parent);
                        sibling = fixUp.Parent.Left;
                    }

                    if (sibling.Left.IsBlack() && sibling.Right.IsBlack())
                    {
                        sibling.SetRed();
                        fixUp = fixUp.Parent;
                        continue;
                    }
                    else if (sibling.Left.IsBlack())
                    {
                        sibling.Right.SetBlack();
                        sibling.SetRed();
                        LeftRotate(sibling);
                        sibling = fixUp.Parent.Left;
                    }

                    sibling.NodeColor = fixUp.Parent.NodeColor;
                    fixUp.Parent.SetBlack();
                    sibling.Left.SetBlack();
                    RightRotate(fixUp.Parent);
                }

                fixUp = Root;
            }

            fixUp.SetBlack();
        }

        private void Transplant(Node a, Node b)
        {
            if (a.Parent == _sentinel) ResetRoot(b);
            else if (a.Parent.Right == a)
            {
                a.Parent.Right = b;
                b.Parent = a.Parent; // permissible if b is sentinel
            }
            else
            {
                a.Parent.Left = b;
                b.Parent = a.Parent;
            }
        }

        private Node Search(Node n, TKey key)
        {
            while (true)
            {
                if (n == _sentinel) return null;
                if (_kComparator(n.Key, key) == 0)
                    return n;
                else if (n.Left != _sentinel && _kComparator(n.Key, key) > 0)
                {
                    n = n.Left;
                    continue;
                }
                else if (n.Right != _sentinel && _kComparator(n.Key, key) < 0)
                {
                    n = n.Right;
                    continue;
                }

                return null;
            }
        }

        private void LeftRotate(Node leftNode)
        {
            var rightNode = leftNode.Right;
            //exchange
            leftNode.Right = rightNode.Left;
            // remember to double link
            if (rightNode.Left != _sentinel) rightNode.Left.Parent = leftNode;
            //exchange
            rightNode.Parent = leftNode.Parent; // double link right_node to left_node parent
            if (leftNode.Parent == _sentinel) ResetRoot(rightNode);
            else if (leftNode.Parent.Left == leftNode) leftNode.Parent.Left = rightNode;
            else leftNode.Parent.Right = rightNode;
            //exchange
            rightNode.Left = leftNode;
            leftNode.Parent = rightNode;
        }

        private void RightRotate(Node rightNode)
        {
            // mirror of leftRotate
            var leftNode = rightNode.Left;
            rightNode.Left = leftNode.Right;
            if (leftNode.Right != _sentinel) leftNode.Right.Parent = rightNode;
            leftNode.Parent = rightNode.Parent;
            if (rightNode.Parent == _sentinel) ResetRoot(leftNode);
            else if (rightNode.Parent.Right == rightNode) rightNode.Parent.Right = leftNode;
            else rightNode.Parent.Left = leftNode;
            leftNode.Right = rightNode;
            rightNode.Parent = leftNode;
        }

        private Node GetMinimum(Node current)
        {
            Node target = null;
            var ptr = current;
            while (ptr != _sentinel)
            {
                target = ptr;
                ptr = ptr.Left;
            }

            return target;
        }

        private Node GetMaximum(Node current)
        {
            Node target = null;
            var ptr = current;
            while (ptr != _sentinel)
            {
                target = ptr;
                ptr = ptr.Right;
            }

            return target;
        }

        private Node GetSuccessor(Node current)
        {
            if (current.Right != _sentinel) return GetMinimum(current.Right);
            else
            {
                var target = current.Parent;
                var targetRight = current;
                while (target != _sentinel && target!.Right == targetRight)
                {
                    targetRight = target;
                    target = target.Parent;
                }

                return target;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private Node GetPredecessor(Node current)
        {
            if (current.Left != _sentinel) return GetMaximum(current.Left);
            else
            {
                var target = current.Parent;
                var targetLeft = current;
                while (target != _sentinel && target!.Left == targetLeft)
                {
                    targetLeft = target;
                    target = target.Parent;
                }

                return target;
            }
        }
    }
}