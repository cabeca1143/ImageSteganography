using BitStreamNS;
using ExtensionsNS;
using System.IO.Compression;
using System.Text;
using SixLabors.ImageSharp.Formats.Png;
public static class ImageProcessor
{
    public static void Decoder(string imagePath, byte bitCount)
    {
        Console.WriteLine("Loading Image...");
        Image<Rgb24> image = Image.Load<Rgb24>(new(), File.OpenRead(imagePath));

        Console.WriteLine("Converting Image...");
        using (Stream imageStream = image.ToStream())
        {
            BitStream stream = new(imageStream, bitCount);

            //Read Length of the encoded file's extension
            int extensionLength = stream.ReadInt();
            //Read the actual extension
            string fileExtension = stream.ReadString(extensionLength);
            //Read the ammount of bytes
            long bytesToRead = stream.ReadLong();

            Console.WriteLine("Writing Data...");

            using FileStream fileStream = File.OpenWrite($"{Path.GetDirectoryName(imagePath)}/Output.{fileExtension}");
            for (long i = 0; i < bytesToRead; i++)
            {
                fileStream.WriteByte(stream.ReadBits(8));
            }
        }

        image.Dispose();

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }

    public static void Encoder(string imagePath, string dataPath, byte bitCount)
    {
        string? outputPath = Path.GetDirectoryName(imagePath);
        string extension = Path.GetExtension(dataPath).Remove(0, 1);

        MemoryStream stream = new();
        BitStream reader = new(stream, 8);

        //Read Image
        Task<Image<Rgb24>> imageTask = LoadImageAsync(imagePath);

        using (BinaryWriter br = new(stream, Encoding.UTF8, true))
        {
            //Write length of extension
            br.Write(BitConverter.GetBytes(extension.Length));

            //Write actual extension
            stream.Write(Encoding.ASCII.GetBytes(extension));

            Console.WriteLine("Oppening File...");
            using FileStream fileData = File.OpenRead(dataPath);
            if (fileData.Length > int.MaxValue)
            {
                br.Close();
                stream.Close();
                fileData.Close();
                imageTask.Dispose();

                Console.WriteLine($"Data files can't be greater than {int.MaxValue} bytes!");
                Thread.Sleep(3000);
                return;
            }

            //Writes how many bytes the file has
            br.Write(BitConverter.GetBytes(fileData.Length));

            //Writes the actual file data
            LoadDataFileAsync(fileData, stream).Wait();
            stream.Position = 0;
        }

        Image<Rgb24> image = imageTask.Result;
        imageTask.Dispose();

        Console.WriteLine("Writing data to image...");
        int height = image.Height;
        int width = image.Width;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Override old pixel
                Rgb24 rgb = image[x, y];
                image[x, y] = new()
                {
                    R = rgb.R.GetImageByte(reader, bitCount),
                    G = rgb.G.GetImageByte(reader, bitCount),
                    B = rgb.B.GetImageByte(reader, bitCount)
                };

                //Checks if the end of stream has been reached (All data has been written)
                if (reader.EndOfStream)
                {
                    Console.WriteLine("End of Stream Reached!");
                    goto getInput;
                }
            }
        }
        Console.WriteLine("End of stream not reached! Try increasing the BitCount or the Image Size!");
        Console.WriteLine($"{stream.Length - stream.Position} bytes remain!");

    getInput:
        Console.WriteLine("Which format do you want to save as?");
        Console.WriteLine("1) PNG");
        Console.WriteLine("2) BMP");
        Console.WriteLine("3) Both");
        switch (Console.ReadLine())
        {
            case "1":
                File.Delete($"{outputPath}/ImageOutput.png");
                image.SaveAsPng(($"{outputPath}/ImageOutput.png"));
                break;
            case "2":
                File.Delete($"{outputPath}/ImageOutput.bmp");
                image.SaveAsBmp(File.OpenWrite($"{outputPath}/ImageOutput.bmp"));
                break;
            case "3":
                File.Delete($"{outputPath}/ImageOutput.png");
                File.Delete($"{outputPath}/ImageOutput.bmp");
                image.SaveAsPng(($"{outputPath}/ImageOutput.png"));
                image.SaveAsBmp(($"{outputPath}/ImageOutput.bmp"));
                break;
            default:
                Console.Clear();
                Console.WriteLine("Invalid Option! Please use numbers between 1 and 4!");
                goto getInput;
        }

        image.Dispose();
        stream.Close();
    }

    private static async Task<Image<Rgb24>> LoadImageAsync(string path)
    {
        Console.WriteLine("Loading Image...");
        Image<Rgb24> image = await Image.LoadAsync<Rgb24>(path);
        Console.WriteLine("Image Loaded!");
        return image;
    }

    private static async Task LoadDataFileAsync(FileStream file, Stream toCopyTo)
    {
        Console.WriteLine("Writing file data to Stream...");
        await file.CopyToAsync(toCopyTo);
        Console.WriteLine("Data copied to Strem!");
    }
}
