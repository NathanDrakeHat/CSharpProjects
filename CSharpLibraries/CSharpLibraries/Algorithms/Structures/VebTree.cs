#nullable disable
using System;

namespace CSharpLibraries.Algorithms.Structures
{
    public sealed class VebTree
    {
        //keys are not duplicate and confined in a range
        private int u; //universe size
        private readonly VebTree _summary;
        private readonly VebTree[] _cluster;
        private readonly int NONE = -1;
        private int _min = -1; // hidden in cluster
        private int _max = -1;

        // ReSharper disable once UnusedParameter.Local
        private VebTree(int u, string s)
        {
            this.u = u;
            if (u != 2)
            {
                var c = (int) Math.Ceiling(Math.Pow(u, 1 / 2.0));
                _summary = new VebTree(c, "");
                _cluster = new VebTree[c];
                var f = (int) Math.Floor(Math.Pow(u, 1 / 2.0));
                for (int i = 0; i < _cluster.Length; i++)
                {
                    _cluster[i] = new VebTree(f);
                }
            }
        }

        public VebTree(int k)
        {
            if (k < 1) throw new ArgumentOutOfRangeException(nameof(k));
            u = (int) Math.Pow(2, k);
            if (u != 2)
            {
                var c = (int) Math.Ceiling(Math.Pow(u, 1 / 2.0)); // return double
                _summary = new VebTree(c, "");
                _cluster = new VebTree[c];
                var f = (int) Math.Floor(Math.Pow(u, 1 / 2.0));
                for (int i = 0; i < _cluster.Length; i++)
                {
                    _cluster[i] = new VebTree(f, "");
                }
            }
        }

        private void emptyInsert(VebTree tree, int x)
        {
            tree._min = x;
            tree._max = x;
        }

        private void Insert(VebTree tree, int x)
        {
            // recursive strip x into cluster index and array index
            if (tree._min == NONE)
            {
                emptyInsert(tree, x);
            }
            else
            {
                if (x < tree._min)
                {
                    var t = x;
                    x = tree._min;
                    tree._min = t;
                }

                if (tree.u > 2)
                {
                    if (minimum(tree._cluster[tree.High(x)]) == NONE)
                    {
                        //                System.out.println(String.format("enter summary, cluster index %d", tree.high(x)));
                        Insert(tree._summary, tree.High(x));
                        emptyInsert(tree._cluster[tree.High(x)], tree.Low(x));
                    }
                    else
                    {
                        //                System.out.println(String.format("enter cluster %d, index %d", tree.high(x), tree.low(x)));
                        Insert(tree._cluster[tree.High(x)], tree.Low(x));
                    }
                }

                if (x > tree._max)
                {
                    tree._max = x;
                }
            }
        }

        public VebTree SafeInsert(int x)
        {
            if (x >= u) throw new ArgumentOutOfRangeException(nameof(x));
            if (!HasMember(x))
            {
                Insert(this, x);
            }

            return this;
        }

        public VebTree UncheckInsert(int x)
        {
            // duplicate insert will invoke problems
            if (x >= u) throw new ArgumentOutOfRangeException(nameof(x));
            Insert(this, x);
            return this;
        }

        public VebTree SafeDelete(int x)
        {
            if (x >= u) throw new ArgumentOutOfRangeException(nameof(x));
            if (HasMember(x)) // can't delete multi-time, can't delete none
            {
                Delete(this, x);
            }

            return this;
        }

        public VebTree UncheckDelete(int x)
        {
            //duplicate delete or delete items not in tree will invoke problems
            if (x >= u) throw new ArgumentOutOfRangeException(nameof(x));
            Delete(this, x);
            return this;
        }

