﻿using System;
using System.Text;
using System.IO;


namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOrigin = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\txtFile.txt";
            StreamReader OriginFile = new StreamReader(File.Open(pathOrigin, FileMode.Open));
            HuffmanTree.Compressor(OriginFile, pathOrigin);

            string pathToDescompress = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\COMPRIMIDO txtFile.txt";
            HuffmanTree.Descompressor(pathToDescompress);
        }
    }
}
