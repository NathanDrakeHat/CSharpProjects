namespace CSharpLibraries.Algorithms.Structures
{
    public sealed class DpMatrix<TAny>
    {
        private readonly TAny[][] _matrix;

        public DpMatrix(int size)
        {
            _matrix = new TAny[size][];
            for (int i = 0; i < size; i++)
                _matrix[i] = new TAny[size - i];
        }

        public TAny this[int r, int c]
        {
            get => _matrix[r][c - r];
            set => _matrix[r][c - r] = value;
        }
    }
}