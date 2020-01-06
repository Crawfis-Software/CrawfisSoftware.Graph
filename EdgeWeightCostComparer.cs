using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// A concrete implementation of the interface <typeparamref name="IIndexedEdgeCostComparer{E}"/>.
    /// Compares edge (or path) costs in a graph search, as well as a method
    /// to fix (or close) the costs as nodes in the graph are visited.
    /// </summary>
    /// <typeparam name="N">The type of the node labels in the corresponding graph.</typeparam>
    /// <typeparam name="E">The type of the edge labels in the corresponding graph.</typeparam>
    /// <seealso cref="GraphQuery{N,E}.FindPath(IIndexedGraph{N,E}, int, int, IIndexedEdgeCostComparer{E})"/>
    public class EdgeWeightCostComparer<N, E> : IComparer<IIndexedEdge<E>>
    {
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
            E costX = x.Value;
            E costY = y.Value;
            int state =  Comparer<E>.Default.Compare(costX, costY);
            return state;
        }
        #endregion
    }
}