using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Helper extension methods for common index graph traversal algorithms.
    /// </summary>
    public static class IndexedGraphTraversalExtensions
    {
        /// <summary>
        /// Breadth-first traversal of a graph.
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
        /// Depth-first traversal of a graph.
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
        /// Best-first (Dijkstra's algorithm) traversal of a graph using a function converting the general edge labels to a numeric (float) value.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<int> DijkstraTraversalNodes<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
        {
            yield return startingIndex;
            foreach (var edge in DijkstraTraversalEdges(graph, startingIndex, costDelegate))
            {
                yield return edge.To;
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with integer edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of node indices.</returns>
        public static IEnumerable<int> DijkstraTraversalNodes<N>(this IIndexedGraph<N, int> graph, int startingIndex)
        {
            yield return startingIndex;
            foreach (var edge in DijkstraTraversalEdges(graph, startingIndex))
            {
                yield return edge.To;
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with path costs using a function converting the general edge labels to a numeric (float) value.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of value-tuples including the next (cheapest) node and the minimum total path cost to reach that node.</returns>
        public static IEnumerable<(int cellIndex, float pathCost)> DijkstraTraversalNodesWithCosts<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
        {
            yield return (startingIndex, 0);
            foreach (var (edge, _, cost) in DijkstraTraversalEdgesWithCosts(graph, startingIndex, costDelegate))
            {
                yield return (edge.To, cost);
            }
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

        /// <summary>
        /// Depth-first traversal of a graph.
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

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<E>> DijkstraTraversalEdges<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
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
                yield return edge;
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph that includes the path cost.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <param name="costDelegate">Function to convert the Edge label to a float representing the cost of the edge.</param>
        /// <returns>An enumerable of value-tuples including the edge used to reach the next node and the minimum total path cost to reach that node.</returns>
        /// <remarks>This traverses all reachable nodes, not all reachable edges.</remarks>
        public static IEnumerable<(IIndexedEdge<E> edge, float fromCost, float toCost)> DijkstraTraversalEdgesWithCosts<N, E>(this IIndexedGraph<N, E> graph, int startingIndex, EdgeCostDelegate<E> costDelegate)
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
        public static IEnumerable<IIndexedEdge<int>> DijkstraTraversalEdges<N>(this IIndexedGraph<N, int> graph, int startingIndex)
        {
            int start = startingIndex;
            PathCostComparer<N, int> costComparer = new PathCostComparer<N, int>(graph);
            costComparer.EdgeCostDelegate = (edge) => { return edge.Value; };
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<int>> heap = new HeapAdaptor<IIndexedEdge<int>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, int>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                yield return edge;
                costComparer.UpdateCost(edge);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with integer  (long) edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<long>> DijkstraTraversalEdges<N>(this IIndexedGraph<N, long> graph, int startingIndex)
        {
            int start = startingIndex;
            PathCostComparer<N, long> costComparer = new PathCostComparer<N, long>(graph);
            costComparer.EdgeCostDelegate = (edge) => { return edge.Value; };
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<long>> heap = new HeapAdaptor<IIndexedEdge<long>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, long>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                yield return edge;
                costComparer.UpdateCost(edge);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with boolean edge weights. True is low cost, false is high cost.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<bool>> DijkstraTraversalEdges<N>(this IIndexedGraph<N, bool> graph, int startingIndex)
        {
            const float largeValue = 10000000f;
            int start = startingIndex;
            PathCostComparer<N, bool> costComparer = new PathCostComparer<N, bool>(graph);
            costComparer.EdgeCostDelegate = (edge) => { return edge.Value ? 1f : largeValue; };
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<bool>> heap = new HeapAdaptor<IIndexedEdge<bool>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, bool>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                yield return edge;
                costComparer.UpdateCost(edge);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with floating point edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<float>> DijkstraTraversalEdges<N>(this IIndexedGraph<N, float> graph, int startingIndex)
        {
            int start = startingIndex;
            PathCostComparer<N, float> costComparer = new PathCostComparer<N, float>(graph);
            costComparer.EdgeCostDelegate = (edge) => { return edge.Value; };
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<float>> heap = new HeapAdaptor<IIndexedEdge<float>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, float>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                yield return edge;
                costComparer.UpdateCost(edge);
            }
        }

        /// <summary>
        /// Best-first (Dijkstra's algorithm) traversal of a graph with double precision edge weights.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node..</returns>
        /// <remarks>This traversing all reachable node, not all reachable edges.</remarks>
        public static IEnumerable<IIndexedEdge<double>> DijkstraTraversalEdges<N>(this IIndexedGraph<N, double> graph, int startingIndex)
        {
            int start = startingIndex;
            PathCostComparer<N, double> costComparer = new PathCostComparer<N, double>(graph);
            costComparer.EdgeCostDelegate = (edge) => { return (float)edge.Value; };
            costComparer.Initialize(start);
            HeapAdaptor<IIndexedEdge<double>> heap = new HeapAdaptor<IIndexedEdge<double>>();
            heap.ComparerToUse = costComparer;
            var gridEnumerator = new IndexedGraphEdgeEnumerator<N, double>(graph, heap);
            foreach (var edge in gridEnumerator.TraverseNodes(start))
            {
                yield return edge;
                costComparer.UpdateCost(edge);
            }
        }
    }
}
