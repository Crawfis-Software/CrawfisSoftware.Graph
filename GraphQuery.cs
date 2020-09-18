using System;
using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Provides a scoped location to place static methods that take
    /// a graph as input and provide basic queries on it.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    public static class GraphQuery<N, E>
    {
        #region Acyclic test
        /// <summary>
        /// Checks whether the graph has any cycles.
        /// </summary>
        /// <param name="graph">An index-based graph that the test will be performed on.</param>
        /// <returns>True if there are no cycles (the graph is acyclic). False otherwise.</returns>
        /// <remarks>The algorithm uses a topological sort (see <seealso cref="TraversalOrder.PostOrder"/>) to enumerate
        /// the nodes in the graph. If a node has an out-going edge to another node that has not
        /// already been enumerated, then a cycle exists and the routine returns false.
        /// Note that the routine will return early if a cycle is found.</remarks>
        public static bool IsAcyclic(IIndexedGraph<N, E> graph)
        {
            bool acyclic = true;
            IndexedGraphEnumerator<N, E> graphWalker = new IndexedGraphEnumerator<N, E>(graph);
            graphWalker.TraversalOrder = TraversalOrder.PostOrder;
            //
            // postOrder will keep track of whether we have already visited a node. Initial 
            // values are false, and they are changed to true once all of the node's children 
            // have been processed, since we requested a post-order traveral of the graph.
            //
            Dictionary<int, bool> postOrder = new Dictionary<int, bool>(graph.NumberOfNodes);
            foreach (int node in graph.Nodes)
            {
                postOrder[node] = false;
            }
            foreach (int node in graphWalker.TraverseNodes())
            {
                postOrder[node] = true;
                //
                // Check to see if all of its children have been added to the list.
                // If not, then they are waiting for their children to return in the
                // post-order, and hence this node must be an ancestor and a cycle exists.
                //
                foreach (int neighbor in graph.Neighbors(node))
                {
                    if (!postOrder[neighbor])
                    {
                        acyclic = false;
                        return acyclic;
                    }
                }
            }
            return acyclic;
        }

        /// <summary>
        /// Checks whether an undirected graph has any cycles.
        /// </summary>
        /// <param name="graph">An index-based graph that the test will be performed on.</param>
        /// <returns>True if there are no cycles (the graph is acyclic). False otherwise.</returns>
        /// <remarks>Note that the routine will return early if a cycle is found.</remarks>
        public static bool IsAcyclicUndirected(IIndexedGraph<N, E> graph)
        {
            bool acyclic = true;
            IndexedGraphEnumerator<N, E> graphWalker = new IndexedGraphEnumerator<N, E>(graph);
            graphWalker.TraversalOrder = TraversalOrder.PreOrder;
            Dictionary<int, bool> visited = new Dictionary<int, bool>(graph.NumberOfNodes);
            foreach (int node in graph.Nodes)
            {
                visited[node] = false;
            }
            foreach (int node in graphWalker.TraverseNodes())
            {
                visited[node] = true;
                //
                // Check to see if there is at most one (usually one except for 
                // and roots of the forest) neighbors who have already been
                // visited. If yes, then a cycle exists. This represents the 
                // presence of a backedge in addition to the "parent" edge.
                //
                int count = 0;
                foreach (int neighbor in graph.Neighbors(node))
                {
                    if (visited[neighbor] && neighbor != node)
                        count++;
                }
                if (count > 1)
                {
                    acyclic = false;
                    break;
                }
            }
            return acyclic;
        }

        /// <summary>
        /// Checks whether the graph has any cycles.
        /// </summary>
        /// <param name="graph">The graph that the test will be performed on.</param>
        /// <returns>True if there are no cycles (the graph is acyclic). False otherwise.</returns>
        /// <remarks>The algorithm uses a topological sort (see <seealso cref="TraversalOrder.PostOrder"/>) to enumerate
        /// the nodes in the graph. If a node has an out-going edge to another node that has not
        /// already been enumerated, then a cycle exists and the routine returns false.
        /// <para>Note that the routine will return early if a cycle is found,</para></remarks>
        public static bool IsAcyclic(IGraph<N, E> graph)
        {
            bool acyclic = true;
            GraphEnumerator<N, E> graphWalker = new GraphEnumerator<N, E>(graph);
            graphWalker.TraversalOrder = TraversalOrder.PostOrder;
            //
            // postOrder will keep track of each node we visit. It is added to the list
            // once all of its children have been processed, since we requested a post-order
            // traveral of the graph.
            //
            List<N> postOrder = new List<N>();
            foreach (N node in graphWalker.TraverseNodes())
            {
                postOrder.Add(node);
                //
                // Check to see if all of its children have been added to the list.
                // If not, then they are waiting for their children to return in the
                // post-order, and hence this node must be an ancestor and a cycle exists.
                //
                foreach (N neighbor in graph.Neighbors(node))
                {
                    if (!postOrder.Contains(neighbor))
                    {
                        acyclic = false;
                        return acyclic;
                    }
                }
            }
            return acyclic;
        }
        #endregion

        #region Topological sort
        /// <summary>
        /// Enumerate through the graph nodes in topological order
        /// </summary>
        /// <param name="graph">The graph to enumerate.</param>
        /// <returns>An enumeration of the graph nodes.</returns>
        public static IEnumerable<N> GetTopologicalSort(IGraph<N, E> graph)
        {
            return GetTopologicalSort(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Enumerate through the graph nodes in topological order
        /// </summary>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the <paramref name="MaxNodes"/>
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>An enumeration of the graph nodes.</returns>
        /// <returns>The number of nodes in the graph.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static IEnumerable<N> GetTopologicalSort(IGraph<N, E> graph, int MaxNodes)
        {
            int count = 0;
            GraphEnumerator<N, E> graphWalker = new GraphEnumerator<N, E>(graph);
            graphWalker.TraversalOrder = TraversalOrder.PostOrder;
            Stack<N> postOrder = new Stack<N>();
            foreach (N node in graphWalker.TraverseNodes())
            {
                postOrder.Push(node);
                count++;
                if (count > MaxNodes)
                    throw new System.InvalidOperationException("The maximum number of nodes specified was exceeded in NumberOfComponents");
            }

            return postOrder;
        }

        /// <summary>
        /// Enumerate through the graph nodes in topological order
        /// </summary>
        /// <param name="graph">The graph to enumerate.</param>
        /// <returns>An enumeration of the graph nodes.</returns>
        public static IEnumerable<int> GetTopologicalSort(IIndexedGraph<N, E> graph)
        {
            return GetTopologicalSort(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Enumerate through the graph nodes in topological order
        /// </summary>
        /// <param name="graph">The graph to enumerate.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the <paramref name="MaxNodes"/>
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>An enumeration of the graph nodes.</returns>
        /// <returns>The number of nodes in the graph.</returns>
        /// <exception cref="InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static IEnumerable<int> GetTopologicalSort(IIndexedGraph<N, E> graph, int MaxNodes)
        {
            int count = 0;
            IndexedGraphEnumerator<N, E> graphWalker = new IndexedGraphEnumerator<N, E>(graph);
            graphWalker.TraversalOrder = TraversalOrder.PostOrder;
            Stack<int> postOrder = new Stack<int>(graph.NumberOfNodes);
            foreach (int node in graphWalker.TraverseNodes())
            {
                postOrder.Push(node);
                count++;
                if (count > MaxNodes)
                    throw new InvalidOperationException("The maximum number of nodes specified was exceeded in NumberOfComponents");
            }

            return postOrder;
        }
        #endregion

        #region Size and Degree queries
        /// <summary>
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <returns>The number of nodes in the graph.</returns>
        public static int NumberOfNodes(IGraph<N, E> graph)
        {
            return NumberOfNodes(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Counts the number of nodes or vertices.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the <paramref name="MaxNodes"/>
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>The number of nodes in the graph.</returns>
        /// <exception cref="InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static int NumberOfNodes(IGraph<N, E> graph, int MaxNodes)
        {
            IFiniteGraph<N, E> diagram = graph as IFiniteGraph<N, E>;
            if (diagram != null)
                return diagram.NumberOfNodes;

            int count = 0;
            foreach (N node in graph.Nodes)
            {
                count++;
                if (count > MaxNodes)
                    throw new InvalidOperationException("The maximum number of nodes specified was exceeded in NumberOfNodes");
            }
            return count;
        }

        /// <summary>
        /// Counts the number of Edges in the graph.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <returns>The number of edges in the graph.</returns>
        public static int NumberOfEdges(IGraph<N, E> graph)
        {
            return NumberOfEdges(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Counts the number of edges.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <param name="MaxEdges">For very large (and possibly infinite) graphs, the <paramref name="MaxEdges"/>
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>The number of edges in the graph.</returns>
        /// <exception cref="InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of edges to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static int NumberOfEdges(IGraph<N, E> graph, int MaxEdges)
        {
            IFiniteGraph<N, E> diagram = graph as IFiniteGraph<N, E>;
            if (diagram != null)
                return diagram.NumberOfEdges;

            int count = 0;
            foreach (IEdge<N, E> edge in graph.Edges)
            {
                count++;
                if (count > MaxEdges)
                    throw new InvalidOperationException("The maximum number of edges specified was exceeded in NumberOfEdges");
            }
            return count;
        }
        #endregion
    }
}
