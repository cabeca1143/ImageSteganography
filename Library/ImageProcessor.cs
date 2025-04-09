using BitStreamNS;
using ExtensionsNS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImageProcessorNS;

public static class ImageProcessor
{
    public static void Decoder(string imagePath, byte bitCount)
    {
        Console.WriteLine("Loading Image...");

        using Image<Rgb24> image = Image.Load<Rgb24>(new(), imagePath);
        byte[] bytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgb24>()];
        image.CopyPixelDataTo(bytes);

        Console.WriteLine("Converting Image...");

        using (Stream imageStream = new MemoryStream(bytes))
        {
            BitStream stream = new(imageStream, bitCount);

            //Read Length of the encoded file's extension
            int nameLength = stream.Read<int>();

            //Read the actual extension
            string fileName = new([.. stream.ReadMany<char>(nameLength)]);

            //Read the ammount of bytes
            long bytesToRead = stream.Read<long>();

            //Read data
            Console.WriteLine("Writing Data...");
            using FileStream fileStream = File.OpenWrite($"{Path.GetDirectoryName(imagePath)}/{fileName}");
            foreach (byte b in stream.ReadMany<byte>(bytesToRead))
            {
                fileStream.WriteByte(b);
            }
        }

        Console.WriteLine("Done!");
        Thread.Sleep(2000);
        Console.Clear();
    }

    public static void Encoder(string imagePath, string dataPath, byte bitCount)
    {
        using Task<Image<Rgb24>> imageTask = LoadImageAsync(imagePath);
        using MemoryStream stream = new();
        string? outputPath = Path.GetDirectoryName(imagePath);
        string fileName = Path.GetFileName(dataPath);
        byte[] pixelData;
        int width;
        int height;

        BitStream reader = new(stream);

        //Populates a stream with all data to be encoded
        if (!PopulateStream(stream, fileName, dataPath))
        {
            return;
        }

        //Get Pixel bytes and encodes the desired data into them
        using (Image<Rgb24> originalImage = imageTask.Result)
        {
            Console.WriteLine("Writing data to image...");

            width = originalImage.Width;
            height = originalImage.Height;
            pixelData = new byte[originalImage.Width * originalImage.Height * Unsafe.SizeOf<Rgb24>()];

            originalImage.CopyPixelDataTo(pixelData);
            for (int i = 0; i < pixelData.Length && !reader.EndOfStream; i++)
            {
                pixelData[i] = pixelData[i].GetImageByte(reader, bitCount);
            }

            if (reader.EndOfStream)
            {
                Console.WriteLine("End of Stream Reached!");
            }
            else
            {
                Console.WriteLine("End of stream not reached! Try increasing the BitCount or the Image Size!");
                Console.WriteLine($"{stream.Length - stream.Position} bytes remain!");
            }
        }

        //Final Encoded Image
        using Image<Rgb24> image = Image.LoadPixelData<Rgb24>(pixelData, width, height);
        while (true)
        {
            Console.WriteLine("Which format do you want to save as?");
            Console.WriteLine("1) PNG");
            Console.WriteLine("2) BMP");
            Console.WriteLine("3) Both");

            string pngName = $"{outputPath}/ImageOutput.png";
            string bmpName = $"{outputPath}/ImageOutput.bmp";
            switch (Console.ReadLine())
            {
                case "1":
                    File.Delete(pngName);
                    image.SaveAsPng(pngName);
                    return;
                case "2":
                    File.Delete(bmpName);
                    image.SaveAsBmp(bmpName);
                    return;
                case "3":
                    File.Delete(pngName);
                    File.Delete(bmpName);
                    image.SaveAsPng(pngName);
                    image.SaveAsBmp(bmpName);
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid Option! Please use numbers between 1 and 4!");
                    continue;
            }
        }
    }

    private static bool PopulateStream(Stream stream, string fileName, string path)
    {
        using BinaryWriter br = new(stream, Encoding.Unicode, true);

        //Write length of extension
        br.Write(BitConverter.GetBytes(fileName.Length));

        //Write actual extension
        stream.Write(Encoding.Unicode.GetBytes(fileName));

        Console.WriteLine("Oppening File...");
        using FileStream fileData = File.OpenRead(path);
        if (fileData.Length > int.MaxValue)
        {
            br.Close();
            stream.Close();
            fileData.Close();

            Console.WriteLine($"Data files can't be greater than {int.MaxValue} bytes!");
            Thread.Sleep(3000);
            return false;
        }

        //Writes how many bytes the file has
        br.Write(BitConverter.GetBytes(fileData.Length));

        //Writes the actual file data
        Console.WriteLine("Writing file data to Stream...");
        fileData.CopyTo(stream);
        Console.WriteLine("Data copied to Stream!");

        stream.Position = 0;

        return true;
    }

    private static async Task<Image<Rgb24>> LoadImageAsync(string path)
    {
        Console.WriteLine("Loading Image...");
        Image<Rgb24> image = await Image.LoadAsync<Rgb24>(path);
        Console.WriteLine("Image Loaded!");
        return image;
    }
}