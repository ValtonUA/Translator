using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class Token
    {
        public int code;
        public int line;
        public int column;

        public Token()
        {

        }

        public string FullInfo
        {
            get
            {
                return "Code: " + code + 
                       ", line:" + line + 
                       ", column:" + column;
            }
        }
    }

    public partial class Lexer
    {
        List<Token> _tokens = new List<Token>();

        public InfoTable LexerInfoTable { get; private set; }

        string _programText;
        int _currPos;
        char _currSymbol;
        string _currLexem;
        int _currLine;
        int _startColumn;
        int _currColumn;

        private string _errorList;

        List<string> _keywords = new List<string>
        {
            "PROGRAM", "BEGIN", "END", "ENDIF",
            "WHILE", "DO", "ENDWHILE", "IF",
            "THEN", "ELSE"
        };

        List<string> _longDelimiters = new List<string>
        {
            "<=", "<>", ">="
        };

        enum Category {
            ws, // Whitespaces
            dig, // Digits
            let, // Letters
            del1, // Short delimiters
            del2, // long delimiters
            com, // commentaries
            err // forbidden symbols
        }

        Category[] _symbolCategories = ArrangeSymbolCategories();

        static Category[] ArrangeSymbolCategories ()
        {
            const int ARRAY_SIZE = 128;
            Category[] symbols = new Category[ARRAY_SIZE];
            int i;
            
            for (i = 0; i < ARRAY_SIZE; i++) // Other symbols are forbidden
                symbols[i] = Category.err;

            for (i = 8; i <= 13; i++) // Whitespaces
                symbols[i] = Category.ws;

            symbols[32] = Category.ws; // Space

            symbols[40] = Category.com; // '('
            symbols[41] = Category.del1; // ')'
            symbols[46] = Category.del1; // '.'

            for (i = 48; i <= 57; i++) // '0' - '9'
                symbols[i] = Category.dig;

            symbols[58] = Category.del1; // ':'
            symbols[59] = Category.del1; // ';'
            symbols[60] = Category.del2; // '<'
            symbols[61] = Category.del1; // '='
            symbols[62] = Category.del2; // '>'

            for (i = 65; i <= 90; i++) // 'A' - 'Z'
                symbols[i] = Category.let;

            for (i = 97; i <= 122; i++) // 'a' - 'z'
                symbols[i] = Category.let;

            return symbols;
        }
    }
}
