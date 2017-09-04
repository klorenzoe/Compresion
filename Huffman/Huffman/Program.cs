using System;
using System.Text;
using System.IO;


namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOrigin = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\descarga.txt";
            StreamReader OriginFile = new StreamReader(File.Open(pathOrigin, FileMode.Open), Encoding.GetEncoding("ANSI"));
            HuffmanTree.Compressor(OriginFile, pathOrigin);

            string pathToDescompress = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\COMPRIMIDO descarga.txt";
            HuffmanTree.Descompressor(pathToDescompress);
        }
    }
}
