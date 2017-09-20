using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunLengthEncoding;
namespace RunLengthEnconding_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Compresión RLE > ");
                string[] entrada = Console.ReadLine().Split(' ');
                if (entrada[0] == "--ayuda")
                {
                    Console.WriteLine("-c Comprime un archivo usando RLE.\n-d Descomprime un archivo comprimido por RLE.\n-f Indica la ruta del archivo a comprimir o descomprimir.\n--ayuda  Muestra este mensaje.");
                }
                else if (entrada.Length < 3)
                {
                    Console.WriteLine("Se esperaban 3 argumentos: -c o -d, -f, la ruta del archivo.\nEscriba --ayuda para ver la guía de uso.");
                }
                else
                {
                    switch (entrada[0])
                    {
                        case "-c":
                            if (entrada[1] == "-f")
                            {
                                Comprimir(entrada[2]);
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

        static public void Comprimir(string entrada)
        {
           RLE run = new RLE();
            Console.Write("Comprimiendo...");
            Console.WriteLine(run.Comprimir(entrada));
        }

        static public void Descomprimir(string entrada)
        {
            RLE run = new RLE();
            Console.Write("Descomprimiendo...");
            Console.WriteLine(run.Descomprimir(entrada));
        }
    }
}
