using System;
using System.Collections.Generic;

#nullable disable
namespace CSharpLibraries.Algorithms.Structures
{
    /// <summary>
    /// better insert implementation
    /// </summary>
    public class RedBlackBst<Key, Value> : ISymbolTable<Key, Value> where Key : IComparable<Key>
    {
        private enum Color
        {
            Red,
            Black
        }

        public Value this[Key key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Delete(Key key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Key key)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public int Size { get; }

        public Key Min()
        {
            throw new NotImplementedException();
        }

        public Key Max()
        {
            throw new NotImplementedException();
        }

        public Key Floor()
        {
            throw new NotImplementedException();
        }

        public Key Ceiling()
        {
            throw new NotImplementedException();
        }

        public int Rank(Key key)
        {
            throw new NotImplementedException();
        }

        public Key Select(int rank)
        {
            throw new NotImplementedException();
        }

        public int Count(int low, int high)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Key> Keys(Key low, Key high)
        {
            throw new NotImplementedException();
        }
    }
}