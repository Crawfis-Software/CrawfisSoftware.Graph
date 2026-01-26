using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Provides several iterators for a graph that can be used to enumerate the graph's nodes
    /// in a particular order.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    public class GraphEnumerator<N, E> : GraphEnumeratorBase
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IGraph{N,E}"/> that
        /// will be traversed.</param>
        /// <remarks>This will construct a <typeparamref name="GraphEnumerator{N,E}"/> that will
        /// perform a depth-first traversal of the <typeparamref name="IGraph{N,E}"/>.</remarks>
        public GraphEnumerator(IGraph<N, E> graph)
            : this(graph, new StackAdaptor<N>())
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IGraph{N,E}"/></param> that
        /// will be traversed.
        /// <param name="searchContainer">An <typeparamref name="IPriorityCollection{T}"/>
        /// to use for the traversal.</param>
        /// <remarks>Passing in a <typeparamref name="StackAdaptor{T}"/> will traverse the
        /// graph in a depth-first manner. This is the default.</remarks>
        /// <remarks>Passing in a <typeparamref name="QueueAdaptor{T}"/> will traverse the
        /// graph in a breadth-first manner.</remarks>
        public GraphEnumerator(IGraph<N, E> graph, IPriorityCollection<N> searchContainer)
        {
            _graph = graph;
            activeList = searchContainer;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get an <typeparamref name="IEnumerable{T}"/> of the <c>Components</c> in the 
        /// <typeparamref name="IGraph{N,E}"/>. Each component is an <typeparamref name="IEnumerable{T}"/>
        /// of node labels.
        /// </summary>
        public IEnumerable<IEnumerable<N>> Components
        {
            get
            {
                Reset();
                componentNumber = 0;
                foreach (N rootNode in _graph.Nodes)
                {
                    if (!visited.Contains(rootNode))
                    {
                        yield return TraverseFromNode(rootNode);
                        componentNumber++;
                    }
                }
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Traverse all of the nodes in the graph.
        /// </summary>
        /// <returns>An IEnumerable{T} of the node labels.</returns>
        public IEnumerable<N> TraverseNodes()
        {
            foreach (IEnumerable<N> component in this.Components)
            {
                foreach (N node in component)
                {
                    yield return node;
                }
            }
        }
        /// <summary>
        /// Traverse the <typeparamref name="IGraph{N,E}"/> starting from the specified node
        /// until no more connected nodes exist.
        /// </summary>
        /// <param name="startingNode">The node to start the traversal from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// node labels.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the startingNode.</remarks>
        public IEnumerable<N> TraverseNodes(N startingNode)
        {
            Reset();
            return TraverseFromNode(startingNode);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Restart the traversal.
        /// </summary>
        protected void Reset()
        {
            visited = new List<N>();
            activeList.Clear();
        }

        private IEnumerable<N> TraverseFromNode(N startingNode)
        {
            if (this.TraversalOrder == TraversalOrder.PreOrder)
                return TraverseComponent(startingNode);
            else
                return TraverseComponentRecursively(startingNode);
        }
        private IEnumerable<N> TraverseComponentRecursively(N startingNode)
        {
            if (!visited.Contains(startingNode))
            {
                visited.Add(startingNode);
                foreach (N neighbor in _graph.Neighbors(startingNode))
                {
                    if (!visited.Contains(neighbor))
                    {
                        foreach (N node in TraverseComponentRecursively(neighbor))
                            yield return node;
                    }
                }
                yield return startingNode;
            }
        }
        private IEnumerable<N> TraverseComponent(N startingNode)
        {
            activeList.Put(startingNode);
            while (activeList.Count > 0)
            {
                N currentNode = activeList.GetNext();
                if (!visited.Contains(currentNode))
                {
                    visited.Add(currentNode);
                    if (this.TraversalOrder == TraversalOrder.PreOrder)
                        yield return currentNode;
                    foreach (N node in _graph.Neighbors(currentNode))
                    {
                        if (!visited.Contains(node))
                        {
                            activeList.Put(node);
                        }
                    }
                    if (this.TraversalOrder == TraversalOrder.PostOrder)
                        yield return currentNode;
                }
            }
        }
        #endregion

        #region Member Variables
        private List<N> visited;
        private IPriorityCollection<N> activeList;
        private IGraph<N, E> _graph;
        #endregion
    }
}
