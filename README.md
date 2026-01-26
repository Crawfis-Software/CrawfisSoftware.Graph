# CrawfisSoftware.Graph

Algorithms and traversal helpers for graph data structures.

This package is intentionally focused on **graph algorithms** (traversal, queries, connectivity, shortest paths) and assumes you already have (or will implement) concrete graph types that satisfy the graph interfaces used throughout the library.

## Target frameworks

- `netstandard2.1`
- `net8.0`

## Namespaces

Most types are in `CrawfisSoftware.Collections.Graph`.

## Core concepts

The library works with two families of graph abstractions:

1. **Node-labeled graphs** via `IGraph<N, E>`
   - Nodes are addressed by their label type `N`.
   - Edges carry a label/weight type `E`.

2. **Index-based graphs** via `IIndexedGraph<N, E>`
   - Nodes are addressed by `int` indices (often `0..NumberOfNodes-1`).
   - You still can have node labels of type `N`, and edge labels/weights of type `E`.

Many algorithms have both an `IGraph` and `IIndexedGraph` version.

### Edge types

- `IEdge<N, E>` represents an edge in an `IGraph<N, E>`.
- `IIndexedEdge<E>` represents an edge in an `IIndexedGraph<N, E>`.

The algorithms typically use:

- `edge.From` / `edge.To` for endpoints
- `edge.Value` (or similar) for the edge label/weight (depends on the edge interface)

### Traversal order

`TraversalOrder` controls whether nodes are emitted:

- `TraversalOrder.PreOrder` (typically “when discovered”)
- `TraversalOrder.PostOrder` (typically “after children are processed”; implemented via recursion + depth-first behavior)

## Quick start

Add a reference to the project/package, then import:

- `using CrawfisSoftware.Collections.Graph;`

You will also need a concrete graph implementation that implements `IGraph<N,E>` or `IIndexedGraph<N,E>`.

## Traversal

### High-level traversal extension methods

For `IGraph<N,E>`:

- `BreadthFirstTraversalNodes(start)`
- `DepthFirstTraversalNodes(start)`
- `BreadthFirstTraversalEdges(start)`
- `DepthFirstTraversalEdges(start)`

For `IIndexedGraph<N,E>`:

- `BreadthFirstTraversalNodes(startIndex)`
- `DepthFirstTraversalNodes(startIndex)`
- `BreadthFirstTraversalNodesWithEdges(startIndex)`
- `DepthFirstTraversalNodesWithEdges(startIndex)`
- `BreadthFirstTraversalEdges(startIndex, isUndirected: true)`
- `DepthFirstTraversalEdges(startIndex, isUndirected: true)`
- Dijkstra traversals (see below)

Example (index-based BFS over nodes):

```csharp
using CrawfisSoftware.Collections.Graph;

IIndexedGraph<string, int> graph = /* your graph */;

foreach (int nodeIndex in graph.BreadthFirstTraversalNodes(startingIndex: 0))
{
    // nodeIndex are visited in BFS order
}
```

Example (node-labeled DFS returning edges used to reach nodes):

```csharp
using CrawfisSoftware.Collections.Graph;

IGraph<string, float> graph = /* your graph */;

foreach (IEdge<string, float> edge in graph.DepthFirstTraversalEdges("A"))
{
    // edge is the tree edge used to discover edge.To
}
```

### Low-level enumerators

Use these when you need more control than the extension methods provide.

- `GraphEnumerator<N,E>`: enumerates nodes in an `IGraph<N,E>`
- `IndexedGraphEnumerator<N,E>`: enumerates node indices in an `IIndexedGraph<N,E>`
- `GraphEdgeEnumerator<N,E>`: enumerates discovery edges in an `IGraph<N,E>`
- `IndexedGraphEdgeEnumerator<N,E>`: enumerates discovery edges or *all reachable edges* in an `IIndexedGraph<N,E>`

Common patterns:

```csharp
var walker = new GraphEnumerator<string, int>(graph);
walker.TraversalOrder = TraversalOrder.PostOrder;

foreach (var node in walker.TraverseNodes())
{
    // ...
}
```

`GraphEnumerator` / `IndexedGraphEnumerator` also expose `Components` to iterate each connected component of a traversal.

## Graph queries

### Acyclic checks

`GraphQuery` includes:

- `IsAcyclic()` for `IGraph<N,E>`
- `IsAcyclic()` for `IIndexedGraph<N,E>`
- `IsAcyclicUndirected()` for `IIndexedGraph<N,E>`

Notes:

- The acyclic check uses a post-order traversal/topological ordering idea.
- For undirected graphs, the undirected routine treats “back edges” as cycles.

Example:

```csharp
using CrawfisSoftware.Collections.Graph;

bool ok = graph.IsAcyclic();
```

### Topological sort

`GraphQuery.GetTopologicalSort()` returns an `IEnumerable` of nodes (or node indices) in a topological ordering.

```csharp
foreach (var node in graph.GetTopologicalSort())
{
    // node in topo order
}
```

### Size queries

`GraphQuery` also includes extension methods:

- `NumberOfNodes()`
- `NumberOfEdges()`

These methods work even for graphs that are not `IFiniteGraph`, and include overloads with `MaxNodes` / `MaxEdges` to guard against very large or infinite enumerations.

## Connectivity

`Connectivity<N,E>` provides component and strong-component queries.

For `IGraph<N,E>`:

