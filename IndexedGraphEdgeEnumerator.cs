using System;
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
        /// until no more connected edges exist.
        /// </summary>
        /// <param name="startingNode">The node to start the traversal from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// <typeparamref name="IIndexedEdge{N,E}"/>.</returns>
        /// <remarks>This routine will only traverse those nodes reachable from 
        /// the startingNode.</remarks>
        public IEnumerable<IIndexedEdge<E>> TraverseGraph(int startingNode)
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
            if (visited == null)
            {
                visited = new bool[indexedGraph.NumberOfNodes];
            }
            else
            {
                for (int i = 0; i < indexedGraph.NumberOfNodes; i++)
                    visited[i] = false;
            }

            activeList.Clear();
        }
        /// <summary>
        /// Traverses any untouched graph nodes that are accessible from the specified node.
        /// </summary>
        /// <param name="startingNode">A new node to continue the search from.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/> of 
        /// <typeparamref name="IIndexedEdge{N,E}"/>.</returns>
        protected IEnumerable<IIndexedEdge<E>> ResumeTraverseGraph(int startingNode)
        {
            visited[startingNode] = true;
            foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(startingNode))
            {
                activeList.Put(edge);
            }
            while (activeList.Count > 0)
            {
                IIndexedEdge<E> currentEdge = activeList.GetNext();
                if (!visited[currentEdge.To])
                {
                    int currentNode = currentEdge.To;
                    yield return currentEdge;
                    visited[currentNode] = true;
                    foreach (IIndexedEdge<E> edge in indexedGraph.OutEdges(currentNode))
                    {
                        if (!visited[edge.To])
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
        private bool[] visited;
        #endregion
    }
}