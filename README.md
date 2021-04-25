# OctreeArray

Dynamic cubic octree capable of storing class data, accessed via indexers. Written in C#, based on the C++ [Cubic-octree](https://github.com/markusgod/cubic-octree).

Basically a generic endless 3D array, which can be accessed like regular arrays and grows and shrinks depending on how much space needs to be allocated. Location index can be positive or negative in any direction. Octree is centered around the zero coordinate (0, 0, 0), and grows and shrinks _from_ and _to_ this zero coordinate.

Unlike multidimensional or jagged arrays, an octree is memory friendly. You can write a data block at the `[2^30, 2^30, 2^30]` position and not run out of memory. It will then take roughly 30 octree node lookups (logarithmic complexity) to find that data, or anything near it.

```cs
// Usage
public OctreeArray<Thing> octree = new OctreeArray<Thing>();	// Create an octree
octree[10, -20, 30] = new Thing();		// Write
Thing thing = octree[10, -20, 30];		// Read assigned, returns your object
thing = octree[1, 2, 3];			// Read unassigned, returns null

// Debug
int nodeCount = octree.GetNodeCount();	// Count all octree nodes
octree.DrawTree((float x, float y, float z, float halfSize) =>
{
	// Draw the octree using your own box drawing function
	DrawBox(x, y, z, halfSize);
});
```

## Limitations

Data position is currently limited by the Int32.MaxValue number. However OctreeNode positions could easily be changed to Int64s.

Data is currently required to be classes. It too can however be easily changed to structs, should you need to store integers, strings, ect. as data.
