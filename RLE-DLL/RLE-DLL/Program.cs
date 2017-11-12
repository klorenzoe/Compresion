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
            string cadena = "BBBBBBBBBBBBNBBBBBBBBBBBBNNNBBBBBBBBBBBBBBBBBBBBBBBBNBBBBBBBBBBBB";
            string compresion = Encoding.ASCII.GetString(run.RunLenght(cadena));
            Console.ReadKey();
        }
    }
}
