using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{   
    public partial class Lexer
    {
        public Lexer()
        {
            LexerInfoTable = new InfoTable();
            // Experimental thing
            // I add some lexems, that my grammar consist of, to info table beforehand
            LexerInfoTable.AddKeyword(_keywords);
            LexerInfoTable.AddLongDelimiter(_longDelimiters);
            LexerInfoTable.AddShortDelimiter(new List<string>
            {
                ";", ".", "=", "<", ">"
            });
        }

        public Lexer(InfoTable infoT)
        {
            LexerInfoTable = infoT;
        }

        public List<Token> Analyze(string filename)
        {
            Category category;
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (var sr = new StreamReader(fs, Encoding.ASCII))
                {
                    _programText = sr.ReadToEnd();
                    _programText += " ";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            _errorList = null;
            _currPos = 0;
            _currLine = 0;
            _currColumn = -1;
            _currSymbol = GetSymbol();

            while (_currPos < _programText.Length)
            {
                category = _symbolCategories[_currSymbol];
                switch (category)
                {
                    case Category.ws:
                        {
                            while (_symbolCategories[_currSymbol] == Category.ws &&
                                    _currPos < _programText.Length)
                            {
                                _currSymbol = GetSymbol();
                            }
                            break;
                        }
                    case Category.dig:
                        {
                            _startColumn = _currColumn;
                            while (_symbolCategories[_currSymbol] == Category.dig &&
                                    _currPos < _programText.Length)
                            {
                                _currLexem += _currSymbol;
                                _currSymbol = GetSymbol();
                            }
                            /* Additional task (data lexem like 16/04/2019)
                            if (_currSymbol == '/')
                            {
                                bool exitFlag = false;
                                if (_currLexem.Length != 2 && int.Parse(_currLexem) > 31)
                                {
                                    AddError("Error in date\n");
                                    _currLexem = "";
                                    _currSymbol = GetSymbol();
                                    break;
                                }
                                _currLexem += _currSymbol;
                                _currSymbol = GetSymbol();
                                string month = "";
                                for (int i = 0; i < 2; i++)
                                {
                                    if (_symbolCategories[_currSymbol] != Category.dig)
                                    {
                                        AddError("Error in month\n");
                                        _currLexem = "";
                                        exitFlag = true;
                                        break;
                                    }
                                    _currLexem += _currSymbol;
                                    month += _currSymbol;
                                    _currSymbol = GetSymbol();
                                }
                                if (exitFlag)
                                    break;

                                    
                                if (_currSymbol != '/')
                                {
                                    AddError("Expected '/' between month and year\n");
                                    _currLexem = "";
                                    break;
                                }
                                
                                if (int.Parse(month) > 12)
                                {
                                    AddError("Error in month(can`t be more than 12)\n");
                                    _currLexem = "";
                                    break;
                                }
                                _currLexem += _currSymbol;

                                for (int i = 0; i < 4; i++)
                                {
                                    _currSymbol = GetSymbol();
                                    if (_symbolCategories[_currSymbol] != Category.dig)
                                    {
                                        AddError("Error in year\n");
                                        _currLexem = "";
                                        exitFlag = true;
                                        break;
                                    }
                                    _currLexem += _currSymbol;
                                }
                                if (exitFlag)
                                    break;

                                _currSymbol = GetSymbol();
                                if (_symbolCategories[_currSymbol] != Category.ws &&
                                    _symbolCategories[_currSymbol] != Category.com)
                                {
                                    AddError("Error in date structure\n");
                                    _currLexem = "";
                                    break;
                                }
                                else 
                                    Console.WriteLine(_currLexem);
                                _currLexem = "";
                            }
                            Additional task */
                            
                            OutToken(_currLexem, category);
                            _currLexem = "";
                            break;
                        }
                    case Category.let:
                        {
                            _startColumn = _currColumn;
                            while (_symbolCategories[_currSymbol] == Category.dig ||
                                    _symbolCategories[_currSymbol] == Category.let)
                            {
                                _currLexem += _currSymbol;
                                if (_currPos < _programText.Length)
                                    _currSymbol = GetSymbol();
                                else
                                    break;
                            }

                            OutToken(_currLexem, category);
                            _currLexem = "";
                            break;
                        }
                    case Category.del1:
                        {
                            _startColumn = _currColumn;
                            _currLexem += _currSymbol;
                            OutToken(_currLexem, category);
                            _currLexem = "";
                            _currSymbol = GetSymbol();
                            break;
                        }
                    case Category.del2:
                        {
                            _startColumn = _currColumn;
                            _currLexem += _currSymbol; 
                            _currSymbol = GetSymbol(); // second symbol
                            _currLexem += _currSymbol; // Probably a long delimiter
                            bool isFound = false;
                            foreach (string longDel in _longDelimiters)
                            {
                                if (longDel == _currLexem) // Long delimeter is found
                                {
                                    OutToken(_currLexem, category);
                                    _currLexem = "";
                                    _currSymbol = GetSymbol();
                                    isFound = true;
                                    break;
                                }
                            }

                            if (isFound == false) // if it`s a short delimiter
                            {
                                // delete the second symbol from a lexem
                                OutToken(_currLexem.Trim(_currSymbol), Category.del1);
                                _currLexem = "";
                            }

                            break;
                        }
                    case Category.com:
                        {
                            _currSymbol = GetSymbol(); 
                            if (_currSymbol != '*')
                            {
                                AddError("Non-opened commentary, missed '*' after '('\n");
                                break;
                            }
                            // Escape commentaries
                            _currSymbol = GetSymbol();
                            bool exitFlag = false;
                            while (!exitFlag)
                            {
                                while (_currSymbol != '*')
                                {
                                    if (_currPos >= _programText.Length)
                                    {
                                        //AddError("Non-closed commentary, missed '*'\n");
                                        AddError("Non-closed commentary\n");
                                        exitFlag = true;
                                        break;
                                    }
                                    _currSymbol = GetSymbol();
                                }
                                if (exitFlag)
                                    break;
                                if (_currPos >= _programText.Length)
                                {
                                    //AddError("Non-closed commentary, missed ')'\n");
                                    AddError("Non-closed commentary\n");
                                    exitFlag = true;
                                }
                                _currSymbol = GetSymbol();
                                if (_currSymbol == ')') // commentary is closed
                                {
                                    _currSymbol = GetSymbol();
                                    exitFlag = true;
                                }    
                            }

                            break;
                        }
                    case Category.err:
                        {
                            AddError("Forbidden symbol '" + _currSymbol + "'.\n");
                            _currSymbol = GetSymbol();
                            break;
                        }
                }
            }

            if (string.IsNullOrEmpty(_errorList))
                return _tokens;
            else
                return null;
        }

        void AddError (string errorMessage)
        {
            _errorList += "Error (line " + _currLine + ", column " + _currColumn + "). " +
                         errorMessage;
        }

        char GetSymbol()
            // Next symbol must be taken only with this function!
            // Otherwise counters will be corrupt
        {
            if (_currPos >= _programText.Length)
                return (char)0; // EOF
            if (_programText[_currPos] == '\n')
            {
                _currLine++;
                _currColumn = -1;
            }
            else
                _currColumn++;

            return _programText[_currPos++];
        }

        void OutToken(string lexem, Category category)
        {
            int lexemCode = -1;
            switch (category)
            {
                case Category.dig:
                    {
                        lexemCode = LexerInfoTable.AddConstant(lexem);
                        break;
                    }
                case Category.let:
                    {
                        foreach(string keyword in _keywords)
                        {
                            if (keyword == lexem.ToUpper()) // Our lexem is keyword
                            {
                                lexemCode = LexerInfoTable.AddKeyword(lexem.ToUpper());
                                break;
                            }
                        }
                        // Otherwise it`s an identifier
                        if (lexemCode == -1)
                            lexemCode = LexerInfoTable.AddIdentifier(lexem);
                        break;
                    }
                case Category.del1:
                    {
                        lexemCode = LexerInfoTable.AddShortDelimiter(lexem);
                        break;
                    }
                case Category.del2:
                    {
                        lexemCode = LexerInfoTable.AddLongDelimiter(lexem);
                        break;
                    }
            }
            if (lexemCode != -1)
                _tokens.Add(new Token
                {
                    code = lexemCode,
                    column = _startColumn,
                    line = _currLine
                });
        }

        public string GetErrorList()
        {
            if (string.IsNullOrEmpty(_errorList))
                return "Lexer: Error list is empty!\n";
            else
                return _errorList;
        }
    }
}
