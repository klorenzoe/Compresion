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
        private Encoding ENCODE = Encoding.GetEncoding("iso-8859-1");
        private const string EXTENSION = ".rlex";

        public string Comprimir(string path)
        {
            string archivo;
            try
            {
                archivo = ReadFile(path);
            }
            catch (FileNotFoundException ex)
            {
                return ex.Message;
            }
            try
            {
                int repeticiones;
                for (int i = 0; i < archivo.Length - 1; i++)
                {
                    repeticiones = 1;
                    while (i != archivo.Length - 1 && archivo[i + 1] == archivo[i])
                    {
                        i++;
                        repeticiones++;
                    }
                    try
                    {
                        WriteCompFile(path + EXTENSION, repeticiones, archivo[i]);
                    }
                    catch (FileNotFoundException ex)
                    {
                        return ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Se ha producido un error: " + ex.Message;
            }
            return "Compresión completa en archivo: " + path + EXTENSION;
       }
        
        public string Descomprimir(string path)
        {
            try
            {
                foreach (var item in ReadCompFile(path))
                {
                    if (item != null)
                    {
                        string letra = item[item.Length - 1].ToString();
                        int repeticiones = int.Parse(item.Remove(item.Length - 1).ToString());
                        for (int i = 0; i < repeticiones; i++)
                        {
                            WriteFile(path.Replace(".rlex", ""), letra);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return "Descompresión completa en archivo: " + path.Replace(".rlex", "");
            }
            catch (Exception ex)
            {
                return "Se ha producido un error: " + ex.Message;
            }
        }

        private string ReadFile(string path)
        {
            string line = "";
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file, ENCODE))
                {
                    for (int i = 0; i < reader.BaseStream.Length; i++)
                    {
                        line += reader.ReadChar();
                    }
                }
            }
            return line;
        }

        private IEnumerable<string> ReadCompFile(string path)
        {
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file, ENCODE))
                {
                    for (int i = 0; i < reader.BaseStream.Length; i++)
                    {
                        if (reader.BaseStream.Position + 1 < reader.BaseStream.Length)
                        {
                            yield return (int)(reader.ReadChar()) + reader.ReadChar().ToString();
                        }
                    }
                }
            }
            yield return null;
        }

        public double RazonCompresion(string PathOriginalFile, string PathCompFile)
        {
            try
            {
                using (var OriginalFile = new BinaryReader((new FileStream(PathOriginalFile, FileMode.Open)), ENCODE))
                {
                    using (var CompFile = new BinaryReader((new FileStream(PathCompFile, FileMode.Open)), ENCODE))
                    {
                        return CompFile.BaseStream.Length / OriginalFile.BaseStream.Length;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public double FactorCompresion(string PathOriginalFile, string PathCompFile)
        {
            try
            {
                using (var OriginalFile = new BinaryReader((new FileStream(PathOriginalFile, FileMode.Open)), ENCODE))
                {
                    using (var CompFile = new BinaryReader((new FileStream(PathCompFile, FileMode.Open)), ENCODE))
                    {
                        return OriginalFile.BaseStream.Length / CompFile.BaseStream.Length;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void WriteFile(string name, string data)
        {
            using (var file = new FileStream(name, FileMode.Append))
            {
                using (var writer = new StreamWriter(file, ENCODE))
                {
                    writer.Write(data);
                }
            }
        }

        private void WriteCompFile(string name, int count, char value)
        {
            using (var file = new FileStream(name, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file, ENCODE))
                {
                    for (int i = 0; i < count / 255; i++)
                    {
                        writer.Write((char)255);
                        writer.Write(value);
                    }
                    writer.Write((char)(count % 255));
                    writer.Write(value);
                }
            }
        }

        private void Stadistics(string cPath, string dPath)
        {
        }

    }
}