        private void Delete(VebTree tree, int x)
        {
            // worst case lg(lg u)
            // base case
            if (tree._min == tree._max)
            {
                tree._min = NONE;
                tree._max = NONE;
            }
            else if (tree.u == 2)
            {
                if (x == 0) tree._min = 1;
                else tree._min = 0;
                tree._max = tree._min;
            }
            else
            {
                if (x == tree._min)
                {
                    var firstCluster = minimum(tree._summary);
                    x = tree.Index(firstCluster, minimum(tree._cluster[firstCluster]));
                    tree._min = x; // second min to tree.min
                }

                Delete(tree._cluster[tree.High(x)], tree.Low(x));
                if (minimum(tree._cluster[tree.High(x)]) == NONE)
                {
                    Delete(tree._summary, tree.High(x));
                    if (x == tree._max)
                    {
                        var summaryMax = Maximum(tree._summary);
                        if (summaryMax == NONE) tree._max = tree._min;
                        else tree._max = tree.Index(summaryMax, Maximum(tree._cluster[summaryMax]));
                    }
                }
                else if (x == tree._max)
                    tree._max = tree.Index(tree.High(x), Maximum(tree._cluster[tree.High(x)]));
            }
        }


        public int ForceGetMaximum()
        {
            var res = Maximum(this);
            if (res != NONE) return res;
            else throw new InvalidOperationException();
        }

        private static int Maximum(VebTree tree) => tree._max;


        public int ForceGetMinimum()
        {
            var res = minimum(this);
            if (res != NONE) return res;
            else throw new InvalidOperationException();
        }

        private int minimum(VebTree tree) => tree._min;


        public bool HasMember(int x) => hasMember(this, x);


        private bool hasMember(VebTree tree, int x)
        {
            if (x == tree._min || x == tree._max) return true;
            else if (tree.u == 2) return false;
            else return hasMember(tree._cluster[tree.High(x)], tree.Low((x)));
        }

        public int ForceGetSuccessor(int x)
        {
            var res = Successor(this, x);
            if (res != NONE) return res;
            else throw new InvalidOperationException();
        }

        private int Successor(VebTree tree, int x)
        {
            // base
            if (tree.u == 2)
            {
                if (x == 0 && tree._max == 1) return 1; // have x and successor
                else return NONE;
            }
            else if (tree._min != NONE && x < tree._min) return tree._min; // dose not have x but have successor
            else
            {
                // recursive
                var maxLow = Maximum(tree._cluster[tree.High(x)]);
                if (maxLow != NONE && tree.Low(x) < maxLow)
                {
                    var offset = Successor(tree._cluster[tree.High(x)], tree.Low(x));
                    return tree.Index(tree.High(x), offset);
                }
                else
                {
                    var succCluster = Successor(tree._summary, tree.High(x));
                    if (succCluster == NONE) return NONE;
                    var offset = minimum(tree._cluster[succCluster]);
                    return tree.Index(succCluster, offset);
                }
            }
        }

        public int ForceGetPredecessor(int x)
        {
            var res = Predecessor(this, x);
            if (res != NONE) return res;
            else throw new InvalidOperationException();
        }

        private int Predecessor(VebTree tree, int x)
        {
            if (tree.u == 2)
            {
                if (x == 1 && tree._min == 0) return 0;
                else return NONE;
            }
            else if (tree._max != NONE && x > tree._max) return tree._max;

            else
            {
                var minLow = minimum(tree._cluster[tree.High(x)]);
                if (minLow != NONE && tree.Low(x) > minLow)
                {
                    //and
                    var offset = Predecessor(tree._cluster[tree.High(x)], tree.Low(x));
                    return tree.Index(tree.High(x), offset);
                }
                else
                {
                    var predCluster = Predecessor(tree._summary, tree.High(x));
                    if (predCluster == NONE)
                    {
                        if (tree._min != NONE && x > tree._min) return tree._min;
                        else return NONE;
                    }
                    else
                    {
                        var offset = Maximum(tree._cluster[predCluster]);
                        return tree.Index(predCluster, offset);
                    }
                }
            }
        }

        private int High(int x) => (int) (x / Math.Floor(Math.Pow(u, 1 / 2.0)));


        private int Low(int x) => (int) (x % Math.Floor(Math.Pow(u, 1 / 2.0)));


        private int Index(int h, int l) => (int) (h * Math.Floor(Math.Pow(u, 1 / 2.0)) + l);
    }
}