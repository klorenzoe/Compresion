using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman
{
    class Node
    {
        public float Percentage { get; set; }
        public char Element { get; set; }
        public string Code { get; set; }
        public bool IsFull { get; set; }

        public Node Father { get; set; }
        public Node SonRight { get; set; }
        public Node SonLeft { get; set; }
    }
}
