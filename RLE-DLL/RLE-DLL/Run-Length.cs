using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_DLL
{
    class Run_Length
    {
        public byte[] RunLenght(string archivo)
        {
            byte[] entrada = Encoding.ASCII.GetBytes(archivo + " ");
            string compresion = "";
            int repeticiones;
            for (int i = 0; i < entrada.Length - 1; i++) 
            {
                repeticiones = 1;
                while (entrada[i + 1] == entrada[i] && i != entrada.Length - 1)  
                {
                    i++;
                    repeticiones++;
                }
                compresion += repeticiones +" "+entrada[i]+" ";
            }
            return Encoding.ASCII.GetBytes(compresion);
        }

    }
}
