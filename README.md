# Octarr

Stands for <b>oct</b>ree based 3D <b>arr</b>ay.

Octarr is a dynamic cubic octree capable of storing class data, accessed via indexers. Written in C#, based on the C++ [Cubic-octree](https://github.com/markusgod/cubic-octree).

Basically a generic endless 3D array, which can be accessed like regular arrays and grows and shrinks depending on how much space needs to be allocated. Location index can be positive or negative in any direction. Octree is centered around the zero coordinate (0, 0, 0), and grows and shrinks _from_ and _to_ this zero coordinate.

Unlike multidimensional or jagged arrays, an octree is memory friendly. You can write a data block at the `[2^64, 2^64, 2^64]` position and not run out of memory. It will then take roughly 64 octree node lookups (logarithmic complexity) to find that data, or anything near it. Octarr is internally using the [BigInteger](https://docs.microsoft.com/dotnet/api/system.numerics.biginteger) data type to allow for unconstrained data location.

```cs
// Usage
public Octarr<Thing> octarr = new Octarr<Thing>();	// Create an octarr
octarr[10, -20, 30] = new Thing();		// Write
Thing thing = octarr[10, -20, 30];		// Read assigned, returns your object
thing = octarr[1, 2, 3];			// Read unassigned, returns null

// Debug
int nodeCount = octarr.GetNodeCount();	// Count all octree nodes
octarr.DrawTree((float x, float y, float z, float halfSize) =>
{
	// Draw the octree using your own box drawing function
	DrawBox(x, y, z, halfSize);
});
```

## Limitations

Data is currently required to be classes. If you want to store structs (int, string, etc.) it'll require some simple modifications and a definition of a default data value other than null.
