using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class Parser
    {
        #region Properties
        readonly List<Token> _tokens;

        readonly InfoTable _infoTable;

        readonly Dictionary<string, int> _grammarLexems;

        string _errorList;

        int _currentToken;

        enum Response {
            OK,
            Empty,
            EOF,
            Error
        }
        #endregion

        public Parser()
        {
            _tokens = new List<Token>();
            _infoTable = new InfoTable();
            _grammarLexems = new Dictionary<string, int>();
        }

        public Parser(List<Token> tokens, InfoTable infoT)
        {
            _tokens = tokens;
            _infoTable = infoT;
            _grammarLexems = new Dictionary<string, int>();
            
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            list = infoT.GetKeywords()
                    .Concat(infoT.GetShortDelimiters())
                    .Concat(infoT.GetLongDelimiters())
                    .ToList();
            foreach (KeyValuePair<string, int> kv in list)
                _grammarLexems.Add(kv.Key, kv.Value);

        }

        #region Methods
        public Node Parse()
        {
            _currentToken = 0;

            Node root = new Node("<Signal program>");
            if (program(root.Children) == Response.EOF) {
                _currentToken = _tokens.Count - 1;
                AddError("Unexpected end of file. File structure was failed");
            }
            return root;
        }

        void AddError(string message)
        {
            _errorList += "Error (line " + _tokens[_currentToken].line +
                         ", column " + _tokens[_currentToken].column + "). " +
                         message + "\n";
        }

        Response program(List<Node> nodes)
        {
            nodes.Add(new Node("<Program>"));

            if (_tokens[_currentToken].code != _grammarLexems["PROGRAM"])
            {
                AddError("Expected 'PROGRAM' keyword, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            if (procedure_identifier(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (_tokens[_currentToken].code != _grammarLexems[";"])
            {
                AddError("Expected ';' after <program-identifier>, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            if (block(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (_tokens[_currentToken].code != _grammarLexems["."])
            {
                AddError("Expected '.' at the end of the program, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));

            return Response.OK;
        }

        Response block(List<Node> nodes)
        {
            nodes.Add(new Node("<Block>"));

            if (_tokens[_currentToken].code != _grammarLexems["BEGIN"])
            {
                AddError("Expected 'BEGIN' keyword, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            if (statements_list(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (_tokens[_currentToken].code != _grammarLexems["END"])
            {
                AddError("Expected 'END' keyword, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            return Response.OK;
        }

        Response statements_list(List<Node> nodes)
        {
            nodes.Add(new Node("<Statements-list>"));

            Response res = statement(nodes.Last().Children);
            if (res == Response.EOF)
                return Response.EOF;
            else if (res == Response.Empty) // out from recursion 
            {
                nodes.Last().Children.RemoveAt(nodes.Last().Children.Count - 1);
                nodes.Last().Children.Add(new Node("<Empty>"));
                return Response.OK;
            }

            if (statements_list(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            return Response.OK;
        }

        Response statement(List<Node> nodes)
        {
            nodes.Add(new Node("<Statement>"));

            int initIndex = _currentToken;
            Response res = condition_statement(nodes.Last().Children, true);
            if (res == Response.EOF)
                return Response.EOF;
            else if (res == Response.OK)
            {
                if (_tokens[_currentToken].code != _grammarLexems["ENDIF"])
                {
                    AddError("Expected 'ENDIF' keyword, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;

                if (_tokens[_currentToken].code != _grammarLexems[";"])
                {
                    AddError("Expected ';' at the end of the statement, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;
            }
            else // trying another branch
            {
                _currentToken = initIndex;
                nodes.Last().Children.RemoveAt(nodes.Last().Children.Count - 1);
                if (_tokens[_currentToken].code != _grammarLexems["WHILE"])
                {
                    return Response.Empty; // No statement
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;

                if (conditional_expression(nodes.Last().Children) == Response.EOF)
                    return Response.EOF;

                if (_tokens[_currentToken].code != _grammarLexems["DO"])
                {
                    AddError("Expected 'DO' keyword, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;

                if (statements_list(nodes.Last().Children) == Response.EOF)
                    return Response.EOF;

                if (_tokens[_currentToken].code != _grammarLexems["ENDWHILE"])
                {
                    AddError("Expected 'ENDWHILE' keyword, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;

                if (_tokens[_currentToken].code != _grammarLexems[";"])
                {
                    AddError("Expected ';' at the end of the statement, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
                else
                    nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;
            }

            return Response.OK;
        }

        Response condition_statement(List<Node> nodes, bool multipleBranch = false)
        {
            nodes.Add(new Node("<Condition-statement>"));

            Response res = incomplete_condition_statement(nodes.Last().Children, multipleBranch);
            if (res == Response.EOF)
                return Response.EOF;
            else if (res == Response.Error)
                return Response.Error;

            if (alternative_part(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            return Response.OK;
        }

        Response incomplete_condition_statement(List<Node> nodes, bool multipleBranch = false)
        {
            nodes.Add(new Node("<Incomplete-condition-statement>"));

            if (_tokens[_currentToken].code != _grammarLexems["IF"])
                if (multipleBranch)
                {
                    nodes.Last().Children.Add(new Node("<Empty>"));
                    return Response.Error;
                }
                else
                {
                    AddError("Expected 'IF' keyword, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
            nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            if (conditional_expression(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (_tokens[_currentToken].code != _grammarLexems["THEN"])
            {
                AddError("Expected 'THEN' keyword, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            if (statements_list(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            return Response.OK;
        }

        Response alternative_part(List<Node> nodes)
        {
            nodes.Add(new Node("<Alternative-part>"));

            if (_tokens[_currentToken].code != _grammarLexems["ELSE"])
            {
                nodes.Last().Children.Add(new Node("<Empty>"));
                return Response.Empty; // it can be empty due to the grammar
            }
            else // There is an alternative part
            {
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
                _currentToken++;
                if (_currentToken >= _tokens.Count)
                    return Response.EOF;

                if (statements_list(nodes.Last().Children) == Response.EOF)
                    return Response.EOF;
            }

            return Response.OK;
        }

        Response conditional_expression(List<Node> nodes)
        {
            nodes.Add(new Node("<Conditional-expression>"));

            if (expression(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (comparison_operator(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            if (expression(nodes.Last().Children) == Response.EOF)
                return Response.EOF;

            return Response.OK;
        }

        Response comparison_operator(List<Node> nodes)
        {
            nodes.Add(new Node("<Comparison-operator>"));

            if (_tokens[_currentToken].code != _grammarLexems["<"] &&
                _tokens[_currentToken].code != _grammarLexems["<="] &&
                _tokens[_currentToken].code != _grammarLexems["="] &&
                _tokens[_currentToken].code != _grammarLexems[">"] &&
                _tokens[_currentToken].code != _grammarLexems[">="] &&
                _tokens[_currentToken].code != _grammarLexems["<>"])
            {
                AddError("Expected comparison operator, but '" +
                    _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                return Response.EOF;
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            return Response.OK;
        }

        Response expression(List<Node> nodes)
        {
            nodes.Add(new Node("<Expression>"));

            int startPosition = _currentToken;
            Response res = variable_identifier(nodes.Last().Children, true);
            if (res == Response.EOF)
                return Response.EOF;
            else if (res == Response.Error)
            {
                _currentToken = startPosition;
                nodes.Last().Children.RemoveAt(nodes.Last().Children.Count - 1);
                res = unsigned_integer(nodes.Last().Children, true);
                if (res == Response.EOF)
                    return Response.EOF;
                else if (res == Response.Error)
                {
                    AddError("Expected identifier or constant, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
            }
            /*
            if (_tokens[_currentToken].code < InfoTable.CONSTANTS_START)
                AddError("Expected identifier or constant, but '" +
                    _tokens[_currentToken].code + "' found.");
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;
*/
            return Response.OK;
        }

        Response procedure_identifier(List<Node> nodes)
        {
            nodes.Add(new Node("<Procedure-identifier>"));

            return identifier(nodes.Last().Children);
        }

        Response variable_identifier(List<Node> nodes, bool multipleBranch = false)
        {
            nodes.Add(new Node("<Variable-identifier>"));

            return identifier(nodes.Last().Children, multipleBranch);
        }

        Response identifier(List<Node> nodes, bool multipleBranch = false)
        {
            nodes.Add(new Node("<Identifier>"));

            if (_tokens[_currentToken].code < InfoTable.IDENTIFIERS_START)
            {
                if (multipleBranch)
                    return Response.Error;
                else
                {
                    AddError("Expected identifier, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;
                }
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            return Response.OK;
        }

        Response unsigned_integer(List<Node> nodes, bool multipleBranch = false)
        {
            nodes.Add(new Node("<Unsigned-integer>"));

            if (_tokens[_currentToken].code < InfoTable.CONSTANTS_START ||
                _tokens[_currentToken].code >= InfoTable.IDENTIFIERS_START)
            {
                if (multipleBranch)
                    return Response.Error;
                else
                {
                    AddError("Expected constant, but '" +
                        _infoTable.GetKey(_tokens[_currentToken].code) + "' found.");
                    return Response.EOF;

                }
            }
            else
                nodes.Last().Children.Add(new Node(_tokens[_currentToken].code));
            _currentToken++;
            if (_currentToken >= _tokens.Count)
                return Response.EOF;

            return Response.OK;
        }

        public string GetErrorList()
        {
            if (string.IsNullOrEmpty(_errorList))
                return "Parser: Error list is empty!\n";
            else
                return _errorList;
        }
        #endregion
    }
}
