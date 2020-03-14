using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Calculates the shortest paths for all reachable nodes in a graph
    /// from a specified initial starting node.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    /// <param name="costDelegate">A function used to retrieve or calculate
    /// the cost for a given edge.</param>
    /// <remarks>Implements Dijkstra's algorithm.</remarks>
    public class SourceShortestPaths<N, E>
    {
        public SourceShortestPaths(IIndexedGraph<N, E> graph, int startingNode, EdgeCostDelegate<E> costDelegate)
        {
            this.startingNode = startingNode;
            EdgeCostComparer<N, E> costComparer = new EdgeCostComparer<N, E>(graph, startingNode);
            if (costDelegate != null)
                costComparer.EdgeCostDelegate = costDelegate;
            FindPaths(graph, costComparer);
        }
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
    }
}
