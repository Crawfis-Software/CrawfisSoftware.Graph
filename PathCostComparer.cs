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
    public class PathCostComparer<N, E> : IIndexedEdgeCostComparer<E>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graph">The <typeparamref name="IIndexedGraph{N,E}"/> that will be 
        /// used in the path finding algorithms.</param>
        /// <param name="heuristicAcceleration">A coefficient to accelerate more towards the target using the heuristic.</param>
        public PathCostComparer(IIndexedGraph<N, E> graph, float heuristicAcceleration = 1)
        {
            _graph = graph;
            _heuristicAcceleration = heuristicAcceleration;
            _edgeCostDelegate = new EdgeCostDelegate<E>(GetEdgeCost);
        }
        #endregion

        /// <summary>Clear out any prior computations. Set the starting node and target node.</summary>
        /// <param name="startNode">A starting node in the path.</param>
        /// <param name="target">(Optional) A target node that can be used to accelerate the search using A*. The method TargetHeuristic needs to be overridden for A*.</param>
        public void Initialize(int startNode, int target = -1)
        {
            _targetNode = target;
            InitializeCosts(startNode);
        }
        /// <inheritdoc/>
        public float PathCost(int targetNode)
        {
            return _costs[targetNode];
        }

        /// <summary>
        /// Set the cost of a particular node. Used and needed when starting a new search.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="cost"></param>
        public void SetCost(int startNode, float cost)
        {
            _costs[startNode] = cost;
        }

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
        private void InitializeCosts(int startNode)
        {
            // TODO Allow this to be lazily allocated and initialized.
            _costs = new Dictionary<int,float>(_graph.NumberOfNodes);

            foreach (int node in _graph.Nodes)
                _costs[node] = float.MaxValue;
            _costs[startNode] = 0.0f;
        }

        /// <summary>
        /// Returns the cost of the path if this edge is added.
        /// Derived classes should override this method to provide their own updates.
        /// </summary>
        /// <param name="edge">The edge to add to the path cost.</param>
        /// <returns>A <c>float</c> as the path cost.</returns>
        protected float GetPathCost(IIndexedEdge<E> edge)
        {
            return _costs[edge.From] + _edgeCostDelegate(edge) + _heuristicAcceleration * TargetHeuristic(edge.To);
        }

        /// <summary>
        /// Returns the cost of the edge for this comparer.
        /// Derived classes should override this method to provide their own updates.
        /// </summary>
        /// <param name="edge">The edge to get the cost from.</param>
        /// <returns>A <c>float</c> as the edge cost.</returns>
        protected virtual float GetEdgeCost(IIndexedEdge<E> edge)
        {
            return 1.0f;
        }

        /// <summary>
        /// Returns an estimated cost to reach a target cell.
        /// </summary>
        /// <param name="cellIndex">The current cell index.</param>
        /// <returns>An estimated (and conservative) path cost from the current cell to the target cell.</returns>
        /// <remarks>Override this method to enable A* search.</remarks>
        /// <remarks>Default is a constant (0), which is not conservative, but results in Dijkstra's path search.</remarks>
        protected virtual float TargetHeuristic(int cellIndex)
        {
            return 0;
        }
        #endregion

        #region Member Variables
        /// <summary>
        /// The graph.
        /// </summary>
        protected IIndexedGraph<N, E> _graph;
        /// <summary>
        /// A value to speed up the A* search.
        /// </summary>
        protected float _heuristicAcceleration;
        /// <summary>
        /// The target node.
        /// </summary>
        protected int _targetNode;
        /// <summary>
        /// The function to call to get an edge cost.
        /// </summary>
        protected EdgeCostDelegate<E> _edgeCostDelegate;
        /// <summary>
        /// A dictionary of edge indices to path costs.
        /// </summary>
        protected IDictionary<int,float> _costs;
        #endregion
    }
}