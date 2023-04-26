using ExtensionsNS;

namespace BitStreamNS;

class BitStream
{
    private readonly Stream _stream;
    private byte _current;
    private string _currentBinary = "";
    private int _index = 8;
    private int _bitCount = 8;
    public bool EndOfStream => _stream.Position == _stream.Length && _index == 8;
    public BitStream(Stream stream, int bitCount)
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

        //Check
        if (!BitConverter.IsLittleEndian)
        {
            return toReturn.Reverse(count);
        }

        return toReturn;
    }

    public byte ReadBit()
    {
        if (_index >= 8)
        {
            _current = (byte)_stream.ReadByte();
            _currentBinary = Convert.ToString(_current, 2);
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
        //Check
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }

    public string ReadString(int size)
    {
        string toReturn = "";
        for(int i = 0; i < size; i++)
        {
            toReturn += (char)ReadBits(8);
        }
        return toReturn;
    }
}
