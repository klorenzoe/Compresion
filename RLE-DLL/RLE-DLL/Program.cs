using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace RLE_DLL
{
    //Entiendo que esto va en un proyecto aparte   


    //        DLL
    //	-COMPRIMIR
    //	-DESCOMPRIMIR
    //	-RAZON DE COMPRESION
    //	-FACTOR DE COMPRESION
    //	-RUN LENGTH

    //APLICACION
    //	-ABRIR ARCHIVOS
    //	-LEER CONTENIDO
    //	-LEER INSTRUCCIONES
    //	-INTERFAZ


    class Program
    {
        static void Main(string[] args)
        {
            string[] entrada = Console.ReadLine().Split(' ');
            int current = CaracterValido(0, entrada);
            switch (entrada[current])
            {
                case "-c":
                    Comprimir(entrada[CaracterValido(current + 1, entrada)]);
                    break;
                case "-d":
                    Descomprimir(entrada[CaracterValido(current + 1, entrada)]);
                    break;
                case "--ayuda":
                    Console.WriteLine("Comprimir -c\nDscomprimir -d\nRuta de archivo -f\nAyuda --ayuda");
                    break;
                default:
                    Console.WriteLine("Comando no válido.\nEscriba --ayuda para obtener ayuda");
                    break;
            }
            Console.ReadKey();
        }

        static public int CaracterValido(int posicion, string[] entrada)
        {
            if (entrada[posicion] == "")
                return CaracterValido(posicion + 1, entrada);
            return posicion;
        }

        static public void Comprimir(string entrada)
        {
            Run_Length run = new Run_Length();
            string[] comando = entrada.Split('"');
            int current = CaracterValido(0, comando);

            if (String.Compare(comando[current], "-f") == 1)
            {
                Console.WriteLine("Sintaxis no válida\nEscriba --ayuda para obtener ayuda");
                return;
            }
                
            current = CaracterValido(current, comando);
            run.Comprimir(comando[current]);
        }

        static public void Descomprimir(string entrada)
        {
            Run_Length run = new Run_Length();
            string[] comando = entrada.Split('"');
            int current = CaracterValido(0, comando);

            if (String.Compare(comando[current], "-f") == 1)
            {
                Console.WriteLine("Sintaxis no válida\nEscriba --ayuda para obtener ayuda");
                return;
            }

            current = CaracterValido(current, comando);
            run.Descomprimir(comando[current]);
        }
    }
}
