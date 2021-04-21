using System;
using System.Collections;
using System.Collections.Generic;

public class Octree<T> where T : class
{
	OctreeNode<T> root;

	public Octree()
	{
		root = new OctreeNode<T>(new int[] {-1, -1, -1}, 2);
	}

	public T this[int x, int y, int z]
	{
		get
		{
			if (IsOutsideRoot(x, y, z)) return null;

			OctreeNode<T> currentNode = root;
			while (currentNode.size > 1)
			{
				if(currentNode.IsLeaf) return null;	// If a node bigger than 1 has no children, we've hit a dead end
				OctantIdentifier octant = currentNode.GetOctantFromPosition(x, y, z);
				currentNode = currentNode.subNodes[octant.index];
			}

			return currentNode.data;
		}
		set
		{
			while (IsOutsideRoot(x, y, z)) Grow();

			OctreeNode<T> currentNode = root;
			while (currentNode.size > 1)
			{
				OctantIdentifier octant = currentNode.GetOctantFromPosition(x, y, z);
				if(currentNode.IsLeaf) currentNode.Subdivide();
				currentNode = currentNode.subNodes[octant.index];
			}

			currentNode.data = value;
			Optimize();
		}
	}

	public void Optimize()
	{
		root.RemoveDeadEnds(true);
		Shrink(true);
	}

	public void DrawTree()
	{
		root.DrawBounds(true);
	}

	public int CountNodes()
	{
		int count = 1;
		root.CountSubNodes(ref count);
		return count;
	}

	bool IsOutsideRoot(int x, int y, int z)
	{
		bool more = x >= root.HalfSize || y >= root.HalfSize || z >= root.HalfSize;
		bool less = x < -root.HalfSize || y < -root.HalfSize || z < -root.HalfSize;

		return more || less;
	}

	void Grow()
	{
		root.Subdivide();
		List<OctreeNode<T>> oldSubNodes = root.subNodes;

		// Create new root
		int[] newPos = new int[] {
			root.position[0] - root.HalfSize,
			root.position[1] - root.HalfSize,
			root.position[2] - root.HalfSize
		};
		root = new OctreeNode<T>(newPos, root.size * 2);
		root.Subdivide(2);
		
		// Reassign inner octants
		for (int i = 0; i < 8; i++)
		{
			OctantIdentifier octant = new OctantIdentifier(i);
			OctreeNode<T> subNode = root.subNodes[octant.index];
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
			if (!subNode.IsLeaf) outerHaveChildren = true; // Outer node has children, can't shrink
		});
		if (outerHaveChildren) return;

		// We can shrink, first subdivide twice
		root.Subdivide(2);

		// Find inner subNodes
		List<OctreeNode<T>> innerSubNodes = new List<OctreeNode<T>>();
		ForEachInnerRootSubNode(subNode =>
		{
			innerSubNodes.Add(subNode);
		});

		// Create new half sized root and assign old inner subNodes as root children
		int[] newPos = new int[] {
			root.position[0] + root.HalfSize / 2,
			root.position[1] + root.HalfSize / 2,
			root.position[2] + root.HalfSize / 2
		};
		root = new OctreeNode<T>(newPos, root.size / 2);
		root.subNodes = innerSubNodes;

		// Remove dead ends caused by subdivision performed earlier
		root.RemoveDeadEnds();

		// Shrink further
		if (recursively) Shrink(true);
	}

	public void ForEachInnerRootSubNode(Action<OctreeNode<T>> callback)
	{
		ForEachSecondOrderRootSubNode(callback);
	}

	public void ForEachOuterRootSubNode(Action<OctreeNode<T>> callback)
	{
		ForEachSecondOrderRootSubNode(null, callback);
	}

	public void ForEachSecondOrderRootSubNode(Action<OctreeNode<T>> innerNodesCallback = null, Action<OctreeNode<T>> outerNodesCallback = null)
	{
		if (!root.IsLeaf)
		{
			for (int i = 0; i < 8; i++)
			{
				OctantIdentifier octant = new OctantIdentifier(i);
				OctreeNode<T> subNode = root.subNodes[octant.index];

				if (!subNode.IsLeaf)
				{
					for (int j = 0; j < 8; j++)
					{
						OctreeNode<T> secondOrderSubNode = subNode.subNodes[j];
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
	}
}