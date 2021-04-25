using System.Numerics;
using System.Collections.Generic;

public class OctarrNode<T>
{
	public T data;
	public List<OctarrNode<T>> subNodes = new List<OctarrNode<T>>();
	public bool IsDeadEnd => subNodes.Count == 0;

	public BigInteger[] position;
	public BigInteger size;
	public BigInteger HalfSize => size / 2;

	public OctarrNode(BigInteger[] position, BigInteger size)
	{
		this.position = position;
		this.size = size;
	}

	public void Subdivide(int amount = 1)
	{
		if (IsDeadEnd && size > 1)
		{
			for (int i = 0; i < 8; i++)
			{
				BigInteger[] position = GetOctantPosition(new OctantIdentifier(i));
				subNodes.Add(new OctarrNode<T>(position, size / 2));
			}
		}

		if (amount > 1)
		{
			subNodes.ForEach(subNode => subNode.Subdivide(amount - 1));
		}
	}

	public void RemoveDeadEnds(bool recursively = false)
	{
		// Check if already simplified
		if (IsDeadEnd) return;

		// Simplify recursively
		if (recursively)
		{
			subNodes.ForEach(subNode => subNode.RemoveDeadEnds(true));
		}

		// If any subNode has children or data, exit
		foreach(OctarrNode<T> subNode in subNodes)
		{
			if (!subNode.IsDeadEnd || subNode.data != null) return;
		}

		// All subNodes are empty, simplify this node
		subNodes.Clear();
	}

	public OctantIdentifier GetOctantFromPosition(BigInteger x, BigInteger y, BigInteger z)
	{
		OctantIdentifier octant = new OctantIdentifier();
		if (x >= position[0] + HalfSize) octant.bits[0] = true;
		if (y >= position[1] + HalfSize) octant.bits[1] = true;
		if (z >= position[2] + HalfSize) octant.bits[2] = true;

		return octant;
	}

	public delegate void DrawBox(
		float centerX, float centerY, float centerZ,
		float boxHalfExtent
	);

	public void DrawBounds(DrawBox callback, bool recursively = false)
	{
		callback(
			(float)position[0] + ((float)size / 2f),
			(float)position[1] + ((float)size / 2f),
			(float)position[2] + ((float)size / 2f),
			(float)size);

		if (recursively)
		{
			subNodes.ForEach(subNode => subNode.DrawBounds(callback, true));
		}
	}

	public void CountSubNodes(ref int count)
	{
		count += subNodes.Count;

		foreach (var subNode in subNodes)
		{
			subNode.CountSubNodes(ref count);
		}
	}

	BigInteger[] GetOctantPosition(OctantIdentifier octant)
	{
		return new BigInteger[] {
			position[0] + (octant.x * HalfSize),
			position[1] + (octant.y * HalfSize),
			position[2] + (octant.z * HalfSize)
		};
	}
}
