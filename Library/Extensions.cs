using BitStreamNS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ExtensionsNS;

public static class Extensions
{
    internal static byte OverrideBits(this byte variable, byte value, byte count) => (byte)((variable & 0xFF << count) | value);

    internal static byte Reverse(this byte value, int size)
    {
        byte reverse = 0;
        for (int bit = 0; bit < size; bit++)
        {
            reverse <<= 1;
            reverse |= (byte)(value & 1);
            value >>= 1;
        }

        return reverse;
    }

    internal static byte GetImageByte(this byte originalByte, BitStream stream, byte bitCount)
    {
        return originalByte.OverrideBits(stream.ReadBits(bitCount), bitCount);
    }
}