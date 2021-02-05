#nullable disable
using System;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Strings
{
    public class StringAutomata
    {
        public sealed class TransitionEntry
        {
            private readonly int _integer;
            private readonly char _character;
            private readonly int _hash;
            private readonly string _s;

            public TransitionEntry(int i, char c)
            {
                _integer = i;
                _character = c;
                _hash = HashCode.Combine(_integer, _character);
                _s = $"TransitionEntry({_integer},{_character})";
            }

            public override bool Equals(object other)
            {
                if (other == null) throw new ArgumentNullException(nameof(other));
                if (other is TransitionEntry entry)
                {
                    return (_integer == entry._integer) &&
                           (_character == entry._character);
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode() => _hash;


            public override string ToString() => _s;
        }

        public static Dictionary<TransitionEntry, int> ComputeTransitionPattern(string pattern, char[] charSet)
        {
            int m = pattern.Length;
            var map = new Dictionary<TransitionEntry, int>();
            for (int q = 0; q <= m; q++)
            {
                foreach (var a in charSet)
                {
                    int k = Math.Min(m + 1, q + 2);
                    do
                    {
                        k--;
                    } while (!string.Concat(pattern.Substring(0, q), a).EndsWith(pattern.Substring(0, k)));

                    map[new TransitionEntry(q, a)] = k;
                }
            }

            return map;
        }

        public static List<int> FiniteAutomationMatcher(string t, Dictionary<TransitionEntry, int> delta, int m)
        {
            List<int> res = new List<int>();
            int n = t.Length;
            int q = 0;
            for (int i = 0; i < n; i++)
            {
                q = delta[new TransitionEntry(q, t[i])];
                if (q == m)
                {
                    res.Add(i - m);
                }
            }

            return res;
        }
    }
}