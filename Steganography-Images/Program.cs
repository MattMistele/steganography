using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;

namespace Steganography_Images
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Encode or decode an image? ");
                string input = Console.ReadLine();

                if (input == "encode")
                {
                    Console.Write("Image filename: ");
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Console.ReadLine());
                    Console.Write("Message to encode: ");
                    string message = Console.ReadLine();

                    Bitmap newImage = encodeImage(new Bitmap(imagePath), message);
                    newImage.Save(Path.Combine(Directory.GetCurrentDirectory(), "encoded.png"));
                    Console.WriteLine("Encoded image saved!");
                    Console.WriteLine();
                }
                else if (input == "decode")
                {
                    Console.Write("Image filename: ");
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Console.ReadLine());
                    string message = decodeImage(new Bitmap(imagePath));
                    Console.WriteLine();
                    Console.WriteLine("Decoded message: " + message);
                    Console.WriteLine();
                }
            }
        }

        static Bitmap encodeImage(Bitmap image, string message)
        {
            BitArray bits = new BitArray(Encoding.UTF8.GetBytes(message));
            int currentBit = 0;
            bool done = false;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int red = 0, green = 0, blue = 0;
                    //Console.Write("Pixel [" + x + ", " + y + "]: ");

                    if (currentBit <= bits.Length - 3)
                    {
                        red = bits[currentBit] == true ? pixel.R | 0b00000001 : pixel.R & 0b11111110;
                        green = bits[currentBit + 1] == true ? pixel.G | 0b00000001 : pixel.G & 0b11111110;
                        blue = bits[currentBit + 2] == true ? pixel.B | 0b00000001 : pixel.B & 0b11111110;
                        if (currentBit == bits.Length - 3) done = true;
                        //Console.WriteLine("R: " + bits[currentBit] + " G: " + bits[currentBit + 1] + " B: " + bits[currentBit + 2]);
                    } else if (currentBit == bits.Length - 2)
                    {
                        red = bits[currentBit] == true ? pixel.R | 0b00000001 : pixel.R & 0b11111110;
                        green = bits[currentBit + 1] == true ? pixel.G | 0b00000001 : pixel.G & 0b11111110;
                        blue = pixel.B;
                        done = true;
                        //Console.WriteLine("R: " + bits[currentBit] + " G: " + bits[currentBit + 1]);
                    } else if (currentBit == bits.Length - 1)
                    {
                        red = bits[currentBit] == true ? pixel.R | 0b00000001 : pixel.R & 0b11111110;
                        green = pixel.G;
                        blue = pixel.B;
                        done = true;
                        //Console.WriteLine("R: " + bits[currentBit]);
                    }

                    currentBit += 3;
                    Color newPixel = Color.FromArgb(red, green, blue);
                    //Console.WriteLine("R: " + newPixel.R + " G " + newPixel.G + " B " + newPixel.B);
                    //Console.WriteLine();
                    image.SetPixel(x, y, newPixel);
                    if (done) goto next;
                }
            }

        next:
            return image;
        }

        static string decodeImage(Bitmap image)
        {
            BitArray bits = new BitArray(700);
            int currentBit = 0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    //Console.Write("Pixel [" + x + ", " + y + "]: ");

                    bits[currentBit] = (pixel.R & (1 << 0)) != 0;
                    bits[currentBit + 1] = (pixel.G & (1 << 0)) != 0;
                    bits[currentBit + 2] = (pixel.B & (1 << 0)) != 0;

                    //Console.WriteLine("R: " + bits[currentBit] + " G: " + bits[currentBit + 1] + " B: " + bits[currentBit + 2]);
                    //Console.WriteLine("R: " + pixel.R + " G " + pixel.G + " B " + pixel.B);
                    //Console.WriteLine();
                    currentBit += 3;
                    if (currentBit >= 700 - 1) goto next;
                }
            }

        next:
            byte[] bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
    }
}
