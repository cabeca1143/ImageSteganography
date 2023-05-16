class Encoder
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0]))
        {
            Console.WriteLine("Invalid Image path! Please Drag and Drop the image to decode on the Executable!");
            _ = Console.ReadKey();
            return;
        }

    getDataPath:
        Console.Write("Data Path: ");
        string? dataPath = Console.ReadLine();
        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"File \"{dataPath}\" does not exist! Please double check the path!");
            Thread.Sleep(2000);
            Console.Clear();
            goto getDataPath;
        }

    getInput:
        Console.Write("Bit Size (1-8): ");
        if (!byte.TryParse(Console.ReadLine(), out byte bitCount) || bitCount is > 8 or < 1)
        {
            Console.WriteLine("Invalid input!  Please use numbers between 1 and 8!");
            Thread.Sleep(2000);
            Console.Clear();
            goto getInput;
        }

        ImageProcessor.Encoder(args[0], dataPath, bitCount);
    }
}