using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;

namespace StegoNEW
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get the image to decode from the user 
            Console.Write("Image filename: ");
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Console.ReadLine());

            // Decode and print the message!
            string message = decodeImage(new Bitmap(imagePath));
            Console.WriteLine("\n Decoded message: " + message);
        }      

        static string decodeImage(Bitmap image)
        {
            BitArray bits = new BitArray(801); // keeps track of binary values
            int index = 0; // keeps track of the current bit

            // Loop through every pixel in the image
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // The magic happens here:
                    // For each RGB byte in the color, save the least signifigant bit
                    bits[index] = (pixel.R & (1 << 0)) != 0;
                    bits[index + 1] = (pixel.G & (1 << 0)) != 0;
                    bits[index + 2] = (pixel.B & (1 << 0)) != 0;
                    index += 3;

                    Console.WriteLine("R: " + pixel.R + " G: " + pixel.G + " B: " + pixel.B);
                   
                    if (index > 800) goto next; // Exit the loop if we've hit 800 bits (100 characters) 
                }
            }

        next:
            // Convert our bits to bytes
            byte[] bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);

            // Get a message from the bytes! 
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
