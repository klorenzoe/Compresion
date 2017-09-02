using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Huffman
{
    static class HuffmanTree
    {
        //------------- Compress zone

        /// <summary>
        /// This method finds the ocurrences of each symbol, returns a dictionary with the 
        /// symbol like key and ocurrences like value. 
        /// </summary>
        /// <param name="Originalfile"></param>
        /// <param name="TotalElements"></param>
        /// <returns></returns>
        private static Dictionary<string, int> SymbolsAndOcurrences(StreamReader Originalfile, ref float TotalElements)
        {
            Dictionary<string, int> Ocurrences = new Dictionary<string, int>();
            string line = "";

            while ((line = Originalfile.ReadLine()) != null)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    try
                    {
                        Ocurrences[line[i].ToString()]++;
                    }
                    catch
                    {
                        Ocurrences.Add(line[i].ToString(), 1);
                    }
                    TotalElements++;
                }
            }

            return Ocurrences;
        }

        /// <summary>
        /// This method calculates each percentage of ocurrence and returns a 
        /// sorted list with the nodes.(Only with its symbol and its percentage)
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <returns></returns>
        private static List<Node> OccurrencePercentage(StreamReader OriginalFile)
        {
            List<Node> PercentageList = new List<Node>();
            float TotalElements = 0f;
            Dictionary<string, int> Ocurrences = SymbolsAndOcurrences(OriginalFile, ref TotalElements);

            foreach (KeyValuePair<string, int> Symbol in Ocurrences)
            {
                Node SymbolNode = new Node
                {
                    Element = Symbol.Key,
                    Percentage = Symbol.Value / TotalElements
                };
                PercentageList.Add(SymbolNode);
            }
            PercentageList.Sort((x, y) => x.Percentage.CompareTo(y.Percentage));
            return PercentageList;
        }

        /// <summary>
        /// This method does the tree. It returns the root.
        /// </summary>
        /// <param name="NodesList"></param>
        /// <returns></returns>
        private static Node HuffmanTreeRecursive(List<Node> NodesList)
        {
            if (NodesList.Count != 0)
            {
                if (NodesList.Count == 1)
                {
                    return NodesList[0];
                }
                Node Current1 = new Node();
                Node Current2 = new Node();
                Node CurrentFather = new Node();

                Current1 = NodesList[0];
                Current2 = NodesList[1];
                CurrentFather.Percentage = Current1.Percentage + Current2.Percentage;

                if (Current1.Percentage > Current2.Percentage)
                {
                    CurrentFather.SonRight = Current1;
                    CurrentFather.SonLeft = Current2;
                }
                else
                {
                    CurrentFather.SonRight = Current2;
                    CurrentFather.SonLeft = Current1;
                }
                Current1.Father = CurrentFather;
                Current2.Father = CurrentFather;

                NodesList.RemoveAt(0);
                NodesList.RemoveAt(0);
                NodesList.Add(CurrentFather);
                NodesList.Sort((x, y) => x.Percentage.CompareTo(y.Percentage));
            }
            return HuffmanTreeRecursive(NodesList);
        }

        /// <summary>
        /// This method runs the tree and assings its code (0 and 1 combined) 
        /// </summary>
        /// <param name="Current"></param>
        /// <param name="ResultEncoding"></param>
        private static void AddCodeToTree(ref Node Current, ref Dictionary<string, Node> ResultEncoding)
        {
            Node Temporal;
            if (Current.Father == null)
            {
                Current.SonLeft.Code = "0";
                Current.SonRight.Code = "1";

            }
            else if (Current.SonLeft != null && Current.SonRight != null)
            {
                Current.SonLeft.Code = Current.Code + "0";
                Current.SonRight.Code = Current.Code + "1";
            }

            if (Current.Element != null)
            {
                ResultEncoding.Add(Current.Element, Current);
            }

            if (Current.SonLeft != null && Current.SonRight != null)
            {
                Temporal = Current.SonLeft;
                AddCodeToTree(ref Temporal, ref ResultEncoding); //To the left
                Temporal = Current.SonRight;
                AddCodeToTree(ref Temporal, ref ResultEncoding);//To the right
            }
        }

        /// <summary>
        /// This method calls to other necessary methods (the before methods) 
        /// for complete the encoding table, It returns the encoding table (It has all the
        /// symbol with its percentage and code).
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <returns></returns>
        private static Dictionary<string, Node> HuffmanEncoding(StreamReader OriginalFile)
        {
            Dictionary<string, Node> ResultEncoding = new Dictionary<string, Node>();
            List<Node> NodesList = OccurrencePercentage(OriginalFile);
            Node Root = HuffmanTreeRecursive(NodesList);

            AddCodeToTree(ref Root, ref ResultEncoding);

            return ResultEncoding;
        }

        /// <summary>
        /// This method create a directory for saving the result compressed, join the its encoding table
        /// and original name.
        /// </summary>
        /// <param name="Compressed"></param>
        /// <param name="PathDestination"></param>
        private static void CreateCompressedFile(string Compressed, string PathDestination, string EncodingTable)
        {
            PathDestination = PathDestination.Replace(PathDestination.Split('\\')[PathDestination.Split('\\').Length - 1], "COMPRIMIDO " + EncodingTable.Split('\n')[0].Split(',')[1]);
            Directory.CreateDirectory(PathDestination);

            if (File.Exists(PathDestination))
            {
                File.Delete(PathDestination);
            }

            using (FileStream CompressedFile = File.Create(PathDestination + "\\" + EncodingTable.Split('\n')[0].Split('.')[0] + ".comp"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(Compressed);
                CompressedFile.Write(info, 0, info.Length);
            }

            using (FileStream CompressedFile = File.Create(PathDestination + "\\EncodingTable.comp"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(EncodingTable);
                CompressedFile.Write(info, 0, info.Length);
            }
        }

        /// <summary>
        /// This method is the only one public (for the compressor). This manages the others methods for the compression.
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <param name="PathDestination"></param>
        public static void Compressor(StreamReader OriginalFile, string PathDestination)
        {
            Dictionary<string, Node> ResultEncoding = HuffmanEncoding(OriginalFile);

            OriginalFile.BaseStream.Seek(0, SeekOrigin.Begin);

            string Compressed = OriginalFile.ReadToEnd();

            foreach (KeyValuePair<string, Node> Element in ResultEncoding)
            {
                Compressed = Compressed.Replace(Element.Key, Element.Value.Code);
            }

            string EncodingTable = "Huffman,"+PathDestination.Split('\\')[PathDestination.Split('\\').Length - 1] + "\n";
            foreach (KeyValuePair<string, Node> Encoding in ResultEncoding)
            {
                EncodingTable = EncodingTable + Encoding.Value.Code + "," + Encoding.Key + "\n";
            }
            CreateCompressedFile(Compressed, PathDestination, EncodingTable);

        }


        //------------- Descompress zone 
        
        /// <summary>
        /// This method returns the Encoding table in a Sorted list. This information it get it of the
        /// compress directory.
        /// </summary>
        /// <param name="PathEncodingTable"></param>
        /// <returns></returns>
        private static SortedList<string,string> CodesDictionary(string PathEncodingTable)
        {
            StreamReader OriginFile = new StreamReader(File.Open(PathEncodingTable, FileMode.Open));
            SortedList<string, string> EncodingTable = new SortedList<string, string>();

            string Line = "";
            while ((Line = OriginFile.ReadLine())!=null)
            {
                EncodingTable.Add(Line.Split(',')[0], Line.Split(',')[1]);
            }

            return EncodingTable;
        }

        /// <summary>
        /// this method replaces the code with the real symbol, and return the decoded.
        /// </summary>
        /// <param name="PathCompress"></param>
        /// <param name="PathEncodingTable"></param>
        /// <returns></returns>
        private static string DecodeTheFile(string PathCompress, string PathEncodingTable, ref string OriginalName)
        {
            StreamReader OriginFile = new StreamReader(File.Open(PathCompress, FileMode.Open));
            SortedList<string, string> EncodingTable = CodesDictionary(PathEncodingTable);
            OriginalName = EncodingTable["Huffman"];

            string Line = "";
            string chain = "";
            string Descompressor = "";

            while ((Line = OriginFile.ReadLine())!=null)
            {
                for (int i = 0; i < Line.Length; i++)
                {
                    chain += Line[i].ToString();
                    try
                    {
                        Descompressor +=EncodingTable[chain];
                        chain = "";
                    }
                    catch{}
                }
            }

            return Descompressor;
        }

        /// <summary>
        /// This method create a file with the decode result, with the original name, out of the compress directory.
        /// </summary>
        /// <param name="DescompressContent"></param>
        /// <param name="DirectoryPath"></param>
        /// <param name="OriginalName"></param>
        private static void CreateDecodeFile(string DescompressContent, string DirectoryPath, string OriginalName)
        {
            //DirectoryPath = DirectoryPath.Replace(DirectoryPath.Split('\\')[DirectoryPath.Split('\\').Length-1],OriginalName);
            DirectoryPath += "\\"+OriginalName;
            if (File.Exists(DirectoryPath))
            {
                File.Delete(DirectoryPath);
            }

            using (FileStream DescompressedFile = File.Create(DirectoryPath))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(DescompressContent);
                DescompressedFile.Write(info, 0, info.Length);
            }
        }

        /// <summary>
        /// This method manages the others methods for the descompressor, is the only one public (for the descompressor)
        /// </summary>
        /// <param name="PathCompresorFile"></param>
        public static void Descompressor(string PathCompresorFile)
        {
            DirectoryInfo DirectoryToDescompress = new DirectoryInfo(PathCompresorFile);
            string PathCompress = "";
            string PathEncodingTable = "";
            string OriginalName = "";

            foreach (var SpecificFile in DirectoryToDescompress.GetFiles())
            {
                if (SpecificFile.Name == "EncodingTable.comp")
                {
                    PathEncodingTable = SpecificFile.FullName;
                }
                else if (SpecificFile.Name.Split('.')[SpecificFile.Name.Split('.').Length - 1] == "comp")
                {
                    PathCompress = SpecificFile.FullName;
                }
            }

            CreateDecodeFile(DecodeTheFile(PathCompress, PathEncodingTable, ref OriginalName), PathCompresorFile, OriginalName);
        }
    }


}
