using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class Node
    {
        public string Value { get; private set; }
        public List<Node> Children { get; private set; }

        public Node()
        {
            Children = new List<Node>();
        }

        public Node(string value)
        {
            Value = value;
            Children = new List<Node>();
        }

        public Node(int value)
        {
            Value = value.ToString();
            Children = new List<Node>();
        }

        public void Print(Node node, int spaces = 0)
        {
            for (int i = 0; i < spaces; i++)
                Console.Write("  ");
            Console.WriteLine(node.Value);

            if (node?.Children != null)
                foreach (Node n in node.Children)
                    Print(n, spaces + 1);
        }
    }
}
