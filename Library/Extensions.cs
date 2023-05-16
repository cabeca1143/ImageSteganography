using BitStreamNS;

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

    internal static Stream ToStream(this Image<Rgb24> image)
    {
        FileStream stream = File.Open("BufferStream.buf", FileMode.Create, FileAccess.ReadWrite);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                stream.WriteByte(image[x, y].R);
                stream.WriteByte(image[x, y].G);
                stream.WriteByte(image[x, y].B);
            }
        }

        stream.Position = 0;
        return stream;
    }

    internal static byte GetImageByte(this byte originalByte, BitStream stream, byte bitCount)
    {
        return stream.EndOfStream ? originalByte : originalByte.OverrideBits(stream.ReadBits(bitCount), bitCount);
    }
}
