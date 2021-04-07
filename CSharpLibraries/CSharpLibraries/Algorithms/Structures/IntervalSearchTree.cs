namespace CSharpLibraries.Algorithms.Structures{
    public class IntervalSearchTree<TEle>{
        // private readonly Func<TEle, TEle, int> _eleComparer;

        // public IntervalSearchTree(Func<TEle, TEle, int> eleComparer) :
        //     base((tuple1, tuple2) => eleComparer(tuple1.Item1, tuple2.Item1))
        // {
        //     throw new NotImplementedException();
        //     _eleComparer = eleComparer;
        // }


        // public void Insert(Tuple<TEle, TEle> range)
        // {
        // if (range == null || range.Item1 == null || range.Item2 == null)
        //     throw new ArgumentNullException(nameof(range));
        // var n = new Node(range, range.Item2);
        // base.Insert(n);
        // Count++;
        // var ptr = n;
        // while (ptr != Root && _eleComparer(ptr.Value, ptr.Parent.Value) > 0)
        // {
        //     ptr.Parent.Value = ptr.Value;
        //     ptr = ptr.Parent;
        // }
        // }


        // public Tuple<TEle, TEle> SearchLap(Tuple<TEle, TEle> range)
        // {
        // if (range == null || range.Item1 == null || range.Item2 == null)
        //     throw new ArgumentNullException();
        // return SearchLap(Root, range);
        // }

        // private Tuple<TEle, TEle> SearchLap(Node n, Tuple<TEle, TEle> range)
        // {
        // if (n == Sentinel)
        //     return null;
        // if (IsOverlap(n.Key, range))
        //     return n.Key;
        // else if (n.Left == Sentinel)
        //     return SearchLap(n.Right, range);
        // else if (_eleComparer(n.Left.Key.Item1, range.Item1) < 0)
        //     return SearchLap(n.Right, range);
        // else
        //     return SearchLap(n.Left, range);
        // }

        // private bool IsOverlap(Tuple<TEle, TEle> a, Tuple<TEle, TEle> b)
        // {
        //     return (_eleComparer(a.Item1, b.Item1) >= 0 && _eleComparer(a.Item1, b.Item2) <= 0) ||
        //            (_eleComparer(a.Item2, b.Item1) >= 0 && _eleComparer(a.Item2, b.Item2) <= 0);
        // }
    }
}