using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace RLE_DLL
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Run_Length compresor = new Run_Length();

            compresor.Comprimir("test.png");
            string value = compresor.Descomprimir("test.comp");
            Console.WriteLine(value);
            Console.ReadLine();
        }
    }
}
