using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Helper extension methods for common index graph traversal algorithms.
    /// </summary>
    public static class GraphTraversalExtensions
    {
        /// <summary>
        /// Breadth-first traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<N> BreadthFirstTraversalNodes<N, E>(this IGraph<N, E> graph, N startingIndex)
        {
            var gridEnumerator = new GraphEnumerator<N, E>(graph, new QueueAdaptor<N>());
            return gridEnumerator.TraverseNodes(startingIndex);
        }

        /// <summary>
        /// Depth-first traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<N> DepthFirstTraversalNodes<N, E>(this IGraph<N, E> graph, N startingIndex)
        {
            var gridEnumerator = new GraphEnumerator<N, E>(graph, new StackAdaptor<N>());
            return gridEnumerator.TraverseNodes(startingIndex);
        }

        /// <summary>
        /// Breadth-first traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node.</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IEdge<N,E>> BreadthFirstTraversalEdges<N, E>(this IGraph<N, E> graph, N startingIndex)
        {
            var gridEnumerator = new GraphEdgeEnumerator<N, E>(graph, new QueueAdaptor<IEdge<N,E>>());
            return gridEnumerator.TraverseGraph(startingIndex);
        }

        /// <summary>
        /// Depth-first traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node.</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IEdge<N,E>> DepthFirstTraversalEdges<N, E>(this IGraph<N, E> graph, N startingIndex)
        {
            var gridEnumerator = new GraphEdgeEnumerator<N, E>(graph, new StackAdaptor<IEdge<N,E>>());
            return gridEnumerator.TraverseGraph(startingIndex);
        }
   }
}