using System.Collections.Generic;

namespace CrawfisSoftware.Collections.Graph
{
    /// <summary>
    /// Static class to find Eulerian Circuits.
    /// </summary>
    public static class EulerianCircuit
    {
        /// <summary>
        /// Output the Euler Circuit from the starting index.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <param name="graph">The graph to traverse.</param>
        /// <param name="startingIndex">The starting graph index.</param>
        /// <returns>An enumerable of the edges used to reach each node.</returns>
        public static IEnumerable<IIndexedEdge<E>> FindCircuit<N, E>(this IIndexedGraph<N, E> graph, int startingIndex)
        {
            //var gridEnumerator = new IndexedGraphEdgeEnumerator<N, E>(graph, new StackAdaptor<IIndexedEdge<E>>());
            int lastNode = startingIndex;
            var stack = new Stack<IIndexedEdge<E>>();
            foreach(var edge in graph.DepthFirstTraversalEdges(startingIndex))
            {
                while(edge.From != lastNode)
                {
                    var visitedEdge = stack.Pop();
                    yield return visitedEdge;
                    // Pop stack until stack.Peek().To == edge.From
                    lastNode = visitedEdge.From;
                }
                lastNode = edge.To;
                stack.Push(edge);
            }
            while(stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }
    }
}