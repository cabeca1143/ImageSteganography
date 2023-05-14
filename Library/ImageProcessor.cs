using BitStreamNS;
using ExtensionsNS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

public static class ImageProcessor
{
    public static void Decoder(string imagePath, byte bitCount)
    {
        Console.WriteLine("Loading Image...");
        Image<Rgb24> image = SixLabors.ImageSharp.Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Converting Image...");
        Stream imageStream = image.ToStream();
        BitStream stream = new(imageStream, bitCount);

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
        image.Dispose();
        File.Delete(imagePath);

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }

    public static void Encoder(string imagePath, string dataPath, byte bitCount)
    {
        string? outputPath = Path.GetDirectoryName(imagePath);

        //Read Image
        Console.WriteLine("Reading Image...");
        Image<Rgb24> image = SixLabors.ImageSharp.Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Oppening File...");
        FileStream fileData = File.OpenRead(dataPath);
        /*if ((image.Height * image.Width * 3) / (bitCount / 8) > fileData.Length)
        {
            Console.WriteLine("Image size is too small for the ammount of data to store!");
            fileData.Close();
            image.Dispose();
            Thread.Sleep(2000);
            return;
        }*/
        BitStream reader = new(fileData, 8, Path.GetExtension(dataPath).Remove(0, 1));
        fileData.Position = 0;

        Console.WriteLine("Writing data to image...");

        int height = image.Height;
        int width = image.Width;
        for (int y = 0; y < height; y++)
        {
            //Checks if the end of stream has been reached (All data has been written)
            if (reader.EndOfStream)
            {
                Console.WriteLine("End of Stream Reached!");
                break;
            }

            for (int x = 0; x < width; x++)
            {
                //Override old pixel
                Rgb24 rgb = image[x, y];
                image[x, y] = new()
                {
                    R = rgb.R.GetImageByte(reader, reader.EndOfStream, bitCount),
                    G = rgb.G.GetImageByte(reader, reader.EndOfStream, bitCount),
                    B = rgb.B.GetImageByte(reader, reader.EndOfStream, bitCount)
                };
            }
        }
        if (!reader.EndOfStream)
        {
            Console.WriteLine("End of stream not reached! Try increasing the BitCount or the Image Size!");
            Console.WriteLine($"{fileData.Length - fileData.Position} bytes remain!");
        }

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
                File.Delete($"{outputPath}/ImageOutput.png");
                PngEncoder encoder = new();
                FileStream stream = File.OpenWrite($"{outputPath}/ImageOutput.png");
                encoder.Encode(image, stream);
                stream.Close();
                break;
            case 2:
                File.Delete($"{outputPath}/ImageOutput.bmp");
                image.SaveAsBmp(File.OpenWrite($"{outputPath}/ImageOutput.bmp"));
                break;
            case 3:
                File.Delete($"{outputPath}/ImageOutput.png");
                File.Delete($"{outputPath}/ImageOutput.bmp");
                image.SaveAsPng(File.OpenWrite($"{outputPath}/ImageOutput.png"));
                image.SaveAsBmp(File.OpenWrite($"{outputPath}/ImageOutput.bmp"));
                break;
            case 4:
                break;
            default:
                Console.WriteLine("Invalid Option! Please use numbers between 1 and 4!");
                Console.Clear();
                goto getInput;
        }

        image.Dispose();
        fileData.Close();
        File.Delete(outputPath + "Temp.temp");

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }
}
