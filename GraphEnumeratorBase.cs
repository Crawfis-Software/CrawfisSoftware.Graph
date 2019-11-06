using System;

namespace OhioState.Collections.Graph
{
    /// <summary>
    /// Specifies the order in which visited nodes or edges should be output.
    /// </summary>
    /// <remarks>A <paramref name="TraversalOrder"/> of <paramref name="PostOrder"/> 
    /// requires a recursive and depth-first (Stack-based) traversal.</remarks>
    public enum TraversalOrder {
        /// <summary>
        /// The nodes are output (possibly) before their children have been processed.
        /// </summary>
        PreOrder,
        /// <summary>
        /// The nodes (or edges) are output once all of their children have been 
        /// processed (unless their are cycles).
        /// </summary>
        PostOrder };

    /// <summary>
    /// An abstract base class for the graph enumeration classes.
    /// </summary>
    public abstract class GraphEnumeratorBase
    {
        #region Public interface
        /// <summary>
        /// Specifies the order in which visited nodes or edges should be output.
        /// </summary>
        public TraversalOrder TraversalOrder
        {
            get { return traversal; }
            set { traversal = value; }
        }
        /// <summary>
        /// Specifies the number of restarts used in the iteration. A restart occurs when all
        /// of the nodes reachable from the node used in the previous restart is complete, but
        /// their are still some nodes who have not been visited. For graph whose nodes are sorted in
        /// topological order, this property represents the current Connected-component and once
        /// all nodes have been traversed is equal to the number of Connected-components in the 
        /// graph.
        /// </summary>
        /// <seealso cref="Connectivity{N, E}.NumberOfComponents(IGraph{N,E})"/>
        /// <seealso cref="Connectivity{N, E}.NumberOfComponents(IIndexedGraph{N,E})"/>
        /// <seealso cref="Connectivity{N, E}.NumberOfStronglyConnectedComponents"/>
        public int CurrentComponent
        {
            get { return componentNumber; }
        }
        #endregion

        #region Member variables
        private TraversalOrder traversal = TraversalOrder.PreOrder;
        /// <summary>
        /// Represents the current component number that the iterator is currently at.
        /// </summary>
        protected int componentNumber = 0;
        #endregion
    }
}
