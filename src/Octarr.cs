using System;
using System.Numerics;
using System.Collections.Generic;

public class Octarr<T> where T : class
{
	OctarrNode<T> root;

	public Octarr()
	{
		root = new OctarrNode<T>(new BigInteger[] {-1, -1, -1}, 2);
	}

	public T this[BigInteger x, BigInteger y, BigInteger z]
	{
		get
		{
			if (IsOutsideRoot(x, y, z)) return null;

			OctarrNode<T> currentNode = root;
			while (currentNode.size > 1)
			{
				if(currentNode.IsDeadEnd) return null;	// If a node bigger than 1 has no children, we've hit a dead end
				OctantIdentifier octant = currentNode.GetOctantFromPosition(x, y, z);
				currentNode = currentNode.subNodes[octant.index];
			}

			return currentNode.data;
		}
		set
		{
			while (IsOutsideRoot(x, y, z)) Grow();

			OctarrNode<T> currentNode = root;
			while (currentNode.size > 1)
			{
				OctantIdentifier octant = currentNode.GetOctantFromPosition(x, y, z);
				if(currentNode.IsDeadEnd) currentNode.Subdivide();
				currentNode = currentNode.subNodes[octant.index];
			}

			currentNode.data = value;
			Optimize();
		}
	}

	public void DrawTree(OctarrNode<T>.DrawBox callback)
	{
		root.DrawBounds(callback, true);
	}

	public int GetNodeCount()
	{
		int count = 1;
		root.CountSubNodes(ref count);
		return count;
	}

	void Optimize()
	{
		root.RemoveDeadEnds(true);
		Shrink(true);
	}

	bool IsOutsideRoot(BigInteger x, BigInteger y, BigInteger z)
	{
		bool more = x >= root.HalfSize || y >= root.HalfSize || z >= root.HalfSize;
		bool less = x < -root.HalfSize || y < -root.HalfSize || z < -root.HalfSize;

		return more || less;
	}

	void Grow()
	{
		root.Subdivide();
		List<OctarrNode<T>> oldSubNodes = root.subNodes;

		// Create new root
		BigInteger[] newPos = new BigInteger[] {
			root.position[0] - root.HalfSize,
			root.position[1] - root.HalfSize,
			root.position[2] - root.HalfSize
		};
		root = new OctarrNode<T>(newPos, root.size * 2);
		root.Subdivide(2);
		
		// Reassign inner octants
		for (int i = 0; i < 8; i++)
		{
			OctantIdentifier octant = new OctantIdentifier(i);
			OctarrNode<T> subNode = root.subNodes[octant.index];
			subNode.subNodes[octant.Inverse().index] = oldSubNodes[octant.index];
		}
	}

	void Shrink(bool recursively = false)
	{
		if (root.size == 2) return;

		// Check if outer nodes have children
		bool outerHaveChildren = false;
		ForEachOuterRootSubNode(subNode =>
		{
			if (!subNode.IsDeadEnd) outerHaveChildren = true; // Outer node has children, can't shrink
		});
		if (outerHaveChildren) return;

		// We can shrink, first subdivide twice
		root.Subdivide(2);

		// Find inner subNodes
		List<OctarrNode<T>> innerSubNodes = new List<OctarrNode<T>>();
		ForEachInnerRootSubNode(subNode =>
		{
			innerSubNodes.Add(subNode);
		});

		// Create new half sized root and assign old inner subNodes as root children
		BigInteger[] newPos = new BigInteger[] {
			root.position[0] + (root.size / 4),
			root.position[1] + (root.size / 4),
			root.position[2] + (root.size / 4)
		};
		root = new OctarrNode<T>(newPos, root.size / 2);
		root.subNodes = innerSubNodes;

		// Remove dead ends caused by subdivision performed earlier
		root.RemoveDeadEnds();

		// Shrink further
		if (recursively) Shrink(true);
	}

	void ForEachInnerRootSubNode(Action<OctarrNode<T>> callback)
	{
		ForEachSecondOrderRootSubNode(callback);
	}

	void ForEachOuterRootSubNode(Action<OctarrNode<T>> callback)
	{
		ForEachSecondOrderRootSubNode(null, callback);
	}

	void ForEachSecondOrderRootSubNode(Action<OctarrNode<T>> innerNodesCallback = null, Action<OctarrNode<T>> outerNodesCallback = null)
	{
		for (int i = 0; i < root.subNodes.Count; i++)
		{
			OctantIdentifier octant = new OctantIdentifier(i);
			OctarrNode<T> subNode = root.subNodes[octant.index];

			for (int j = 0; j < subNode.subNodes.Count; j++)
			{
				OctarrNode<T> secondOrderSubNode = subNode.subNodes[j];
				if (j == octant.Inverse().index)
				{
					innerNodesCallback?.Invoke(secondOrderSubNode);
				}
				else
				{
					outerNodesCallback?.Invoke(secondOrderSubNode);
				}
			}
		}
	}
}