- `NumberOfComponents(graph)`
- `ConnectedComponents(graph, MaxNodes)`

For `IIndexedGraph<N,E>`:

- `NumberOfComponents(graph)`
- `ConnectedComponents(graph, MaxNodes)`
- `NumberOfStronglyConnectedComponents(graph, graphTranspose, MaxNodes)`
- `StronglyConnectedComponents(graph, graphTranspose, MaxNodes)`

Note for strongly connected components:

- You must supply the transpose graph (`graphTranspose`).
- The algorithm assumes the graph/node ordering is compatible with the topological sort routine that is used internally.

Example:

```csharp
using CrawfisSoftware.Collections.Graph;

int componentCount = Connectivity<string, int>.NumberOfComponents(graph);
```

## Paths

### Unweighted path (node-labeled `IGraph`)

`PathQuery<N,E>.FindPath(graph, start, target)` performs a breadth-first style search (via a queue-based edge enumerator) and returns the edges of a discovered path.

```csharp
using CrawfisSoftware.Collections.Graph;

IEnumerable<IEdge<string, int>> pathEdges = PathQuery<string, int>.FindPath(graph, "A", "Z");
foreach (var e in pathEdges)
{
    // e.From -> e.To
}
```

### Weighted shortest path (index-based `IIndexedGraph`)

`PathQuery<N,E>.FindPath(indexedGraph, startIndex, targetIndex, ...)` uses Dijkstra-style traversal and returns the edges on the resulting minimum-cost path.

You can provide a cost function (`EdgeCostDelegate<E>`) to map edge labels to `float` costs:

```csharp
using CrawfisSoftware.Collections.Graph;

IIndexedGraph<string, int> graph = /* your graph */;

var path = PathQuery<string, int>.FindPath(
    graph,
    start: 0,
    target: 10,
    costDelegate: e => e.Value // int weight -> float cost
);

foreach (var e in path)
{
    // e.From -> e.To
}
```

## Dijkstra traversal helpers

`IndexedGraphTraversalExtensions` includes Dijkstra-based traversals that are useful when you want to enumerate reachable nodes/edges in best-first order while tracking cumulative path cost.

Key methods:

- `DijkstraTraversalNodes(startIndex, costDelegate)` returns `(cellIndex, pathCost)`
- `DijkstraTraversalNodesWithEdges(startIndex, costDelegate)` returns `(edge, fromPathCost, toPathCost)`
- `DijkstraTraversalEdges(startIndex, costDelegate, isUndirected: true)` returns `(edge, fromPathCost, toPathCost)`

There are also convenience overloads for common edge-label types such as `int`, `long`, `float`, `double`, and `bool`.

Example:

```csharp
using CrawfisSoftware.Collections.Graph;

foreach (var (nodeIndex, cost) in graph.DijkstraTraversalNodes(startingIndex: 0, costDelegate: e => e.Value))
{
    // nodeIndex reached with current best-known cost
}
```

## Precomputed shortest paths

### Single-source shortest paths

`SourceShortestPaths<N,E>` computes shortest paths from a single source node index to all reachable nodes.

- Construct it once.
- Query target costs with `GetCost(targetIndex)`.
- Get the path to a specific target with `GetPath(targetIndex)`.

```csharp
using CrawfisSoftware.Collections.Graph;

var sssp = new SourceShortestPaths<string, int>(
    graph,
    startingNode: 0,
    costDelegate: e => e.Value
);

float costTo10 = sssp.GetCost(10);
IEnumerable<IIndexedEdge<int>> pathTo10 = sssp.GetPath(10);
```

### All-pairs shortest path costs

`AllPairsShortestPath<N,E>` computes a cost matrix of shortest path costs between all nodes.

```csharp
using CrawfisSoftware.Collections.Graph;

var apsp = new AllPairsShortestPath<string, int>(graph, e => e.Value);
float cost = apsp.GetPathCost(fromNodeIndex: 0, toNodeIndex: 10);

// Optionally get a sorted list of (from,to,cost)
var sorted = apsp.GetSortedCosts(ascending: true, undirectedGraph: true);
```

Note: `AllPairsShortestPath` computes costs; if you need actual edge sequences for a specific pair, use `PathQuery.FindPath(...)`.

## Eulerian circuits

`EulerianCircuit.FindCircuit(graph, startingIndex)` returns an enumeration of `IIndexedEdge<E>` forming a circuit discovered from the given start index.

```csharp
using CrawfisSoftware.Collections.Graph;

foreach (var e in graph.FindCircuit(startingIndex: 0))
{
    // e is an edge in the circuit order
}
```

## Sorting / topological wrapper

`GraphSortWrapper<N,E>` is an `IGraph<N,E>` implementation that wraps an existing graph and yields `Nodes` in topological order.

Note: the constructor is not `public` in the current implementation, so it is not directly instantiable from another assembly. If you control this repository and intended it to be public API, consider making the constructor `public` (or adding a factory method) in a future change.

## Notes and gotchas

- Many algorithms assume node indices are valid keys in the graph’s `Nodes` enumeration.
- Several methods use `MaxNodes` / `MaxEdges` to guard against graphs that may be very large or effectively infinite.
- Some routines distinguish between:
  - traversing **nodes** (visiting each reachable node once)
  - traversing **edges** (enumerating all reachable edges; for `IIndexedGraph` this is exposed via `TraverseEdges` and related extension methods)

## Development

- Build: `dotnet build`

