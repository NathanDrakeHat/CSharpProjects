#nullable disable
using System;
using System.Collections.Generic;
using static CSharpLibraries.Extensions.Extension;

namespace CSharpLibraries.Algorithms.Graph
{
    public enum GraphDirection
    {
        Directed,
        NonDirected
    }

    public sealed class LinkedGraph<TVertex>
    {
        #region InnerClass

        public sealed class Edge
        {
            public readonly TVertex FormerVertex;
            public readonly TVertex LaterVertex;
            private readonly GraphDirection _edgeGraphDirection;
            public double Weight { get; internal set; }

            internal Edge(TVertex former, TVertex later, double weight, GraphDirection isDirected)
            {
                Weight = weight;
                FormerVertex = former;
                LaterVertex = later;
                _edgeGraphDirection = isDirected;
            }

            public TVertex AnotherSide(TVertex vertex)
            {
                if (vertex.Equals(FormerVertex)) return LaterVertex;
                else if (vertex.Equals(LaterVertex)) return FormerVertex;
                else throw new ArgumentException("Arg vertex not in this edge.");
            }

            public override string ToString()
            {
                if (_edgeGraphDirection == GraphDirection.Directed)
                    return $"[Edge({FormerVertex} >>> {LaterVertex})], weight:{Weight}";
                else
                    return $"[Edge({FormerVertex} <-> {LaterVertex})], weight:{Weight}";
            }
        }

        #endregion

        #region Fields

        public int Size { get; private set; }
        private readonly GraphDirection _graphGraphDirection;
        private readonly List<TVertex> _vertices = new List<TVertex>();
        private readonly Dictionary<TVertex, List<Edge>> _edgesMap = new Dictionary<TVertex, List<Edge>>();

        #endregion

        public LinkedGraph(IEnumerable<TVertex> vertices, GraphDirection isDirected)
        {
            NotNullArg(vertices, nameof(vertices));
            Size = 0;
            foreach (var vertex in vertices)
            {
                if (vertex == null) throw new ArgumentException("null element in container",nameof(vertices));
                _edgesMap[vertex] = new List<Edge>();
                _vertices.Add(vertex);
                Size++;
            }

            _graphGraphDirection = isDirected;
        }

        public LinkedGraph(LinkedGraph<TVertex> otherGraph)
        {
            NotNullArg(otherGraph,nameof(otherGraph));
            Size = otherGraph._vertices.Count;
            _graphGraphDirection = otherGraph._graphGraphDirection;
            _vertices.AddRange(otherGraph._vertices);
            AddTo(otherGraph._edgesMap, _edgesMap);

            static void AddTo<TK, TV>(Dictionary<TK, TV> dictionary1, Dictionary<TK, TV> dictionary2)
            {
                foreach (var entry in dictionary1)
                    dictionary2[entry.Key] = entry.Value;
            }
        }

        #region Methods

        public void SetNeighbor(TVertex vertex, TVertex neighbor, double w = 1.0)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            if (neighbor == null) throw new ArgumentNullException(nameof(neighbor));
            var edgeT = new Edge(vertex, neighbor, w, _graphGraphDirection);
            if (_graphGraphDirection == GraphDirection.Directed)
            {
                var edgesList = _edgesMap[vertex];
                edgesList.Add(edgeT);
            }
            else
            {
                var edgesList = _edgesMap[vertex];
                edgesList.Add(edgeT);

                edgesList = _edgesMap[neighbor];
                edgesList.Add(edgeT);
            }
        }

        public void AddVertex(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            if (_vertices.Contains(vertex) || _edgesMap.ContainsKey(vertex))
                throw new InvalidOperationException("Duplicate vertex.");
            Size++;
            _vertices.Add(vertex);
            _edgesMap[vertex] = new List<Edge>();
        }

        public List<Edge> AllEdges()
        {
            var res = new List<Edge>();
            foreach (var vertex in _vertices)
                res.AddRange(_edgesMap[vertex]);
            return res;
        }

        public List<TVertex> AllVertices() => new List<TVertex>(_vertices);

        public List<Edge> EdgesAt(TVertex vertex) => new List<Edge>(_edgesMap[vertex]);

        #endregion
    }
}