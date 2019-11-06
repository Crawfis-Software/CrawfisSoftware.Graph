using System.Collections.Generic;

namespace OhioState.Collections.Graph
{
    /// <summary>
    /// Static methods that determine paths from a graph.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    public static class PathQuery<N, E>
    {
        /// <summary>
        /// Enumerates through the edges of a graph from a starting node to to a target node.
        /// </summary>
        /// <param name="graph">The graph to search for a valid path.</param>
        /// <param name="start">The starting node.</param>
        /// <param name="target">The target node. The search may terminate early once found.</param>
        /// <returns></returns>
        public static IEnumerable<IEdge<N, E>> FindPath(IGraph<N, E> graph, N start, N target)
        {
            GraphEdgeEnumerator<N, E> graphWalker = new GraphEdgeEnumerator<N, E>(graph, new QueueAdaptor<IEdge<N, E>>());

            bool pathFound = false;
            IDictionary<N, IEdge<N, E>> parentList = new Dictionary<N, IEdge<N, E>>();
            foreach (IEdge<N, E> edge in graphWalker.TraverseGraph(start))
            {
                parentList.Add(edge.To, edge);
                if (EqualityComparer<N>.Default.Equals(edge.To, target) == true)
                {
                    pathFound = true;
                    break;
                }
            }

            if (!pathFound)
                yield break;
            // Walk the dictionary backwards until we find the start.
            Stack<IEdge<N, E>> path = new Stack<IEdge<N, E>>();
            N currentNode = target;
            while (EqualityComparer<N>.Default.Equals(currentNode, start) == false)
            {
                path.Push(parentList[currentNode]);
                currentNode = parentList[currentNode].From;
            }
            // Now we can output the path.
            foreach (IEdge<N, E> pathNode in path)
                yield return pathNode;
        }
        /// <summary>
        /// FindPath provides the index-based edges from the start node to the target node if a path is found.
        /// If a path is not found, no edges are enumerated.
        /// </summary>
        /// <param name="graph">The index-based graph to query against.</param>
        /// <param name="start">The starting node index for the desired path.</param>
        /// <param name="target">The destination being searched for.</param>
        /// <returns>An enumeration of the path from the starting node to the target node.</returns>
        public static IEnumerable<IIndexedEdge<E>> FindPath(IIndexedGraph<N, E> graph, int start, int target)
        {
            return FindPath(graph, start, target, new EdgeCostComparer<N, E>(graph, start));
        }
        /// <summary>
        /// FindPath provides the index-based edges from the start node to the target node if a path is found.
        /// If a path is not found, no edges are enumerated.
        /// </summary>
        /// <param name="graph">The index-based graph to query against.</param>
        /// <param name="start">The starting node index for the desired path.</param>
        /// <param name="target">The destination being searched for.</param>
        /// <param name="costDelegate">A function used to retrieve or calculate
        /// the cost for a given edge.</param>
        /// <returns>An enumeration of the path from the starting node to the target node.</returns>
        /// <seealso cref="EdgeCostDelegate{E}"/>
        public static IEnumerable<IIndexedEdge<E>> FindPath(IIndexedGraph<N, E> graph, int start, int target,
            EdgeCostDelegate<E> costDelegate)
        {
            EdgeCostComparer<N, E> costComparer = new EdgeCostComparer<N, E>(graph, start);
            if (costDelegate != null)
                costComparer.EdgeCostDelegate = costDelegate;

            return FindPath(graph, start, target, costComparer);
        }
        /// <summary>
        /// FindPath provides the index-based edges from the start node to the target node if a path is found.
        /// If a path is not found, no edges are enumerated.
        /// </summary>
        /// <param name="graph">The index-based graph to query against.</param>
        /// <param name="start">The starting node index for the desired path.</param>
        /// <param name="target">The destination being searched for.</param>
        /// <param name="costDelegate">A function used to retrieve or calculate
        /// the cost for a given edge.</param>
        /// <returns>An enumeration of the path from the starting node to the target node.</returns>
        /// <seealso cref="IIndexedEdgeCostComparer{E}"/>
        public static IEnumerable<IIndexedEdge<E>> FindPath(IIndexedGraph<N, E> graph, int start, int target,
            IIndexedEdgeCostComparer<E> costDelegate)
        {
            HeapAdaptor<IIndexedEdge<E>> heap = new HeapAdaptor<IIndexedEdge<E>>();
            heap.ComparerToUse = costDelegate;

            IndexedGraphEdgeEnumerator<N, E> graphWalker = new IndexedGraphEdgeEnumerator<N, E>(graph, heap);

            IDictionary<int, IIndexedEdge<E>> parentList = new Dictionary<int, IIndexedEdge<E>>();
            bool pathFound = false;
            foreach (IIndexedEdge<E> edge in graphWalker.TraverseGraph(start))
            {
                parentList.Add(edge.To, edge);
                costDelegate.UpdateCost(edge);
                if (edge.To == target)
                {
                    pathFound = true;
                    break;
                }
            }

            if (!pathFound)
                yield break;

            //
            // Walk the dictionary backwards until we find the start.
            // Note, that the parentList contains an encoding of part of 
            // the shortest-path tree from the startNode. That is, we can
            // find the shortest path from the start node to any node 
            // in the parentList. Not all nodes will be in the parentList.
            //
            Stack<IIndexedEdge<E>> path = new Stack<IIndexedEdge<E>>();
            int currentNode = target;
            while (currentNode != start)
            {
                path.Push(parentList[currentNode]);
                currentNode = parentList[currentNode].From;
            }
            // Now we can output the path.
            foreach (IIndexedEdge<E> pathNode in path)
                yield return pathNode;
        }
    }
}
