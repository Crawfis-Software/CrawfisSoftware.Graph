using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Provides a scoped location to place static methods that take
    /// a graph as input and provide queries related to the components
    /// or connectivity of the graph.
    /// </summary>
    /// <typeparam name="N">The type of the nodes in the graph.</typeparam>
    /// <typeparam name="E">The type of the data on an edge.</typeparam>
    public static class Connectivity<N, E>
    {
        #region IGraph queries
        /// <summary>
        /// Calculates the number of connected components (or strongly connected components) in the graph.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <returns>The number of connected components in the graph.</returns>
        /// <seealso cref="Connectivity{N, E}.NumberOfComponents(IIndexedGraph{N,E})"/>
        public static int NumberOfComponents(IGraph<N, E> graph)
        {
            return NumberOfComponents(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Calculates the number of connected components (or strongly connected components) in the graph.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded.</param>
        /// <returns>The number of connected components in the graph.</returns>
        public static int NumberOfComponents(IGraph<N, E> graph, int MaxNodes)
        {
            IList<IList<N>> components = Components(graph, MaxNodes);
            return components.Count;
        }

        /// <summary>
        /// Iterator over each component in a graph.
        /// </summary>
        /// <remarks>This only corresponds to connected-components if the graph is topologically sorted.</remarks>
        /// <param name="graph">A graph.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>An enumeration of disjoint collections of nodes.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static IEnumerable<IEnumerable<N>> ConnectedComponents(IGraph<N, E> graph, int MaxNodes)
        {
            IList<IList<N>> components = Components(graph, MaxNodes);
            foreach (IList<N> component in components)
            {
                yield return component;
            }
        }
        #endregion

        #region IIndexedGraph queries
        /// <summary>
        /// Calculates the number of connected components (or strongly connected components) in the graph.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <returns>The number of connected components in the graph.</returns>
        /// <seealso cref="Connectivity{N, E}.NumberOfComponents(IGraph{N,E})"/>
        public static int NumberOfComponents(IIndexedGraph<N, E> graph)
        {
            return NumberOfComponents(graph, System.Int32.MaxValue);
        }
        /// <summary>
        /// Calculates the number of connected components (or strongly connected components) in the graph.
        /// </summary>
        /// <param name="graph">The graph to query against.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>The number of connected components in the graph.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static int NumberOfComponents(IIndexedGraph<N, E> graph, int MaxNodes)
        {
            IList<IList<int>> components = Components(graph, MaxNodes);
            return components.Count;
        }
        /// <summary>
        /// Iterator over each component in a graph.
        /// </summary>
        /// <remarks>This only corresponds to connected-components if the graph is topologically sorted.</remarks>
        /// <param name="graph">An index-based graph.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>An enumeration of disjoint collections of node indices.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static IEnumerable<IEnumerable<int>> ConnectedComponents(IIndexedGraph<N, E> graph, int MaxNodes)
        {
            IList<IList<int>> components = Components(graph, MaxNodes);
            foreach (IList<int> component in components)
            {
                yield return component;
            }
        }

        /// <summary>
        /// Returns the number of strongly connected-components.
        /// </summary>
        /// <remarks>This only corresponds to strongly connected-components if the graph is topologically 
        /// sorted and <paramref name="graphTranspose"/> corresponds to the transpose of <paramref name="graph"/>.</remarks>
        /// <param name="graph">An index-based graph.</param>
        /// <param name="graphTranspose">An index-based graph.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>The number of strongly-connected components in a directed graph, or
        /// the number of components in an undirected graph.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static int NumberOfStronglyConnectedComponents(IIndexedGraph<N, E> graph, IIndexedGraph<N, E> graphTranspose, int MaxNodes)
        {
            IList<IList<int>> components = StrongComponents(graph, graphTranspose, MaxNodes);
            return components.Count;
        }
        /// <summary>
        /// Iterator over each strongly connected-component.
        /// </summary>
        /// <remarks>This only corresponds to strongly connected-components if the graph is topologically 
        /// sorted and <paramref name="graphTranspose"/> corresponds to the transpose of <paramref name="graph"/>.</remarks>
        /// <param name="graph">An index-based graph.</param>
        /// <param name="graphTranspose">An index-based graph.</param>
        /// <param name="MaxNodes">For very large (and possibly infinite) graphs, the MaxNodes
        /// parameter provides a guard to ensure the algorithm is bounded. Can also be used to 
        /// query whether the graph is larger than a certain size.</param>
        /// <returns>An enumeration of disjoint collections of node indices.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the user-specified 
        /// maximum number of nodes to enumerate is exceeded. Since a graph can be infinite in
        /// size, this guards against passing an infinite graph (e.g., set of all whole numbers)
        /// into the routine.</exception>
        public static IEnumerable<IEnumerable<int>> StronglyConnectedComponents(IIndexedGraph<N, E> graph, IIndexedGraph<N, E> graphTranspose, int MaxNodes)
        {
            IList<IList<int>> components = StrongComponents(graph, graphTranspose, MaxNodes);
            foreach (IList<int> component in components)
            {
                yield return component;
            }
        }
        #endregion

        #region Implementation
        private static IList<IList<N>> Components(IGraph<N, E> graph, int MaxNodes)
        {
            return StrongComponents(graph, graph, MaxNodes);
        }
        private static IList<IList<N>> StrongComponents(IGraph<N, E> graph, IGraph<N, E> graphTranspose, int MaxNodes)
        {
            IEnumerable<N> postOrder = GraphQuery<N, E>.GetTopologicalSort(graph, MaxNodes);

            GraphEnumerator<N, E> graphWalker = new GraphEnumerator<N, E>(graphTranspose);

            //List<N> visited = new List<N>();
            Dictionary<N, bool> visited = new Dictionary<N, bool>();
            foreach (N node in graph.Nodes)
            {
                visited[node] = false;
            }
            IList<IList<N>> components = new List<IList<N>>();
            IList<N> currentComponent;
            foreach (N rootNode in postOrder)
            {
                if (!visited[rootNode])
                {
                    currentComponent = new List<N>();
                    components.Add(currentComponent);
                    foreach (N node in graphWalker.TraverseNodes(rootNode))
                    {
                        if (!visited[node])
                        {
                            visited[node] = true;
                            currentComponent.Add(node);
                        }
                    }
                }
            }
            return components;
        }

        private static IList<IList<int>> Components(IIndexedGraph<N, E> graph, int MaxNodes)
        {
            return StrongComponents(graph, graph, MaxNodes);
        }
        private static IList<IList<int>> StrongComponents(IIndexedGraph<N, E> graph, IIndexedGraph<N, E> graphTranspose, int MaxNodes)
        {
            IEnumerable<int> postOrder = GraphQuery<N, E>.GetTopologicalSort(graph, MaxNodes);

            IndexedGraphEnumerator<N, E> graphWalker = new IndexedGraphEnumerator<N, E>(graphTranspose);

            int count = graph.NumberOfNodes;
            Dictionary<int,bool> visited = new Dictionary<int, bool>(count);
            foreach(int node in graph.Nodes)
            {
                visited[node] = false;
            }
            IList<IList<int>> components = new List<IList<int>>();
            IList<int> currentComponent;
            foreach (int rootNode in postOrder)
            {
                if (!visited[rootNode])
                {
                    currentComponent = new List<int>();
                    components.Add(currentComponent);
                    foreach (int node in graphWalker.TraverseNodes(rootNode))
                    {
                        if (!visited[node])
                        {
                            visited[node] = true;
                            currentComponent.Add(node);
                        }
                    }
                }
            }
            return components;
        }
        #endregion
    }
}