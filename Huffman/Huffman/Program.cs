using System;
using System.IO;


namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOrigin = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\Descarga.jpeg";
            StreamReader OriginFile = new StreamReader(File.Open(pathOrigin, FileMode.Open));
            HuffmanTree.Compressor(OriginFile, pathOrigin);

            string pathToDescompress = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\COMPRIMIDO Descarga.jpeg";
            HuffmanTree.Descompressor(pathToDescompress);
        }
    }
}
