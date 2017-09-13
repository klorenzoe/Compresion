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
        private static Dictionary<string, int> SymbolsAndOcurrences(/*BinaryReader*/StreamReader Originalfile, ref float TotalElements/*, ref string Compressed*/)
        {
            Dictionary<string, int> Ocurrences = new Dictionary<string, int>();
            Ocurrences.Add("\n", 0);
            Ocurrences.Add("\r", 0);
            string line = "";

            while ((line = Originalfile.ReadLine()) != null)
            {
                Ocurrences["\n"]++;
                Ocurrences["\r"]++;
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
            //char Character;
            //while (Originalfile.BaseStream.Position!=Originalfile.BaseStream.Length)
            //{
            //    Character = Convert.ToChar(Originalfile.ReadByte());
            //    Compressed += Character.ToString();
            //    try
            //    {
            //        Ocurrences[Character.ToString()]++;
            //    }
            //    catch
            //    {
            //        Ocurrences.Add(Character.ToString(), 1);
            //    }
            //    TotalElements++;
            //}

            return Ocurrences;
        }

        /// <summary>
        /// This method calculates each percentage of ocurrence and returns a 
        /// sorted list with the nodes.(Only with its symbol and its percentage)
        /// </summary>
        /// <param name="OriginalFile"></param>
        /// <returns></returns>
        private static List<Node> OccurrencePercentage(/*BinaryReader*/StreamReader OriginalFile/*, ref string Compressed*/)
        {
            List<Node> PercentageList = new List<Node>();
            float TotalElements = 0f;
            Dictionary<string, int> Ocurrences = SymbolsAndOcurrences(OriginalFile, ref TotalElements/*, ref Compressed*/);

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
        private static void AddCodeToTree(ref Node Current, ref Dictionary<string, string> ResultEncoding)
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
                if (Current.Element==",")
                ResultEncoding.Add("coma", Current.Code);
                else
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
        private static Dictionary<string, string> HuffmanEncoding(/*BinaryReader*/StreamReader OriginalFile/*, ref string Compressed*/)
        {
            Dictionary<string, string> ResultEncoding = new Dictionary<string, string>();
            List<Node> NodesList = OccurrencePercentage(OriginalFile/*, ref Compressed*/);
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
        private static void CreateCompressedFile(string Compressed, string PathDestination, string/*Dictionary<string, string>*/ EncodingTable)
        {
            PathDestination = PathDestination.Replace(PathDestination.Split('\\')[PathDestination.Split('\\').Length - 1], "COMPRIMIDO " + /*EncodingTable["Huffman"]*/EncodingTable.Split('\n')[0].Split(',')[1]);
            Directory.CreateDirectory(PathDestination);

            if (File.Exists(PathDestination))
            {
                File.Delete(PathDestination);
            }
           // Compressed = EncodingTable + "\n" + Compressed;

            using (FileStream CompressedFile = File.Create(PathDestination + "\\" + EncodingTable.Split('\n')[0].Split(',')[1].Split('.')[0]/* EncodingTable["Huffman"]*/+ ".comp"))
            {
                using (BinaryWriter CompressedFileBinary = new BinaryWriter(CompressedFile, Encoding.ASCII))
                {
                    //foreach (KeyValuePair<string, string> Element in EncodingTable)
                    //{
                    //    if (Element.Key == "Huffman")
                    //    {
                    //        CompressedFileBinary.Write(Element.Key + ",");
                    //        CompressedFileBinary.Write(Element.Value+",");
                    //    }
                    //    else
                    //    {
                    //        CompressedFileBinary.Write(Element.Value + ",");
                    //        char c = Convert.ToChar(Element.Key.Replace("\\u",""));
                    //        //char c = (char)int.Parse(Element.Key);
                    //        string m = c.ToString();
                    //        CompressedFileBinary.Write(Convert.ToByte(Convert.ToChar(Element.Key).ToString(), 2));
                    //        CompressedFileBinary.Write(",");
                    //    }
                    //}
                    //CompressedFileBinary.Write(Convert.ToByte("\r", 2));

                    CompressedFileBinary.Write(EncodingTable);
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
            StreamReader OriginFile = new StreamReader(File.Open(DestinyPath, FileMode.Open));
            //string Compressed="";
            //BinaryReader OriginFile = new BinaryReader(File.Open(DestinyPath, FileMode.Open));
            Dictionary<string, string> ResultEncoding = HuffmanEncoding(OriginFile/*, ref Compressed*/);

            string EncodingTable = "Huffman," + DestinyPath.Split('\\')[DestinyPath.Split('\\').Length - 1] + ","; //It concat the original name, with the key "Huffman"
            //ResultEncoding.Add("Huffman", DestinyPath.Split('\\')[DestinyPath.Split('\\').Length - 1]);
            OriginFile.BaseStream.Seek(0, SeekOrigin.Begin);
            string Compressed = OriginFile.ReadToEnd().Replace("1","").Replace("0","");

            foreach (KeyValuePair<string, string> Element in ResultEncoding)
            {
                EncodingTable += Element.Value + "," + Element.Key + ",";

                if (Element.Key=="coma")
                    Compressed = Compressed.Replace(",",Element.Value);
                else
                    Compressed = Compressed.Replace(Element.Key,Element.Value);
            }
            CreateCompressedFile(Compressed, DestinyPath, /*ResultEncoding*/EncodingTable.Remove(EncodingTable.Length - 1, 1)+"-----");
        }


        //------------- Descompress zone 
        
        /// <summary>
        /// This method returns the Encoding table in a Dictionary. This information it get it of the
        /// compress directory.
        /// </summary>
        /// <param name="PathEncodingTable"></param>
        /// <returns></returns>
        private static Dictionary<string,string> CodesDictionary(BinaryReader Reader, ref int Seek, ref string OriginalName)
        {
            Dictionary<string, string> CodesTable = new Dictionary<string, string>();

            string FirstLine = "";

            while (true)
            {
                //FirstLine+= Convert.ToChar(Reader.ReadByte()).ToString();
                FirstLine += Reader.ReadChar().ToString();
                if (FirstLine.Contains("-----"))
                    break;
            }
            //FirstLine = FirstLine.Split('\r')[0].Remove(FirstLine.Split('\r')[0].Length - 1, 1);

            Seek = FirstLine.Length;
            FirstLine = FirstLine.Replace("-----", "");
            
            string[] MessyCodes = FirstLine.Split(',');

            OriginalName = MessyCodes[1];
            for (int i = 2; i < MessyCodes.Length; i+=2)
            {
               if(MessyCodes[i+1]=="coma")
                     CodesTable.Add(MessyCodes[i],",");
               else
                    CodesTable.Add(MessyCodes[i], MessyCodes[i + 1]);
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
                Dictionary<string, string> TableCodes = CodesDictionary(DescompressFile, ref Seek, ref OriginalName);
                DescompressFile.BaseStream.Position = Seek;

                char CharCode;
                string BitToBit = "", segment="";

                while (DescompressFile.BaseStream.Position != DescompressFile.BaseStream.Length)
                {
                    CharCode = Convert.ToChar(DescompressFile.ReadByte());
                    BitToBit = Convert.ToString(CharCode, 2).PadLeft(8, '0');

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
