class Decoder
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0]))
        {
            Console.WriteLine("Invalid Image path! Please Drag and Drop the image to decode on the Executable!");
            Console.ReadKey();
            return;
        }

    getInput:
        Console.Write("Bit Size (1-8): ");
        if (!byte.TryParse(Console.ReadLine(), out byte bitCount))
        {
            Console.WriteLine("Invalid input!");
            Console.ReadKey();
            return;
        }
        if (bitCount is > 8 or < 1)
        {
            Console.WriteLine("Invalid Number! Please use numbers between 1 and 8!");
            goto getInput;
        }

        ImageProcessor.Decoder(args[0], bitCount);
    }
}