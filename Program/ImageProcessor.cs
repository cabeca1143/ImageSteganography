using BitStreamNS;
using ExtensionsNS;
using System.Text;

namespace ImageProcessorNS;

public static class ImageProcessor
{
    public static void Decoder(string imagePath, byte bitCount)
    {
        Console.WriteLine("Loading Image...");
        Image<Rgb24> image = Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Converting Image...");
        Stream imageStream = image.ToStream();
        BitStream stream = new(imageStream, bitCount);
        imageStream.Position = 0;

        int extensionLength = stream.ReadInt();
        string fileExtension = stream.ReadString(extensionLength);
        int bytesToRead = stream.ReadInt();

        Console.WriteLine("Writing Data...");

        FileStream fileStream = File.OpenWrite($"{Path.GetDirectoryName(imagePath)}/Output.{fileExtension}");
        for (int i = 0; i < bytesToRead; i++)
        {
            fileStream.WriteByte(stream.ReadBits(8));
        }

        fileStream.Close();
        imageStream.Close();

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }

    public static void Encoder(string imagePath, string dataPath, byte bitCount)
    {
        BitStream reader;
        MemoryStream stream = new();
        BinaryWriter writer = new(stream);

        Console.WriteLine("Reading Image...");
        Image<Rgb24> image = Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Reading Data...");
        byte[] fileData = File.ReadAllBytes(dataPath);
        string fileExtension = Path.GetExtension(dataPath).Remove(0, 1);

        writer.Write(fileExtension.Length);
        stream.Write(Encoding.ASCII.GetBytes(fileExtension));
        writer.Write(fileData.Length);
        stream.Write(fileData);
        reader = new(stream, 8);
        stream.Position = 0;

        Console.WriteLine("Writing data to image...");
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (reader.EndOfStream)
                {
                    Console.WriteLine("End of Stream Reached!");
                    y = image.Height;
                    break;
                }

                Rgb24 newColors = new()
                {
                    R = reader.EndOfStream ? image[x, y].R : image[x, y].R.OverrideBits(reader.ReadBits(bitCount), bitCount),
                    G = reader.EndOfStream ? image[x, y].G : image[x, y].G.OverrideBits(reader.ReadBits(bitCount), bitCount),
                    B = reader.EndOfStream ? image[x, y].B : image[x, y].B.OverrideBits(reader.ReadBits(bitCount), bitCount),
                };
                image[x, y] = newColors;
            }
        }

        if (!reader.EndOfStream)
        {
            Console.WriteLine("End of stream not reached! Try increasing the BitCount or the Image Size!");
        }

        string? outputPath = Path.GetDirectoryName(dataPath);

    getInput:
        Console.WriteLine("Which format do you want to save as?");
        Console.WriteLine("1) PNG");
        Console.WriteLine("2) BMP");
        Console.WriteLine("3) Both");
        if (!int.TryParse(Console.ReadLine(), out int input))
        {
            Console.WriteLine("Invalid Input! Please use only numbers!");
            Thread.Sleep(1500);
            Console.Clear();
            goto getInput;
        }

        switch (input)
        {
            case 1:
                image.SaveAsPng($"{outputPath}/ImageOutput.png");
                break;
            case 2:
                image.SaveAsBmp($"{outputPath}/ImageOutput.bmp");
                break;
            case 3:
                image.SaveAsPng($"{outputPath}/ImageOutput.png");
                image.SaveAsBmp($"{outputPath}/ImageOutput.bmp");
                break;
            default:
                Console.WriteLine("Invalid Option! Please use numbers between 1 and 3!");
                Console.Clear();
                goto getInput;
        }

        image.Dispose();
        writer.Close();

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }
}
