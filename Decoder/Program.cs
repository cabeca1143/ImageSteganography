using ImageProcessorNS;

class Decoder
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0]))
        {
            Console.WriteLine("Invalid Image path! Please Drag and Drop the image to decode on the Executable!");
            _ = Console.ReadKey();
            return;
        }

    getInput:
        Console.Write("Bit Size (1-8): ");
        if (!byte.TryParse(Console.ReadLine(), out byte bitCount) || bitCount is > 8 or < 1)
        {
            Console.WriteLine("Invalid input! Please use numbers between 1 and 8!");
            Thread.Sleep(2000);
            Console.Clear();
            goto getInput;
        }

        ImageProcessor.Decoder(args[0], bitCount);
    }
}