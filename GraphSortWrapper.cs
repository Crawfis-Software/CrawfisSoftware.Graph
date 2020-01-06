using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Converts a graph to another instance of the graph that
    /// is topologically sorted.
    /// </summary>
    /// <typeparam name="N">The type of the nodes in the graph.</typeparam>
    /// <typeparam name="E">The type of the data on an edge.</typeparam>
    public class GraphSortWrapper<N, E> : IGraph<N, E>, ISortedGraph
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graph">The graph that will be wrapped into a sorted graph.</param>
        GraphSortWrapper( IGraph<N, E> graph )
        {
            _graph = graph;
            _topologicalSort = GraphQuery<N, E>.GetTopologicalSort(graph);
        }

        #region IGraph<N,E> Members
        /// <summary>
        /// Iterator for the nodes in the graph.
        /// </summary>
        public IEnumerable<N> Nodes
        {
            get
            {
                foreach (N node in _topologicalSort)
                    yield return node;
            }
        }
        /// <summary>
        /// Iterator for the children or neighbors of the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An enumerator of nodes.</returns>
        public IEnumerable<N> Neighbors(N node)
        {
            return _graph.Neighbors(node);
        }
        /// <summary>
        /// Iterator over the parents or immediate ancestors of a node.
        /// </summary>
        /// <remarks>May not be supported by all graphs.</remarks>
        /// <param name="node">The node.</param>
        /// <returns>An enumerator of nodes.</returns>
        public IEnumerable<N> Parents(N node)
        {
            return _graph.Parents(node);
        }
        /// <summary>
        /// Iterator over the emanating edges from a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An enumerator of nodes.</returns>
        public IEnumerable<IEdge<N, E>> OutEdges(N node)
        {
            return _graph.OutEdges(node);
        }
        /// <summary>
        /// Iterator over the in-coming edges of a node.
        /// </summary>
        /// <remarks>May not be supported by all graphs.</remarks>
        /// <param name="node">The node.</param>
        /// <returns>An enumerator of edges.</returns>
        public IEnumerable<IEdge<N, E>> InEdges(N node)
        {
            return _graph.InEdges(node);
        }

        /// <summary>
        /// Iterator for the edges in the graph, yielding IEdge's
        /// </summary>
        public IEnumerable<IEdge<N, E>> Edges
        {
            get
            {
                return _graph.Edges;
            }
        }
        /// <summary>
        /// Tests whether an edge exists between two nodes.
        /// </summary>
        /// <param name="fromNode">The node that the edge emanates from.</param>
        /// <param name="toNode">The node that the edge terminates at.</param>
        /// <returns>True if the edge exists in the graph. False otherwise.</returns>
        public bool ContainsEdge(N fromNode, N toNode)
        {
            return _graph.ContainsEdge(fromNode, toNode);
        }
        /// <summary>
        /// Gets the label on an edge.
        /// </summary>
        /// <param name="fromNode">The node that the edge emanates from.</param>
        /// <param name="toNode">The node that the edge terminates at.</param>
        /// <returns>The edge.</returns>
        public E GetEdgeLabel(N fromNode, N toNode)
        {
            return _graph.GetEdgeLabel(fromNode, toNode);
        }
        /// <summary>
        /// Exception safe routine to get the label on an edge.
        /// </summary>
        /// <param name="fromNode">The node that the edge emanates from.</param>
        /// <param name="toNode">The node that the edge terminates at.</param>
        /// <param name="edge">The resulting edge if the method was successful. A default
        /// value for the type if the edge could not be found.</param>
        /// <returns>True if the edge was found. False otherwise.</returns>
        public bool TryGetEdge(N fromNode, N toNode, out E edge)
        {
            return _graph.TryGetEdge(fromNode, toNode, out edge);
        }
        #endregion

        #region ISortedGraph Members
        /// <summary>
        /// Indicates whether the graph is already sorted.
        /// </summary>
        public bool IsSorted
        {
            get { return true; }
        }
        /// <summary>
        /// Sorts the graph.
        /// </summary>
        public void SortTopologically()
        {
        }
        #endregion

        private IGraph<N, E> _graph;
        private IEnumerable<N> _topologicalSort;
    }
}
