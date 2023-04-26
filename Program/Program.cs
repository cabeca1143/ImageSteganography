using ImageProcessorNS;

internal class Program
{
    private static void Main()
    {
        byte bitCount;
        do
        {
            Console.Write("BitCount(1-8): ");
            _ = byte.TryParse(Console.ReadLine(), out bitCount);
        }
        while (!(bitCount >= 1 || bitCount <= 8));

        Console.Clear();

        while (true)
        {
        loopStart:
            Console.WriteLine("Select an Option:");
            Console.WriteLine("1) Encode Data in an Image");
            Console.WriteLine("2) Decode Data in an Image");
            Console.WriteLine("3) Exit");

            if (!int.TryParse(Console.ReadLine(), out int input))
            {
                Console.WriteLine("Invalid input! Please use only numbers!");
                Thread.Sleep(1500);
                Console.Clear();
                goto loopStart;
            }

            string? imagePath;
            switch (input)
            {
                case 1:
                    Console.Clear();
                    Console.Write("Image Path: ");
                    imagePath = Console.ReadLine()?.Replace("\"", string.Empty);
                    Console.Write("Data Path: ");
                    string? dataFilePath = Console.ReadLine()?.Replace("\"", string.Empty);

                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"Image \"{imagePath}\" does not exist! Please double-check the path!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        break;
                    }
                    if (!File.Exists(dataFilePath))
                    {
                        Console.WriteLine($"File \"{dataFilePath}\" does not exist! Please double-check the path!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        break;
                    }

                    Console.Clear();
                    ImageProcessor.Encoder(imagePath, dataFilePath, bitCount);
                    break;
                case 2:
                    Console.Clear();
                    Console.Write("Image Path: ");
                    imagePath = Console.ReadLine()?.Replace("\"", string.Empty);
                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine($"Image \"{imagePath}\" does not exist! Please double-check the path!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        break;
                    }

                    Console.Clear();
                    ImageProcessor.Decoder(imagePath, bitCount);
                    break;
                case 3:
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
}