using BitStreamNS;
using ExtensionsNS;
using System.Text;

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

        //Read Length of the encoded file's extension
        int extensionLength = stream.ReadInt();
        //Read the actual extension
        string fileExtension = stream.ReadString(extensionLength);
        //Read the ammount of bytes
        long bytesToRead = stream.ReadLong();

        Console.WriteLine("Writing Data...");

        FileStream fileStream = File.OpenWrite($"{Path.GetDirectoryName(imagePath)}/Output.{fileExtension}");
        for (long i = 0; i < bytesToRead; i++)
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
        //Read Image
        Image<Rgb24> image = Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Reading Data...");
        //Reads data to be stored inside image
        byte[] fileData = File.ReadAllBytes(dataPath);
        string fileExtension = Path.GetExtension(dataPath).Remove(0, 1);

        //Wirte binary-int for the text length of the data file extension to the stream
        writer.Write(fileExtension.Length);
        //Write actual extension to the stream
        stream.Write(Encoding.ASCII.GetBytes(fileExtension));
        //Write in binary-long ammount of bytes of data to store to the stream
        writer.Write(fileData.LongLength);
        //Write actual bytes to the stream
        stream.Write(fileData);
        //Instanciate BitStream
        reader = new(stream, 8);
        //Set stream cursor position to 0
        stream.Position = 0;

        Console.WriteLine("Writing data to image...");
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                //Checks if the end of stream has been reached (All data has been written)
                if (reader.EndOfStream)
                {
                    Console.WriteLine("End of Stream Reached!");
                    y = image.Height;
                    break;
                }

                //Instanciate new pixel to override the original image pixel
                Rgb24 newColors = new()
                {
                    R = GetImageByte(reader, reader.EndOfStream, image[x, y].R, bitCount),
                    G = GetImageByte(reader, reader.EndOfStream, image[x, y].G, bitCount),
                    B = GetImageByte(reader, reader.EndOfStream, image[x, y].B, bitCount),
                };
                //Override old pixel
                image[x, y] = newColors;
            }
        }

        if (!reader.EndOfStream)
        {
            Console.WriteLine("End of stream not reached! Try increasing the BitCount or the Image Size!");
        }

        string? outputPath = Path.GetDirectoryName(imagePath);

    getInput:
        Console.WriteLine("Which format do you want to save as?");
        Console.WriteLine("1) PNG");
        Console.WriteLine("2) BMP");
        Console.WriteLine("3) Both");
        Console.WriteLine("4) Discard");
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
            case 4:
                break;
            default:
                Console.WriteLine("Invalid Option! Please use numbers between 1 and 4!");
                Console.Clear();
                goto getInput;
        }

        image.Dispose();
        writer.Close();
        stream.Close();

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }

    private static byte GetImageByte(BitStream stream, bool toOverride, byte originalByte, byte bitCount)
    {
        return toOverride ? originalByte : originalByte.OverrideBits(stream.ReadBits(bitCount), bitCount);
    }
}
