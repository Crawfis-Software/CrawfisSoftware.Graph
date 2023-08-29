using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Helper extension methods for common index graph traversal algorithms: depth-first, breadth-first, Dijkstra's traversal.
    /// Can Traverse all nodes (e.g, BreadthFirstTraversalNodes) or all Edges (e.g., BreadthFirstTraversalEdges).
    /// Traversals can return either just node indices (and path costs with Dijkstra's), or the edge used to reach that node (adding a WithEdges suffix - BreadthFirstTraversalNodesWithEdges).
    /// </summary>
    public static class IndexedGraphTraversalExtensions
    {
        #region Breadth-first
        /// <summary>
        /// Breadth-first traversal of all nodes in  a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<int> BreadthFirstTraversalNodes<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEnumerator<N, E>(graph, new QueueAdaptor<int>());
            return gridEnumerator.TraverseNodes(startingIndex);
        }

        /// <summary>
        /// Breadth-first traversal all nodes in of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node and the edges used to reach them.</returns>
        public static IEnumerable<IIndexedEdge<E>> BreadthFirstTraversalNodesWithEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, new QueueAdaptor<IIndexedEdge<E>>());
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
        public static IEnumerable<IIndexedEdge<E>> BreadthFirstTraversalEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, new QueueAdaptor<IIndexedEdge<E>>());
            return gridEnumerator.TraverseEdges(startingIndex);
        }
        #endregion Breadth-first
        #region Depth-first
        /// <summary>
        /// Depth-first traversal of all nodes in a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<int> DepthFirstTraversalNodes<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEnumerator<N, E>(graph, new StackAdaptor<int>());
            return gridEnumerator.TraverseNodes(startingIndex);
        }

        /// <summary>
        /// Depth-first traversal of all nodes in a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node and the edges used to reach them.</returns>
        public static IEnumerable<IIndexedEdge<E>> DepthFirstTraversalNodesWithEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, new StackAdaptor<IIndexedEdge<E>>());
            return gridEnumerator.TraverseNodes(startingIndex);
        }

        /// <summary>
        /// Depth-first traversal of all edges in a graph
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node.</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<E>> DepthFirstTraversalEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, new StackAdaptor<IIndexedEdge<E>>());
            return gridEnumerator.TraverseEdges(startingIndex);
        }
        #endregion Depth-first

        #region Dijkstra
        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph using a function converting the general edge labels to a numeric (float) value.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<(int cellIndex, float pathCost)> DijkstraTraversalNodes<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
        {
            yield return (startingIndex, 0);
            foreach (var (edge, fromCost, toCost) in DijkstraTraversalNodesWithEdges(graph, startingIndex, costDelegate))
            {
                yield return (edge.To, toCost);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph using a function converting the general edge labels to a numeric (float) value.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<(IIndexedEdge<E> edge, float fromPathCost, float toPathCost)> DijkstraTraversalNodesWithEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
        {
            int start = startingIndex;
            PathCostComparer<N, E> costComparer = new PathCostComparer<N, E>(graph);
            costComparer.EdgeCostDelegate = costDelegate;
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<E>> heap = new HeapAdaptor<IIndexedEdge<E>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                costComparer.UpdateCost(edge);
                float fromPathCost = costComparer.PathCost(edge.From);
                float toPathCost = costComparer.PathCost(edge.To);
                yield return (edge, fromPathCost, toPathCost);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with integer edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices and the minimum path cost to reach that node.</returns>
        public static IEnumerable<(int cellIndex, float pathCost)> DijkstraTraversalNodes<N>(this IIndexedGraph<N, int> graph, int startingIndex)
        {
            return DijkstraTraversalNodes(graph, startingIndex, (edge) => { return edge.Value; });
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of the edges used to reach each node.</returns>
        /// <remarks>This traverses all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<E> edge, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
        {
            int start = startingIndex;
            PathCostComparer<N, E> costComparer = new PathCostComparer<N, E>(graph);
            costComparer.EdgeCostDelegate = costDelegate;
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<E>> heap = new HeapAdaptor<IIndexedEdge<E>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseEdges(start))
            {
                costComparer.UpdateCost(edge);
                float fromPathCost = costComparer.PathCost(edge.From);
                float toPathCost = costComparer.PathCost(edge.To);
                yield return (edge, fromPathCost, toPathCost);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with integer edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<int>, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N>(this IIndexedGraph<N, int> graph, int startingIndex)
        {
            return DijkstraTraversalEdges(graph, startingIndex, (edge) => { return edge.Value; });
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with integer  (long) edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<long>, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N>(this IIndexedGraph<N, long> graph, int startingIndex)
        {
            return DijkstraTraversalEdges(graph, startingIndex, (edge) => { return edge.Value; });
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with boolean edge weights. True is low cost, false is high cost.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<bool>, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N>(this IIndexedGraph<N, bool> graph, int startingIndex)
        {
            const float largeValue = 10000000f;
            return DijkstraTraversalEdges(graph, startingIndex, (edge) => { return edge.Value ? 1f : largeValue; });
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with floating point edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<float>, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N>(this IIndexedGraph<N, float> graph, int startingIndex)
        {
            return DijkstraTraversalEdges(graph, startingIndex, (edge) => { return edge.Value; });
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with double precision edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<double>, float fromPathCost, float toPathCost)> DijkstraTraversalEdges<N>(this IIndexedGraph<N, double> graph, int startingIndex)
        {
            return DijkstraTraversalEdges(graph, startingIndex, (edge) => { return (float)edge.Value; });
        }
        #endregion Dijkstra
    }
}