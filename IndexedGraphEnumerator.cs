using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Provides several iterators for an index-based graph that can be used to enumerate the graph's nodes
    /// in a particular order.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    /// <seealso cref="IndexedGraphEdgeEnumerator{N, E}"/>
    /// <seealso cref="GraphEnumeratorBase"/>
    /// <seealso cref="CrawfisSoftware.Collections.Graph"/>
    public class IndexedGraphEnumerator<N, E> : Graph.GraphEnumeratorBase
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/> that
        /// will be traversed.</param>
        /// <remarks>This will construct a <typeparamref name="IndexedGraphEnumerator{N,E}"/> that will
        /// perform a depth-first traversal of the <typeparamref name="IIndexedGraph{N,E}"/>.</remarks>
        public IndexedGraphEnumerator(IIndexedGraph<N, E> graph)
            : this(graph, new StackAdaptor<int>())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/></param> that
        /// will be traversed.
        /// <param name="searchContainer">An <typeparamref name="IPriorityCollection{T}"/>
        /// to use for the traversal.</param>
        /// <remarks>Passing in a <typeparamref name="StackAdaptor{T}"/> will traverse the
        /// graph in a depth-first manner. This is the default.</remarks>
        /// <remarks>Passing in a <typeparamref name="QueueAdaptor{T}"/> will traverse the
        /// graph in a breadth-first manner.</remarks>
        public IndexedGraphEnumerator(IIndexedGraph<N, E> graph, IPriorityCollection<int> searchContainer)
        {
            indexedGraph = graph;
            activeList = searchContainer;
            CurrentParent = -1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get an <typeparamref name="IEnumerable{T}"/> of the <c>Components</c> in the 
        /// <typeparamref name="IGraph{N,E}"/>. Each component is an <typeparamref name="IEnumerable{T}"/>
        /// of node indices.
        /// </summary>
        public IEnumerable<IEnumerable<int>> Components
        {
            get
            {
                Reset();
                componentNumber = 0;
                foreach (int rootNode in indexedGraph.Nodes)
                {
                    if (!visited[rootNode])
                    {
                        CurrentParent = rootNode;
                        yield return TraverseFromNode(rootNode);
                        componentNumber++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the node index of an already enumerated graph node that is a neighbor to the current node.
        /// </summary>
        public int CurrentParent
        {
            get;
            private set;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Traverse all of the nodes in the graph.
        /// </summary>
        /// <returns>An IEnumerable{T} of the node indices.</returns>
        public IEnumerable<int> TraverseNodes()
        {
            foreach (IEnumerable<int> component in this.Components)
            {
                foreach (int node in component)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Traverse the <typeparamref name="IGraph{N,E}"/> starting from the specified node
        /// until no more connected nodes exist.
        /// </summary>
        /// <param name="startingNode">The node index to start the traversal from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// node indices.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the startingNode.</remarks>
        public IEnumerable<int> TraverseNodes(int startingNode)
        {
            Reset();
            return TraverseFromNode(startingNode);
        }
        #endregion

        #region Implementation
        private void Reset()
        {
            CurrentParent = -1;
            visited = new Dictionary<int, bool>(indexedGraph.NumberOfNodes);
            foreach (int node in indexedGraph.Nodes)
            {
                visited[node] = false;
            }

            activeList.Clear();
        }
        private IEnumerable<int> TraverseFromNode(int startingNode)
        {
            if (this.TraversalOrder == Graph.TraversalOrder.PreOrder)
                return TraverseComponent(startingNode);
            else
                return TraverseComponentRecursively(startingNode);
        }
        private IEnumerable<int> TraverseComponentRecursively(int startingNode)
        {
            if (!visited[startingNode])
            {
                visited[startingNode] = true;
                CurrentParent = startingNode;
                foreach (int neighbor in indexedGraph.Neighbors(startingNode))
                {
                    if (!visited[neighbor])
                    {
                        foreach (int node in TraverseComponentRecursively(neighbor))
                        {
                            CurrentParent = neighbor;
                            yield return node;
                        }
                    }
                }
                yield return startingNode;
            }
        }
        private IEnumerable<int> TraverseComponent(int startingNode)
        {
            activeList.Put(startingNode);
            while (activeList.Count > 0)
            {
                int currentNode = activeList.GetNext();
                if (!visited[currentNode])
                {
                    visited[currentNode] = true;
                    if (this.TraversalOrder == Graph.TraversalOrder.PreOrder)
                        yield return currentNode;
                    foreach (int node in indexedGraph.Neighbors(currentNode))
                    {
                        if (!visited[node])
                        {
                            activeList.Put(node);
                        }
                    }
                    if (this.TraversalOrder == Graph.TraversalOrder.PostOrder)
                        yield return currentNode;
                }
            }
        }
        #endregion

        #region member variables
        private IIndexedGraph<N, E> indexedGraph;
        private IDictionary<int,bool> visited;
        private IPriorityCollection<int> activeList;
        #endregion
    }
}
