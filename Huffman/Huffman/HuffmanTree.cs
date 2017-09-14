using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Huffman
{
    static class HuffmanTree
    {
        static Encoding ExperimentalEncoding = UTF8Encoding.UTF8;
        //------------- Compress zone

        /// <summary>
        /// This method finds the ocurrences of each symbol, returns a dictionary with the 
        /// symbol like key and ocurrences like value. 
        /// </summary>
        /// <param name="Originalfile"></param>
        /// <param name="TotalElements"></param>
        /// <returns></returns>
        private static Dictionary<char, int> SymbolsAndOcurrences(BinaryReader Originalfile, ref float TotalElements, ref string Compressed)
        {
            Dictionary<char, int> Ocurrences = new Dictionary<char, int>();
            char Character;
            while (Originalfile.BaseStream.Position != Originalfile.BaseStream.Length)
            {
                Character = Convert.ToChar(Originalfile.ReadByte());
                Compressed += Character;

                try
                {
                    Ocurrences[Character]++;
                }
                catch
                {
                    Ocurrences.Add(Character, 1);
                }
                TotalElements++;
            }

            return Ocurrences;
        }

        /// <summary>
        /// This method calculates each percentage of ocurrence and returns a 
        /// sorted list with the nodes.(Only with its symbol and its percentage)
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <returns></returns>
        private static List<Node> OccurrencePercentage(BinaryReader/*StreamReader*/ OriginalFile, ref string Compressed)
        {
            List<Node> PercentageList = new List<Node>();
            float TotalElements = 0f;
            Dictionary<char, int> Ocurrences = SymbolsAndOcurrences(OriginalFile, ref TotalElements, ref Compressed);

            foreach (KeyValuePair<char, int> Symbol in Ocurrences)
            {
                Node SymbolNode = new Node
                {
                    Element = Symbol.Key,
                    Percentage = Symbol.Value / TotalElements,
                    IsFull = true
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
        private static void AddCodeToTree(ref Node Current, ref Dictionary<char, string> ResultEncoding)
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

            if (Current.IsFull)
            {
               ResultEncoding.Add(Current.Element, Current.Code);
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
        private static Dictionary<char, string> HuffmanEncoding(BinaryReader/*StreamReader */OriginalFile, ref string Compressed)
        {
            Dictionary<char, string> ResultEncoding = new Dictionary<char, string>();
            List<Node> NodesList = OccurrencePercentage(OriginalFile, ref Compressed);
            Node Root = HuffmanTreeRecursive(NodesList);
            if(Root.SonRight!=null || Root.SonLeft != null)
            {
                AddCodeToTree(ref Root, ref ResultEncoding);
            }
            else
            {
                ResultEncoding.Add(Root.Element, "0");
            }

            return ResultEncoding;
        }

        /// <summary>
        /// This method create a directory for saving the result compressed, join the its encoding table
        /// and original name.
        /// </summary>
        /// <param name="Compressed"></param>
        /// <param name="PathDestination"></param>
        private static void CreateCompressedFile(string Compressed, string PathDestination, string/*Dictionary<string, string>*/ EncodingTable)
        {
            PathDestination = PathDestination.Replace(PathDestination.Split('\\')[PathDestination.Split('\\').Length - 1], "COMPRIMIDO " + /*EncodingTable["Huffman"]*/EncodingTable.Split(new[] { "||" }, StringSplitOptions.None)[1]);
            Directory.CreateDirectory(PathDestination);

            if (File.Exists(PathDestination))
            {
                File.Delete(PathDestination);
            }

            using (FileStream CompressedFile = File.Create(PathDestination + "\\" + EncodingTable.Split(new[] { "||" }, StringSplitOptions.None)[1].Split('.')[0]/* EncodingTable["Huffman"]*/+ ".comp"))
            {
                using (BinaryWriter CompressedFileBinary = new BinaryWriter(CompressedFile, Encoding.ASCII))
                {
                    for (int i = 0; i < EncodingTable.Length; i++)
                    {
                        char c = EncodingTable[i];
                        string m = Convert.ToString(c,2);
                        CompressedFileBinary.Write(Convert.ToByte(m, 2));
                    }
                    
                    while (8 < Compressed.Length)
                    {
                        CompressedFileBinary.Write(Convert.ToByte(Compressed.Substring(0, 8), 2));
                        Compressed = Compressed.Remove(0, 8);
                    }
                    CompressedFileBinary.Write(Convert.ToByte(Compressed, 2));
                }
            }
        }

        /// <summary>
        /// This method is the only one public (for the compressor). This manages the others methods for the compression.
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <param name="DestinyPath"></param>
        public static void Compressor(string DestinyPath)
        {
            BinaryReader OriginFile = new BinaryReader(File.Open(DestinyPath, FileMode.Open));
            string TextToCompress = ""; /*OriginFile.ReadToEnd();*/
            Dictionary<char, string> ResultEncoding = HuffmanEncoding(OriginFile, ref TextToCompress);

            string EncodingTable = "";
            
            OriginFile.BaseStream.Seek(0, SeekOrigin.Begin);

            foreach (KeyValuePair<char, string> Element in ResultEncoding)
            {
                EncodingTable += Element.Value + "||" + Element.Key + "||";
            }
            string Compressed = "";
            for (int i = 0; i < TextToCompress.Length; i++)
            {
                char m = TextToCompress[i];
               Compressed += ResultEncoding[TextToCompress[i]];
            }
            EncodingTable = "length||"+ Compressed.Length+"||" + EncodingTable;
            EncodingTable = "Huffman||" + DestinyPath.Split('\\')[DestinyPath.Split('\\').Length - 1] + "||"+EncodingTable; //It concat the original name, with the key "Huffman"

            CreateCompressedFile(Compressed, DestinyPath, /*ResultEncoding*/EncodingTable.Remove(EncodingTable.Length - 2, 2)+">>");
        }


        //------------- Descompress zone 
        
        /// <summary>
        /// This method returns the Encoding table in a Dictionary. This information it get it of the
        /// compress directory.
        /// </summary>
        /// <param name="PathEncodingTable"></param>
        /// <returns></returns>
        private static Dictionary<string,char> CodesDictionary(BinaryReader Reader, ref int Seek, ref string OriginalName, ref int Huffmanlength)
        {
            Dictionary<String, char> CodesTable = new Dictionary<String, char>();

            string FirstLine = "";

            while (true)
            {
                FirstLine += Convert.ToChar(Reader.ReadByte()).ToString();
                if (FirstLine.Contains(">>"))
                    break;
            }
            Seek = FirstLine.Length;
            FirstLine = FirstLine.Replace(">>", "");

            string[] MessyCodes = FirstLine.Split(new[] { "||" }, StringSplitOptions.None);

            OriginalName = MessyCodes[1];
            Huffmanlength = int.Parse(MessyCodes[3]);
            
            for (int i = 4; i < MessyCodes.Length; i+=2)
            {
                if (MessyCodes[i+1] == "")
                {
                    CodesTable.Add(MessyCodes[i], '|');
                    MessyCodes[i+2]= MessyCodes[i + 2].Replace("|","");
                }   
                else
                    CodesTable.Add(MessyCodes[i], Convert.ToChar(MessyCodes[i + 1]));
            }

            return CodesTable;
        }
        
        /// <summary>
        /// this method replaces the code with the real symbol, and return the decoded.
        /// </summary>
        /// <param name="PathCompress"></param>
        /// <param name="PathEncodingTable"></param>
        /// <returns></returns>
        private static string DecodeTheFile(string PathCompress, string PathEncodingTable, ref string OriginalName)
        {
            string Descompressor = "";
            int Seek=0;
            using (BinaryReader DescompressFile = new BinaryReader(new FileStream(PathCompress, FileMode.Open), Encoding.ASCII))
            {
                int Compressedlength = 0;
                Dictionary<string, char> TableCodes = CodesDictionary(DescompressFile, ref Seek, ref OriginalName, ref Compressedlength);
                DescompressFile.BaseStream.Position = Seek;

                char CharCode;
                string BitToBit = "", segment="";
                int Descompressedlength=0;

                while (DescompressFile.BaseStream.Position != DescompressFile.BaseStream.Length)
                {
                    CharCode = Convert.ToChar(DescompressFile.ReadByte());
                    BitToBit = Convert.ToString(CharCode, 2).PadLeft(8, '0');
                    Descompressedlength++;

                    if(DescompressFile.BaseStream.Position == DescompressFile.BaseStream.Length)
                    {
                        BitToBit = BitToBit.Remove(0, (Descompressedlength*8)- Compressedlength);
                    }

                    for (int i = 0; i < BitToBit.Length; i++)
                    {
                        segment += BitToBit[i].ToString();
                        if (TableCodes.ContainsKey(segment))
                        {
                            Descompressor += TableCodes[segment];
                            segment = "";
                        }
                    }

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
            DirectoryPath += "\\"+OriginalName;
            if (File.Exists(DirectoryPath))
            {
                File.Delete(DirectoryPath);
            }

            using (FileStream DescompressedFile = File.Create(DirectoryPath))
            {
                Byte[] info = Encoding.GetEncoding("iso-8859-1").GetBytes(DescompressContent);
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
