using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            const string FROM_FILENAME = "test.txt";
            const string TO_FILENAME = "assembler.txt";
            const string LISTING_FILENAME = "listing.txt";

            List<Token> tokens = lexer.Analyze(FROM_FILENAME);
            if (tokens == null)
            {
                Console.WriteLine("Lexer found errors. End of tranlating.");
                Console.Read();
                return;
            }
            /*
            Console.WriteLine("Tokens:");
            tokens.ForEach(t =>
            {
                Console.WriteLine(t.FullInfo);
            });
            
            lexer.LexerInfoTable.PrintFullInfo();

            Console.WriteLine(lexer.GetErrorList());
            */

            Parser parser = new Parser(tokens, lexer.LexerInfoTable);

            Node root = parser.Parse();

            if (root == null)
            {
                Console.WriteLine("Parser found errors. End of tranlating.");
                Console.Read();
                return;
            }
            //root.Print(root);

            CodeGenerator codeGen = new CodeGenerator(lexer.LexerInfoTable, root);

            var generatedCode = codeGen.Generate();
            if (generatedCode == null)
            {
                Console.WriteLine("Code generator found errors. End of tranlating.");
                Console.Read();
                return;
            }
            //Console.WriteLine(generatedCode);

            try
            {
                // create listing file
                string listingFile = "";
                FileStream fs = new FileStream(FROM_FILENAME, FileMode.Open, FileAccess.Read);
                using (var sr = new StreamReader(fs, Encoding.ASCII))
                {
                    listingFile += sr.ReadToEnd();
                }
                listingFile += "\r\n" + lexer.GetErrorList() + "\r\n";
                listingFile += parser.GetErrorList() + "\r\n";
                listingFile += codeGen.GetErrorList() + "\r\n";
                // write it to a file
                fs = new FileStream(LISTING_FILENAME, FileMode.Create, FileAccess.Write);
                using (var sw = new StreamWriter(fs, Encoding.Unicode))
                    sw.WriteLine(listingFile);

                // write generated code to a file
                fs = new FileStream(TO_FILENAME, FileMode.Create, FileAccess.Write);
                using (var sw = new StreamWriter(fs, Encoding.ASCII))
                    sw.WriteLine(generatedCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Files '" + TO_FILENAME + "' and '" +
                LISTING_FILENAME + "' have been created successfully!");

            Console.Read();
        }
    }
}
