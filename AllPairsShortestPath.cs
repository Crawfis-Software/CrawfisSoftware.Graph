using System;
using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Finds the minimum path costs between all nodes.
    /// </summary>
    /// <typeparam name="N">The graph node label type.</typeparam>
    /// <typeparam name="E">The edge label type.</typeparam>
    /// <remarks>This class only computes costs. To get paths, use PathQuery.FindPath(...).</remarks>
    public class AllPairsShortestPath<N, E>
    {
        private IIndexedGraph<N, E> graph;
        private float[,] costMatrix;
        private EdgeCostDelegate<E> costDelegate;
        /// <summary>
        /// Calculates the shortest paths for all reachable nodes in a graph
        /// from a specified initial starting node.
        /// </summary>
        /// <param name="graph">The indexed graph used for the search.</param>
        /// <param name="costDelegate">A function used to retrieve or calculate
        /// the cost for a given edge.</param>
        /// <remarks>Implements Dijkstra's algorithm.</remarks>
        public AllPairsShortestPath(IIndexedGraph<N, E> graph, EdgeCostDelegate<E> costDelegate)
        {
            this.graph = graph;
            ComputeCosts(graph, costDelegate);
        }

        /// <summary>
        /// Returns the shortest path cost between two nodes.
        /// </summary>
        /// <param name="fromNodeIndex">A node index.</param>
        /// <param name="toNodeIndex">A node index.</param>
        /// <returns>The total path cost between the nodes.</returns>
        public float GetPathCost(int fromNodeIndex, int toNodeIndex)
        {
            return costMatrix[fromNodeIndex, toNodeIndex];
        }

        /// <summary>
        /// Returns an enumeration of sorted path costs.
        /// </summary>
        /// <param name="ascending">If true, sort is minimum to maximum. If false, maximum to minimum.</param>
        /// <param name="undirectedGraph">If true, only edges (i,j) where j > i will be output.</param>
        /// <returns>An IEnumerable of tuples {nodeIndex,nodeIndex,cost} as {int,int,float}</returns>
        public IList<Tuple<int,int,float>> GetSortedCosts(bool ascending = true, bool undirectedGraph = false)
        {
            var sortedEdgeList = new List<Tuple<int, int, float>>(graph.NumberOfEdges);
            for(int row = 0; row < graph.NumberOfNodes; row++)
            {
                int start = 0;
                if (undirectedGraph) start = row + 1;
                for(; start < graph.NumberOfNodes; start++)
                {
                    sortedEdgeList.Add(new Tuple<int, int, float>(row, start, costMatrix[row, start]));
                }
            }
            if(ascending)
                sortedEdgeList.Sort(ascendingComparer);
            else
                sortedEdgeList.Sort(descendingComparer);

            return sortedEdgeList;
        }

        private int ascendingComparer(Tuple<int, int, float> x, Tuple<int, int, float> y)
        {
            return x.Item3.CompareTo(y.Item3);
        }

        private int descendingComparer(Tuple<int, int, float> x, Tuple<int, int, float> y)
        {
            return -x.Item3.CompareTo(y.Item3);
        }

        private void ComputeCosts(IIndexedGraph<N, E> graph, EdgeCostDelegate<E> costDelegate)
        {
            int numberOfNodes = graph.NumberOfNodes;
            costMatrix = new float[numberOfNodes, numberOfNodes];
            for (int row = 0; row < numberOfNodes; row++)
            {
                for (int column = 0; column < numberOfNodes; column++)
                {
                    for (int pass = 0; pass < numberOfNodes; pass++)
                    {
                        costMatrix[row, column] = Single.MaxValue;
                    }
                }
            }
            foreach(var edge in graph.Edges)
            {
                costMatrix[edge.From, edge.To] = costDelegate(edge);
            }

            for (int row = 0; row < numberOfNodes; row++)
            {
                for(int column = 0; column < numberOfNodes; column++)
                {
                    for(int pass = 0; pass < numberOfNodes; pass++)
                    {
                        float cost = costMatrix[row, pass] + costMatrix[pass, column];
                        if (cost < costMatrix[row,column] )
                        {
                            costMatrix[row, column] = cost;
                        }
                    }
                }
            }
        }
    }
}
