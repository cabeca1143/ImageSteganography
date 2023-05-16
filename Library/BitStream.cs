namespace BitStreamNS;

class BitStream
{
    internal readonly Stream _stream;
    private string _currentBinary = "";
    private int _index = 8;
    private readonly int _bitCount = 8;
    public bool EndOfStream => _stream.Position == _stream.Length && _index == 8;
    public BitStream(Stream stream, int bitCount, string? extension = null)
    {
        _stream = stream;
        _bitCount = bitCount;
    }

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
            _currentBinary = Convert.ToString(_stream.ReadByte(), 2);
            while (_currentBinary.Length < 8)
            {
                _currentBinary = '0' + _currentBinary;
            }
            _index = 8 - _bitCount;
        }
        return byte.Parse(_currentBinary[_index++].ToString());
    }

    public int ReadInt()
    {
        byte[] bytes = new byte[4]
        {
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8)
        };

        return BitConverter.ToInt32(bytes);
    }

    public long ReadLong()
    {
        byte[] bytes = new byte[8]
{
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8),
            ReadBits(8)
        };

        return BitConverter.ToInt64(bytes);
    }

    public string ReadString(int size)
    {
        string toReturn = "";
        for (int i = 0; i < size; i++)
        {
            toReturn += (char)ReadBits(8);
        }
        return toReturn;
    }
}
