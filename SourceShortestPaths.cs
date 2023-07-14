using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Calculates all of the paths from a given node to all reachable nodes.
    /// </summary>
    /// <typeparam name="N">The graph node label type.</typeparam>
    /// <typeparam name="E">The edge label type.</typeparam>
    /// <remarks>The paths can be queried by passing in a target node from the source.</remarks>
    public class SourceShortestPaths<N, E>
    {
        /// <summary>
        /// Calculates the shortest paths for all reachable nodes in a graph
        /// from a specified initial starting node.
        /// </summary>
        /// <param name="graph">The indexed graph used for the search.</param>
        /// <param name="startingNode">The source node index of the resulting shortest paths.</param>
        /// <param name="costDelegate">A function used to retrieve or calculate
        /// the cost for a given edge.</param>
        /// <remarks>Implements Dijkstra's algorithm.</remarks>
        public SourceShortestPaths(IIndexedGraph<N, E> graph, int startingNode, EdgeCostDelegate<E> costDelegate)
        {
            this.startingNode = startingNode;
            costComparer = new PathCostComparer<N, E>(graph, startingNode);
            if (costDelegate != null)
                costComparer.EdgeCostDelegate = costDelegate;
            FindPaths(graph, costComparer);
        }

        /// <summary>
        /// Return the minimum path cost to the target node.
        /// </summary>
        /// <param name="targetNode">An index into a graph node.</param>
        /// <returns>The cost of the minimum path to the target node.</returns>
        public float GetCost(int targetNode)
        {
            return costComparer.PathCost(targetNode);
        }

        /// <summary>
        /// Actual method that returns an enumeration of the path edges.
        /// </summary>
        /// <param name="targetNode"></param>
        /// <returns>An IEnumerable of IIndexedEdge's.</returns>
        public IEnumerable<IIndexedEdge<E>> GetPath(int targetNode)
        {
            // If the target node does not have a parent, then this node
            // was unreachable from the start and no path exists. In
            // this case return an empty path.
            if (!parentList.ContainsKey(targetNode))
                yield break;

            //
            // Walk the dictionary backwards until we find the start.
            // Note, that the parentList contains an encoding of part of 
            // the shortest-path tree from the startNode. That is, we can
            // find the shortest path from the start node to any node 
            // in the parentList. Not all nodes will be in the parentList.
            //
            Stack<IIndexedEdge<E>> path = new Stack<IIndexedEdge<E>>();
            int currentNode = targetNode;
            while (currentNode != startingNode)
            {
                path.Push(parentList[currentNode]);
                currentNode = parentList[currentNode].From;
            }
            // Now we can output the path.
            foreach (IIndexedEdge<E> pathNode in path)
                yield return pathNode;
        }
        /// <summary>
        /// FindPath provides the index-based edges from the start node to the target node if a path is found.
        /// If a path is not found, no edges are enumerated.
        /// </summary>
        /// <param name="graph">The index-based graph to query against.</param>
        /// <param name="costDelegate">A function used to retrieve or calculate
        /// the cost for a given edge.</param>
        /// <returns>An enumeration of the path from the starting node to the target node.</returns>
        /// <seealso cref="IIndexedEdgeCostComparer{E}"/>
        private void FindPaths(IIndexedGraph<N, E> graph,
            IIndexedEdgeCostComparer<E> costDelegate)
        {
            HeapAdaptor<IIndexedEdge<E>> heap = new HeapAdaptor<IIndexedEdge<E>>();
            heap.ComparerToUse = costDelegate;

            IndexedGraphEdgeEnumerator<N, E> graphWalker = new IndexedGraphEdgeEnumerator<N, E>(graph, heap);

            foreach (IIndexedEdge<E> edge in graphWalker.TraverseGraph(startingNode))
            {
                parentList.Add(edge.To, edge);
                costDelegate.UpdateCost(edge);
            }
        }

        private int startingNode;
        IDictionary<int, IIndexedEdge<E>> parentList = new Dictionary<int, IIndexedEdge<E>>();
        PathCostComparer<N, E> costComparer;
    }
}
