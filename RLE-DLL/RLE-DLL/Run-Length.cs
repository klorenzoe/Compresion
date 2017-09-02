using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_DLL
{
    class Run_Length
    {
        public string RunLenght(byte[] entrada)
        {
            string compresion = "";
            int repeticiones;
            for(int i = 0; i < entrada.Length; i++)
            {
                repeticiones = 1;
                while (entrada[i + 1] == entrada[i])
                {
                    i++;
                    repeticiones++;
                }
                compresion += repeticiones + entrada[i];
            }
            return compresion;
        }

    }
}
