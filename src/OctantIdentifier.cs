using System;
using System.Collections;

public class OctantIdentifier
{
	public BitArray bits = new BitArray(3);
	public int index => ToNumeral(bits);
	public int x => bits[0] ? 1 : 0;
	public int y => bits[1] ? 1 : 0;
	public int z => bits[2] ? 1 : 0;

	public OctantIdentifier() {}

	public OctantIdentifier(int index)
	{
		bits = new BitArray(new[] { index });
	}

	public OctantIdentifier(BitArray bits)
	{
		this.bits = bits;
	}

	public OctantIdentifier Inverse()
	{
		BitArray inverse = new BitArray(3);
		inverse[0] = !bits[0];
		inverse[1] = !bits[1];
		inverse[2] = !bits[2];

		return new OctantIdentifier(inverse);
	}

	int ToNumeral(BitArray binary)
	{
		var result = new int[1];
		binary.CopyTo(result, 0);
		return result[0];
	}
}