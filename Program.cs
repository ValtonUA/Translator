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

            //const string FILENAME = "test.txt";
            const string FILENAME = "parser test.txt";
            //const string FILENAME = "parser test 2.txt";
            //const string FILENAME = "parser test with errors.txt";
            //const string FILENAME = "parser test with errors 2.txt";
            //const string FILENAME = "parser test with errors 3.txt";

            List<Token> tokens = lexer.Analyze(FILENAME);
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

            string listingFile = "";
            try
            {
                FileStream fs = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
                using (var sr = new StreamReader(fs, Encoding.ASCII))
                {
                    listingFile = "\n" + "Program text:" + "\n\n";
                    listingFile += sr.ReadToEnd();
                }
                listingFile += "\n\n" + lexer.GetErrorList();
                listingFile += parser.GetErrorList();
                Console.WriteLine(listingFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            root.Print(root);

            CodeGenerator codeGen = new CodeGenerator(lexer.LexerInfoTable, root);

            var generatedCode = codeGen.Generate();

            Console.WriteLine(generatedCode);

            Console.Read();
        }
    }
}
