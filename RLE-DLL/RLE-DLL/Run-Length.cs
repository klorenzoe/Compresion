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
        private const string EXTENSION = ".comp";
        private string NAME = "";

        public string Comprimir(string path)
        {
            path = FixPath(path);
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
                SetMark(path);
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
            return "Compresión completa en archivo: " + path + EXTENSION + "\n" + Stats(path, path +EXTENSION);
       }
        
        public string Descomprimir(string path)
        {
            path = FixPath(path);
            try
            {
                foreach (var item in ReadCompFile(path))
                {
                    var name = path.Split('\\');
                    name[name.Length - 1] = NAME;
                    path = string.Join("\\", name);

                    if (item != null)
                    {
                        WriteFile(path, new string(item[item.Length - 1], int.Parse(item.Remove(item.Length - 1).ToString())));
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

        public bool CheckFile(string path)
        {
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file, ENCODE))
                {
                    return reader.ReadChar() == 'R' && reader.ReadChar() == 'L';
                }
            }
        }

        private string FixPath(string path)
        {
            if (path != null)
            {
                return path.Replace('/', '\\');
            }
            return null;
        }

        private string ReadFile(string path)
        {
            path = FixPath(path);
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
            path = FixPath(path);
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file, ENCODE))
                {
                    if (reader.ReadChar() == 'R' && reader.ReadChar() == 'L')
                    {
                        NAME = reader.ReadString();
                        for (int i = 0; i < reader.BaseStream.Length; i++)
                        {
                            if (reader.BaseStream.Position + 1 < reader.BaseStream.Length)
                            {
                                yield return (int)(reader.ReadChar()) + reader.ReadChar().ToString();
                                if ((Convert.ToDouble(reader.BaseStream.Position) / Convert.ToDouble(reader.BaseStream.Length) * 100) % 10 == 0)
                                {
                                    Console.WriteLine("- " + (Convert.ToDouble(reader.BaseStream.Position) / Convert.ToDouble(reader.BaseStream.Length ))* 100 + "%");
                                }
                            }
                        }
                    }
                }
            }
            yield return null;
        }

        public string Stats(string PathOriginalFile, string PathCompFile)
        {
            PathOriginalFile = FixPath(PathOriginalFile);
            PathCompFile = FixPath(PathCompFile);
            try
            {
                using (var OriginalFile = new BinaryReader((new FileStream(PathOriginalFile, FileMode.Open)), ENCODE))
                {
                    using (var CompFile = new BinaryReader((new FileStream(PathCompFile, FileMode.Open)), ENCODE))
                    {
                        double before = OriginalFile.BaseStream.Length;
                        double after = CompFile.BaseStream.Length;
                        return "Razón de compresión: " + Math.Round( after/ before,2) + "\nFactor de Compresión: " + Math.Round(before /after, 2) + "\nCompresión total: " + Math.Round((1 - after / before) * 100, 2)+ "%";
                    }
                }
            }
            catch
            {
                return "";
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

        private void SetMark(string name)
        {
            using (var file = new FileStream(name + EXTENSION, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file, ENCODE))
                {
                    name = name.Split('\\')[name.Split('\\').Length - 1];
                    writer.Write('R');
                    writer.Write('L');
                    writer.Write(name);
                }
            }
        }

    }
}
