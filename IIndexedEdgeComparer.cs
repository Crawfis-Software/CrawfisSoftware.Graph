using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Provides a common interface to compare edge (or path) costs in a graph
    /// search, as well as a method to fix (or close) the costs as nodes in
    /// the graph are visited.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IIndexedEdgeCostComparer<E> : IComparer<IIndexedEdge<E>>
    {
        /// <summary>
        /// As edges are added to a minimum (or maximum) path search using the IComparer derived
        /// from by a concrete implementation of this interface, the method allows the
        /// control to signal the comparer that it should update its cost logic to
        /// include this edge as a minimum edge in the minimum path tree.
        /// </summary>
        /// <param name="edge">The edge being added to the minimum path tree.</param>
        void UpdateCost(IIndexedEdge<E> edge);
    }
}