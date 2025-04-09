using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BitStreamNS;

class BitStream(Stream stream, int bitCount = 8)
{
    internal readonly Stream _stream = stream;
    private string _currentByte = "";
    private int _index = 8;
    private readonly int _bitCount = bitCount;
    public bool EndOfStream => _stream.Position == _stream.Length && _index == 8;

    public byte ReadBits(int count)
    {
        byte toReturn = ReadBit();
        for (int i = 1; i < count; i++)
        {
            byte result = ReadBit();
            toReturn <<= 1;
            toReturn |= result;
        }
        return toReturn;
    }

    public byte ReadBit()
    {
        if (_index >= 8)
        {
            _currentByte = Convert.ToString(_stream.ReadByte(), 2).PadLeft(8, '0');
            _index = 8 - _bitCount;
        }
        return byte.Parse(_currentByte[_index++].ToString());
    }

    public byte ReadByte()
    {
        return ReadBits(8);
    }

    public T Read<T>() where T : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = ReadByte();
        }
        return MemoryMarshal.Read<T>(bytes);
    }

    //Keep this?
    public unsafe T ReadUnsafe<T>() where T : unmanaged
    {
        int len = Unsafe.SizeOf<T>();
        byte* ptr = stackalloc byte[len];
        for (int i = 0; i < len; i++)
        {
            ptr[i] = ReadByte();
        }
        return *(T*)ptr;
    }

    public IEnumerable<T> ReadMany<T>(long count) where T : unmanaged
    {
        for (long i = 0; i < count; i++)
        {
            Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
            for (int j = 0; j < bytes.Length; j++)
            {
                bytes[j] = ReadByte();
            }
            yield return MemoryMarshal.Read<T>(bytes);
        }
    }
}