using System;
using System.IO;


namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOrigin = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\ImageFile.jpg";

            StreamReader OriginFile = new StreamReader(File.Open(pathOrigin, FileMode.Open));

            HuffmanTree.Compressor(OriginFile, pathOrigin);
        }
    }
}
