using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// An enumerator to walk the edges of a <typeparamref name="IIndexedGraph{N,E}"/>.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
    public class IndexedGraphEdgeEnumerator<N, E>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/> that
        /// will be traversed.</param>
        /// <remarks>This will construct a <typeparamref name="IndexedGraphEdgeEnumerator{N,E}"/> that will
        /// perform a depth-first traversal of the <typeparamref name="IIndexedGraph{N,E}"/>.</remarks>
        public IndexedGraphEdgeEnumerator(IIndexedGraph<N, E> graph)
            : this(graph, new StackAdaptor<IIndexedEdge<E>>())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/> that
        /// will be traversed.</param>
        /// <remarks>This will construct a <typeparamref name="IndexedGraphEdgeEnumerator{N,E}"/> that will
        /// perform a depth-first traversal of the <typeparamref name="IIndexedGraph{N,E}"/>.</remarks>
        /// <param name="searchContainer">An <typeparamref name="IPriorityCollection{T}"/>
        /// to use for the traversal.</param>
        /// <remarks>Passing in a <typeparamref name="StackAdaptor{T}"/> will traverse the
        /// graph in a depth-first manner. This is the default.</remarks>
        /// <remarks>Passing in a <typeparamref name="QueueAdaptor{T}"/> will traverse the
        /// graph in a breadth-first manner.</remarks>
        public IndexedGraphEdgeEnumerator(IIndexedGraph<N, E> graph, IPriorityCollection<IIndexedEdge<E>> searchContainer)
        {
            indexedGraph = graph;
            activeList = searchContainer;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Traverse the <typeparamref name="IIndexedGraph{N,E}"/> starting from the specified node
        /// until no more connected nodes exist.
        /// </summary>
        /// <param name="startingNode">The node to start the traversal from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> where T is a <see cref="IIndexedEdge{E}"/>.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the startingNode once. Not all edges are enumerated.</remarks>
        public IEnumerable<IIndexedEdge<E>> TraverseNodes(int startingNode)
        {
            ResetNodeTraversal();
            return ResumeTraverseGraph(startingNode);
        }

        /// <summary>
        /// Traverse the <typeparamref name="IIndexedGraph{N,E}"/> 
        /// until no more connected nodes exist. 
        /// </summary>
        /// <param name="startingNodes">A list of node index to start the traversal from. The PriorityCollection determines how when these nodes are output.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> where T is a <see cref="IIndexedEdge{E}"/>.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the list of startingNodes.</remarks>
        /// <remarks>Component numbers should be ignored when using this.</remarks>
        public IEnumerable<IIndexedEdge<E>> TraverseNodes(IEnumerable<int> startingNodes)
        {
            ResetNodeTraversal();
            foreach (int node in startingNodes)
            {
                visitedNodes[node] = true;
                foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(node))
                {
                    activeList.Put(edge);
                }
            }
            return ResumeTraverseGraph(0, true);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Restarts the traversal.
        /// </summary>
        protected void ResetNodeTraversal()
        {
            visitedNodes = new Dictionary<int, bool>(indexedGraph.NumberOfNodes);
            foreach (int node in indexedGraph.Nodes)
            {
                visitedNodes[node] = false;
            }

            activeList.Clear();
        }
        /// <summary>
        /// Traverses any untouched graph nodes that are accessible from the specified node.
        /// </summary>
        /// <param name="startingNode">A new node to continue the search from.</param>
        /// <param name="listIsPrePrimed">True is the activeList is already initialized with a set of starting nodes and the neighbors.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> where T is a <see cref="IIndexedEdge{E}"/>.</returns>
        protected IEnumerable<IIndexedEdge<E>> ResumeTraverseGraph(int startingNode, bool listIsPrePrimed = false)
        {
            if (!listIsPrePrimed)
            {
                visitedNodes[startingNode] = true;
                foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(startingNode))
                {
                    activeList.Put(edge);
                }
            }
            while (activeList.Count > 0)
            {
                IIndexedEdge<E> currentEdge = activeList.GetNext();
                if (!visitedNodes[currentEdge.To])
                {
                    int currentNode = currentEdge.To;
                    yield return currentEdge;
                    visitedNodes[currentNode] = true;
                    foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(currentNode))
                    {
                        if (!visitedNodes[edge.To])
                        {
                            activeList.Put(edge);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Traverse the <typeparamref name="IIndexedGraph{N,E}"/> starting from the specified node
        /// until no more connected edges exist.
        /// </summary>
        /// <param name="startingNode">The node to start the traversal from.</param>
        /// <param name="isUndirected">If true treats edges as bi-direction (undirected) adn will not traverse them twice for undirected graphs.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> where T is a <see cref="IIndexedEdge{E}"/>.</returns>
        /// <remarks>This routine will only traverse those edges reachable from 
        /// the startingNode.</remarks>
        public IEnumerable<IIndexedEdge<E>> TraverseEdges(int startingNode, bool isUndirected = true)
        {
            ResetEdgeTraversal();
            return ResumeTraverseEdges(startingNode, isUndirected);
        }
        /// <summary>
        /// Restarts the traversal for edge traversals.
        /// </summary>
        protected void ResetEdgeTraversal()
        {
            visitedEdges = new Dictionary<(int, int), bool>(indexedGraph.NumberOfEdges);
            activeList.Clear();
        }
        /// <summary>
        /// Traverses any untouched graph edges that are accessible from the specified node.
        /// </summary>
        /// <param name="startingNode">A new node to continue the search from.</param>
        /// <param name="isUndirected">If true treats edges as bi-direction (undirected) and will not traverse them twice for undirected graphs.</param>
        /// <param name="listIsPrePrimed">True if the activeList is already initialized with a set of starting nodes and the neighbors.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> where T is a <see cref="IIndexedEdge{E}"/>.</returns>

        protected IEnumerable<IIndexedEdge<E>> ResumeTraverseEdges(int startingNode, bool isUndirected = true, bool listIsPrePrimed = false)
        {
            if (!listIsPrePrimed)
            {
                foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(startingNode))
                {
                    activeList.Put(edge);
                }
            }
            while (activeList.Count > 0)
            {
                IIndexedEdge<E> currentEdge = activeList.GetNext();
                int from = currentEdge.From;
                int to = currentEdge.To;
                if (isUndirected && from > to)
                {
                    int temp = from;
                    from = to;
                    to = temp;
                }

                if (!visitedEdges.ContainsKey((from, to)))
                {
                    yield return currentEdge;
                    visitedEdges[(from, to)] = true;
                    foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(currentEdge.To))
                    {
                        from = edge.From;
                        to = edge.To;
                        if (isUndirected && from > to)
                        {
                            int temp = from;
                            from = to;
                            to = temp;
                        }
                        if (!visitedEdges.ContainsKey((from, to)))
                        {
                            activeList.Put(edge);
                        }
                    }
                }
            }
        }
        #endregion

        #region Member variables
        private IIndexedGraph<N, E> indexedGraph;
        private IPriorityCollection<IIndexedEdge<E>> activeList;
        private IDictionary<int, bool> visitedNodes;
        private IDictionary<(int, int), bool> visitedEdges;
        #endregion
    }
}