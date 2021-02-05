using System.Collections;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Miscellaneous
{
    public class Enumeration : IEnumerable<int>
    {
        private readonly int _low;
        private readonly int _high;
        public Enumeration(int l, int h)
        {
            _low = l;
            _high = h;
        }
        public IEnumerator<int> GetEnumerator()
        {
            for (int i = _low; i < _high; i++)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}