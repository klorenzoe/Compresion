using System;
using System.Text;
using System.IO;


namespace Huffman
{
    class Program
    {
        static void Main(string[] args)
        {

            //Para probarlo solo es de cambiarle la dirección: "@"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\txtFile.txt"
            //a la dirección donde esta el archivo a probar
            string pathOrigin = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\Computadora2.docx";
            
            HuffmanTree.Compressor(pathOrigin);


            //Acá es necesario Copiar la dirección del Path Origin, y antes del nombre del archivo escribirle "COMPRIMIDO "
            string pathToDescompress = @"C:\Users\Krle__000\Desktop\ARCHIVOS PRUEBA\COMPRIMIDO Computadora2.docx";
            HuffmanTree.Descompressor(pathToDescompress);
        }
    }
}
