using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class OctreeNode<T>
{
	public T data;
	public List<OctreeNode<T>> subNodes = new List<OctreeNode<T>>();
	public bool IsLeaf => subNodes.Count == 0;

	public int[] position;
	public int size;
	public int HalfSize => size / 2;

	public OctreeNode(int[] position, int size)
	{
		this.position = position;
		this.size = size;
	}

	public void Subdivide(int amount = 1)
	{
		if (IsLeaf && size > 1)
		{
			for (int i = 0; i < 8; i++)
			{
				int[] position = GetOctantPosition(new OctantIdentifier(i));
				subNodes.Add(new OctreeNode<T>(position, size / 2));
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
		if (IsLeaf) return;

		// Simplify recursively
		if (recursively)
		{
			subNodes.ForEach(subNode => subNode.RemoveDeadEnds(true));
		}

		// If any subNode has children or data, exit
		foreach(OctreeNode<T> subNode in subNodes)
		{
			if (!subNode.IsLeaf || subNode.data != null) return;
		}

		// All subNodes are empty, simplify this node
		subNodes.Clear();
	}

	public OctantIdentifier GetOctantFromPosition(int x, int y, int z)
	{
		OctantIdentifier octant = new OctantIdentifier();
		if (x >= position[0] + HalfSize) octant.bits[0] = true;
		if (y >= position[1] + HalfSize) octant.bits[1] = true;
		if (z >= position[2] + HalfSize) octant.bits[2] = true;

		return octant;
	}

	public void DrawBounds(bool recursively = false)
	{
		Vector3 center = new Vector3(
			position[0] + (size / 2f),
			position[1] + (size / 2f),
			position[2] + (size / 2f)
		);
		Vector3 sizeVector = new Vector3(size, size, size);

		// Here you can implement debug drawing depending
		// on your graphic environment. Draw a box based on two parameters:
		// center - center position in global space
		// sizeVector - half extent of the box
		// DebugDraw.DrawBox(center, sizeVector, new Color("00ff00"));

		if (recursively)
		{
			subNodes.ForEach(subNode => subNode.DrawBounds(true));
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

	int[] GetOctantPosition(OctantIdentifier octant)
	{
		return new int[] {
			position[0] + ((octant.bits[0] ? 1 : 0) * HalfSize),
			position[1] + ((octant.bits[1] ? 1 : 0) * HalfSize),
			position[2] + ((octant.bits[2] ? 1 : 0) * HalfSize)
		};
	}
}