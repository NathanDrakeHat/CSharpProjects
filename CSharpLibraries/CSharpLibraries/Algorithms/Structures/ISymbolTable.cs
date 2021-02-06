using System;
using System.Collections.Generic;

namespace CSharpLibraries.Algorithms.Structures
{
    public interface ISymbolTable<Key, Value>
        where Key : IComparable<Key>
    {
        Value this[Key key] { get; set; }

        void Delete(Key key);

        bool Contains(Key key);

        bool IsEmpty();

        int Size { get; }

        Key Min();

        Key Max();

        Key Floor();

        Key Ceiling();

        int Rank(Key key);

        Key Select(int rank);

        void DeleteMin()
        {
            Delete(Min());
        }

        void DeleteMax()
        {
            Delete(Max());
        }

        /// <summary>
        /// [low, high]
        /// </summary>
        /// <param name="low">include</param>
        /// <param name="high">include</param>
        /// <returns></returns>
        int Count(int low, int high);

        IEnumerable<Key> Keys(Key low, Key high);

        IEnumerable<Key> Keys() => Keys(Min(), Max());
    }
}