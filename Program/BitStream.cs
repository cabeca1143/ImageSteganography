using ExtensionsNS;

namespace BitStreamNS;

class BitStream
{
    private readonly Stream _stream;
    private byte _current;
    private string _currentBinary = "";
    private int _index = 8;
    public bool EndOfStream => _stream.Position == _stream.Length && _index == 8;
    public BitStream(Stream stream)
    {
        _stream = stream;
    }

    public byte ReadBits(int count, int bitCount = 8)
    {
        byte toReturn = ReadBit(bitCount);
        for (int i = 1; i < count; i++)
        {
            byte result = ReadBit(bitCount);
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

    public byte ReadBit(int count = 8)
    {
        if (_index >= 8)
        {
            _current = (byte)_stream.ReadByte();
            _currentBinary = Convert.ToString(_current, 2);
            while (_currentBinary.Length < 8)
            {
                _currentBinary = '0' + _currentBinary;
            }
            _index = 8 - count;
        }
        return byte.Parse(_currentBinary[_index++].ToString());
    }
}
