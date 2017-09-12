using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RLE_DLL
{
    class Run_Length
    {
        private static string ENCODE = "iso-8859-1";

        public void Comprimir(string path)
        {
            string archivo = ReadFile(path);
            int repeticiones;
            for (int i = 0; i < archivo.Length - 1; i++) 
            {
                repeticiones = 1;
                while (i != archivo.Length - 1 && archivo[i + 1] == archivo[i])  
                {
                    i++;
                    repeticiones++;
                }
                WriteFile("test.comp", repeticiones, archivo[i]);
            }
       }
        
        public string Descomprimir(string path)
        {
            string texto = "";
            foreach (var item in ReadCompFile(path) )
            {
                if (item != null)
                {
                    string letra = item[item.Length - 1].ToString();
                    int repeticiones = int.Parse(item.Remove(item.Length - 1).ToString());
                    for (int i = 0; i < repeticiones; i++)
                    {
                        texto += letra;
                    }
                }
                else
                {
                    break;
                }
            }
            return texto;
        }
        
    
        private string ReadFile(string path)
        {
            string line = "";
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    for (int i = 0; i < reader.BaseStream.Length; i++)
                    {
                        line += Convert.ToChar(reader.ReadByte());
                    }
                }
            }
            return line;
        }

        private IEnumerable<string> ReadCompFile(string path)
        {
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file, Encoding.GetEncoding(ENCODE)))
                {
                    for (int i = 0; i < reader.BaseStream.Length; i++)
                    {
                        if (reader.BaseStream.Position + 1 < reader.BaseStream.Length)
                        {
                            yield return reader.ReadInt32() + reader.ReadChar().ToString();
                        }
                    }
                }
            }
            yield return null;
        }

        private void WriteFile(string name, int count, char value)
        {
            using (var file = new FileStream(name, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file, Encoding.GetEncoding(ENCODE)))
                {
                    writer.Write(count);
                    writer.Write(value);
                }
            }
        }

    }
}
