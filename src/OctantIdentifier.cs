using Godot;
using System;
using System.Collections;

public class OctantIdentifier
{
	public int index => ToNumeral(bits);
	public BitArray bits = new BitArray(3);

	public OctantIdentifier() {}

	public OctantIdentifier(int index)
	{
		bits = ToBinary(index);
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

	BitArray ToBinary(int numeral)
    {
        return new BitArray(new[] { numeral });
    }

    int ToNumeral(BitArray binary)
    {
        if (binary == null)
            throw new ArgumentNullException("binary");
        if (binary.Length > 32)
            throw new ArgumentException("must be at most 32 bits long");

        var result = new int[1];
        binary.CopyTo(result, 0);
        return result[0];
    }
}