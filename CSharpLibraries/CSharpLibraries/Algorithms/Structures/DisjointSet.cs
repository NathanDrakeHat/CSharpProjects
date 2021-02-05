namespace CSharpLibraries.Algorithms.Structures
{
    /// <summary>
    /// extends or initial as a public field
    /// </summary>
    public class DisjointSet
    {
        private int Rank { set; get; }

        private DisjointSet Parent { set; get; }

        protected DisjointSet()
        {
            Parent = this;
        }

        /// <summary>
        /// find identifier of the set of an element
        /// </summary>
        /// <param name="x">element</param>
        /// <returns>identifier of the set of an element</returns>
        public static DisjointSet FindSet(DisjointSet x)
        {
            if (!ReferenceEquals(x, x.Parent))
            {
                x.Parent = FindSet(x.Parent);
            }

            return x.Parent;
        }

        public static void Union(DisjointSet a, DisjointSet b) => Link(FindSet(a), FindSet(b));

        private static void Link(DisjointSet x, DisjointSet y)
        {
            if (x.Rank > y.Rank)
            {
                y.Parent = x;
            }
            else
            {
                x.Parent = y;
                if (x.Rank == y.Rank)
                {
                    y.Rank += 1;
                }
            }
        }
    }
}