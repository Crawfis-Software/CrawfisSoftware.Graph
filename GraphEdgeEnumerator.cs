using System;
using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// An enumerator to walk the edges of a <typeparamref name="IGraph{N,E}"/>.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
    public class GraphEdgeEnumerator<N, E>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IGraph{N,E}"/> that
        /// will be traversed.</param>
        /// <remarks>This will construct a <typeparamref name="GraphEdgeEnumerator{N,E}"/> that will
        /// perform a depth-first traversal of the <typeparamref name="IGraph{N,E}"/>.</remarks>
        public GraphEdgeEnumerator(IGraph<N, E> graph)
            : this(graph, new StackAdaptor<IEdge<N, E>>())
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
        public GraphEdgeEnumerator(IGraph<N, E> graph, IPriorityCollection<IEdge<N, E>> searchContainer)
        {
            this.graph = graph;
            activeList = searchContainer;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Traverse the <typeparamref name="IGraph{N,E}"/> starting from the specified node
        /// until no more connected edges exist.
        /// </summary>
        /// <param name="startingNode">The node to start the traversal from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// <typeparamref name="IEdge{N,E}"/>.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the startingNode.</remarks>
        public IEnumerable<IEdge<N, E>> TraverseGraph(N startingNode)
        {
            Reset();
            return ResumeTraverseGraph(startingNode);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Restarts the traversal.
        /// </summary>
        protected void Reset()
        {
            activeList.Clear();
            visited.Clear();
        }
        /// <summary>
        /// Traverses any untouched graph nodes that are accessible from the specified node.
        /// </summary>
        /// <param name="startingNode">A new node to continue the search from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// <typeparamref name="IEdge{N,E}"/>.</returns>
        protected IEnumerable<IEdge<N, E>> ResumeTraverseGraph(N startingNode)
        {
            foreach (IEdge<N, E> edge in graph.OutEdges(startingNode))
            {
                activeList.Put(edge);
            }
            while (activeList.Count > 0)
            {
                IEdge<N, E> currentEdge = activeList.GetNext();
                N currentNode = currentEdge.To;
                if (!visited.Contains(currentNode))
                {
                    yield return currentEdge;
                    visited.Add(currentNode);
                    foreach (IEdge<N, E> edge in graph.OutEdges(currentNode))
                    {
                        if (!visited.Contains(edge.To))
                        {
                            activeList.Put(edge);
                        }
                    }
                }
            }
        }
        #endregion

        #region Member variables
        private IGraph<N, E> graph;
        private IPriorityCollection<IEdge<N, E>> activeList;
        private List<N> visited = new List<N>();
        #endregion
    }
}