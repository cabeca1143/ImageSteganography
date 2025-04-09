using ImageProcessorNS;

internal class Program
{
    private static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.Unicode;

        byte bitCount = GetBitCount();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select an Option:");
            Console.WriteLine("1) Encode Data in an Image");
            Console.WriteLine("2) Decode Data in an Image");
            Console.WriteLine($"3) Set BitCount (Cur: {bitCount})");
            Console.WriteLine("4) Exit");

            if (!int.TryParse(Console.ReadLine(), out int input))
            {
                Console.WriteLine("Invalid input! Please use only numbers!");
                Thread.Sleep(1500);
                Console.Clear();
                continue;
            }

            switch (input)
            {
                case 1:
                    HandleEncodeImage(bitCount);
                    break;
                case 2:
                    HandleDecodeImage(bitCount);
                    break;
                case 3:
                    bitCount = GetBitCount();
                    break;
                case 4:
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid Option! Please use only numbers between 1 and 3;");
                    Thread.Sleep(1500);
                    Console.Clear();
                    break;
            }
        }
    }

    static void HandleEncodeImage(byte bitCount)
    {
        string? imagePath;
        Console.Clear();

        Console.Write("Image Path: ");
        imagePath = Console.ReadLine()?.Replace("\"", string.Empty);
        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Image \"{imagePath}\" does not exist! Please double-check the path!");
            Thread.Sleep(1500);
            Console.Clear();
            return;
        }

        Console.Write("Data Path: ");
        string? dataFilePath = Console.ReadLine()?.Replace("\"", string.Empty);
        if (!File.Exists(dataFilePath))
        {
            Console.WriteLine($"File \"{dataFilePath}\" does not exist! Please double-check the path!");
            Thread.Sleep(1500);
            Console.Clear();
            return;
        }

        Console.Clear();
        ImageProcessor.Encoder(imagePath, dataFilePath, bitCount);
    }

    static void HandleDecodeImage(byte bitCount)
    {
        string? imagePath;
        Console.Clear();

        Console.Write("Image Path: ");
        imagePath = Console.ReadLine()?.Replace("\"", string.Empty);
        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Image \"{imagePath}\" does not exist! Please double-check the path!");
            Thread.Sleep(1500);
            Console.Clear();
            return;
        }

        Console.Clear();
        ImageProcessor.Decoder(imagePath, bitCount);
    }

    static byte GetBitCount()
    {
    getBitCount:
        Console.Clear();
        Console.Write("BitCount(1-8): ");
        if (!byte.TryParse(Console.ReadLine(), out byte bitCount) || bitCount is < 1 or > 8)
        {
            Console.WriteLine("Invalid Bit count! Please use numbers between 1 and 8!");
            Thread.Sleep(1500);
            Console.Clear();
            goto getBitCount;
        }
        return bitCount;
    }
}