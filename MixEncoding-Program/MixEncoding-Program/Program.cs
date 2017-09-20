using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunLengthEncoding;
using HuffmanEncoding;
namespace MixEncoding_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Compresión > ");
                string[] entrada = Console.ReadLine().Split(' ');
                if (entrada[0] == "--ayuda")
                {
                    Console.WriteLine("-h Comprime un archivo usando Huffman.\n-r Comprime un archivo usando RLE. \n-d Descomprime un archivo comprimido por RLE o por Huffman.\n-f Indica la ruta del archivo a comprimir o descomprimir.\n--ayuda  Muestra este mensaje.");
                }
                else if (entrada.Length < 3)
                {
                    Console.WriteLine("Se esperaban 3 argumentos: -h o -r o -d, -f, la ruta del archivo.\nEscriba --ayuda para ver la guía de uso.");
                }
                else
                {
                    switch (entrada[0])
                    {
                        case "-h":
                            if (entrada[1] == "-f")
                            {
                                ComprimirHuffman(entrada[2]);
                            }
                            else
                            {
                                Console.WriteLine("Se esperaba -f\nEscriba --ayuda para obtener ayuda");
                            }
                            break;
                        case "-r":
                            if (entrada[1] == "-f")
                            {
                                ComprimirRLE(entrada[2]);
                            }
                            else
                            {
                                Console.WriteLine("Se esperaba -f\nEscriba --ayuda para obtener ayuda");
                            }
                            break;
                        case "-d":
                            if (entrada[1] == "-f")
                            {
                                Descomprimir(entrada[2]);
                            }
                            else
                            {
                                Console.WriteLine("Se esperaba -f\nEscriba --ayuda para obtener ayuda");
                            }
                            break;
                        default:
                            Console.WriteLine("Comando no válido.\nEscriba --ayuda para obtener ayuda");
                            break;
                    }
                }
            }
        }

        static public void ComprimirRLE(string entrada)
        {
            RLE run = new RLE();
            Console.Write("Comprimiendo con RLE...");
            Console.WriteLine(run.Comprimir(entrada));
        }

        static public void ComprimirHuffman(string entrada)
        {
            Console.Write("Comprimiendo con Huffman...");
            Huffman.Compressor(entrada);
            Console.Write("Compresión finalizada.");
        }

        static public void Descomprimir(string entrada)
        {
            RLE run = new RLE();

            if (run.CheckFile(entrada))
            {
                Console.Write("Descomprimiendo...");
                Console.WriteLine(run.Descomprimir(entrada));
            }
            else
            {
                Console.Write("Descomprimiendo...");
                Huffman.Descompressor(entrada);
            }
        }
    }
}
