using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_DLL
{
    class Program
    {
        static void Main(string[] args)
        {
            Run_Length run = new Run_Length();
            string cadena = "ÁÁÁÁÁÁÁÁÁÁÁÁÁNBBBBBBBBBBBBNNNBBBBBBBBBBBBBBBBBBBBBBBBNBBBBBBBBBBBB";
            byte[]  compresion = run.Comprimir(cadena);
            string cadena2 = run.Descomprimir(compresion);
            Console.WriteLine(cadena.CompareTo(cadena2));
            Console.ReadKey();
        }
    }
}
