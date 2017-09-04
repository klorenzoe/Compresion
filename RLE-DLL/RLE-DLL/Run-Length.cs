using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLE_DLL
{
    class Run_Length
    {
        private const string ENCODE = "iso-8859-1";
        public byte[] Comprimir(string archivo)
        {
            byte[] entrada = Encoding.GetEncoding(ENCODE).GetBytes(archivo + " ");
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
        
        public string Descomprimir(byte[] archivo)
        {
            byte[] data = ASCII_TO_ISO_ARRAY(archivo);
            string texto = "";
            for (int i = 0; i < data.Length - 1; i += 2)
            {
                byte[] value = { data[i + 1] };
                for (int j = 0; j < (char)data[i]; j++)
                {
                    texto += Encoding.GetEncoding(ENCODE).GetString(value);
                }
            }
            return texto;
        }
        
        private byte[] ASCII_TO_ISO_ARRAY(byte[] archivo)
        {
            var data = Encoding.ASCII.GetString(archivo).Split(' ');
            List<byte> values = new List<byte>();
            foreach(string value in data)
            {
                if (value != "")
                {
                    values.Add(byte.Parse(value));
                }
            }
            return values.ToArray();
        }

    }
}
