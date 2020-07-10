using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Delegate type definition for computing the cost of a path if an edge is added.
    /// </summary>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    /// <param name="edge">The edge to compute the cost if added to the path.</param>
    /// <returns>A <c>float</c> as the edge cost.</returns>
    public delegate float EdgeCostDelegate<E>(IIndexedEdge<E> edge);

    /// <summary>
    /// A concrete implementation of the interface <typeparamref name="IIndexedEdgeCostComparer{E}"/>.
    /// Compares edge (or path) costs in a graph search, as well as a method
    /// to fix (or close) the costs as nodes in the graph are visited.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    /// <seealso cref="GraphQuery{N,E}.FindPath(IIndexedGraph{N,E}, int, int, IIndexedEdgeCostComparer{E})"/>
    public class EdgeCostComparer<N, E> : IIndexedEdgeCostComparer<E>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/> that will be 
        /// used in the path finding algorithms.</param>
        /// <param name="startNode">A starting node in the path.</param>
        public EdgeCostComparer(IIndexedGraph<N, E> graph, int startNode)
        {
            _graph = graph;
            _startNode = startNode;

            InitializeCosts();

            _edgeCostDelegate = new EdgeCostDelegate<E>(GetEdgeCost);
        }
        #endregion

        /// <summary>
        /// Get or set the <typeparamref name="EdgeCostDelegate{E}"/> function used to
        /// calculate the edge cost.
        /// </summary>
        public EdgeCostDelegate<E> EdgeCostDelegate
        {
            get { return _edgeCostDelegate; }
            set { _edgeCostDelegate = value; }
        }

        /// <summary>
        /// As edges are added to a minimum (or maximum) path search using the IComparer derived
        /// from by a concrete implementation of this interface, the method allows the
        /// control to signal the comparer that it should update its cost logic to
        /// include this edge as a minimum edge in the minimum path tree.
        /// </summary>
        /// <param name="edge">The edge being added to the path tree.</param>
        public void UpdateCost(IIndexedEdge<E> edge)
        {
            float cost = _costs[edge.From] + _edgeCostDelegate(edge);
            if (cost < _costs[edge.To])
                _costs[edge.To] = cost;

        }

        #region IComparer<IIndexedEdge<E>> Members
        /// <summary>
        /// Compares two edges and returns a value indicating 
        /// whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first edge to compare.</param>
        /// <param name="y">The second edge to compare.</param>
        /// <returns>A <typeparamref name="System.Int32"/>
        /// <value>Less than zero, if x is less than y</value>
        /// <value>Zero, if x is equal to y</value>
        /// <value>Greater than zero, if x is greater than y</value>
        /// </returns>
        public int Compare(IIndexedEdge<E> x, IIndexedEdge<E> y)
        {
            float costX = GetPathCost(x);
            float costY = GetPathCost(y);
            int state = Comparer<float>.Default.Compare(costX, costY);
            return state;
        }
        #endregion

        #region Implementation
        private void InitializeCosts()
        {
            // TODO Allow this to be lazily allocated and initialized.
            _costs = new List<float>(_graph.NumberOfNodes);

            for (int i = 0; i < _graph.NumberOfNodes; i++)
                _costs.Add(float.MaxValue);
            _costs[_startNode] = 0.0f;
        }
        /// <summary>
        /// Returns the cost of the edge for this comparer.
        /// Derived classes should override this method to provide their own updates.
        /// </summary>
        /// <param name="edge">The edge to get the cost from.</param>
        /// <returns>A <c>float</c> as the edge cost.</returns>
        protected float GetEdgeCost(IIndexedEdge<E> edge)
        {
            return 1.0f;
        }
        /// <summary>
        /// Returns the cost of the path if this edge is added.
        /// Derived classes should override this method to provide their own updates.
        /// </summary>
        /// <param name="edge">The edge to add to the path cost.</param>
        /// <returns>A <c>float</c> as the path cost.</returns>
        protected float GetPathCost(IIndexedEdge<E> edge)
        {
            return _costs[edge.From] + _edgeCostDelegate(edge);
        }
        #endregion

        #region Member Variables
        private IIndexedGraph<N, E> _graph;
        private int _startNode;
        private EdgeCostDelegate<E> _edgeCostDelegate;
        private IList<float> _costs;
        #endregion
    }
}