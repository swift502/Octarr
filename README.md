# C# Cubic octree

Dynamic cubic octree in for class data, written C#. Based on the C++ [Cubic-octree](https://github.com/markusgod/cubic-octree).

Basically a generic endless 3D array, which can be accessed like regular arrays and grows and shrinks depending on how much space needs to be allocated.

Unlike multidimensional or jagged arrays, an octree is memory friendly. You can write a data block at the `[2^32, 2^32, 2^32]` position and not run out of memory. It will then take 32 octree node lookups (logarithmic complexity) to find that data, or anything near it.

```cs
// Usage
public Octree<Thing> octree = new Octree<Thing>();	// Create an octree
octree[10, 20, 30] = new Thing();		// Write
Thing thing = octree[10, 20, 30];		// Read assigned, returns your object
Thing thing = octree[1, 2, 3];			// Read unassigned, returns null

// Debug
int nodeCount = octree.CountNodes();	// Count all octree nodes
octree.DrawTree();		// Draws the octree, but first you must add a box drawing function specific to your graphic environment in the OctreeNode.DrawBounds function
```