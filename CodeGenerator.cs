﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class CodeGenerator
    {
        #region Properies
        InfoTable _infoTable;
        // input expression tree received from parser
        Node _root;

        // {0} - label, {1} - code
        const string FORMAT = "{0,-10}{1}";
        // Result code
        string _generatedCode;

        string _codeSegment;

        string _dataSegment;
        // using to create unique labels
        int _labelCounter;
        // this identifier cannot be equal variable name
        string _procedureIdentifier;
        // using for declaring in .data segment
        List<string> _variableIdentifiers;

        string _errorList;
        #endregion

        #region Methods
        public CodeGenerator(InfoTable infoTable, Node root)
        {
            _infoTable = infoTable;
            _root = root;
        }

        void GenerateCode(ref string place, string code, string label = "")
        {
            place += string.Format(FORMAT, label, code + "\r\n");
        }

        public string Generate()
        {
            GenerateCode(ref _generatedCode, ".model small");
            GenerateCode(ref _generatedCode, ".stack 100h");

            GenerateCode(ref _codeSegment, ".code");
            GenerateCode(ref _dataSegment, ".data");

            _variableIdentifiers = new List<string>();
            _labelCounter = 0;
            _errorList = string.Empty;

            program(_root.Children.Last());
            // Define variables
            if (_variableIdentifiers.Count > 0)
                foreach (var varIdent in _variableIdentifiers)
                    GenerateCode(ref _dataSegment, "\t" + varIdent + " DB 0");

            _generatedCode += _dataSegment + _codeSegment;

            if (string.IsNullOrEmpty(_errorList))
                return _generatedCode;
            else
                return null;
        }

        private void AddError(string message)
        {
            _errorList += "Error: " + message + "\n";
        }

        string GetLabel()
        {
            return "?L" + _labelCounter++;
        }

        void program(Node node)
        {
            string programNameLabel = procedure_identifier(node
                .Children
                .Single(i => i.Value == "<Procedure-identifier>"));
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "nop", programNameLabel + ':');

            block(node.Children.Single(i => i.Value == "<Block>"));
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "end " + programNameLabel);
        }

        void block(Node node)
        {
            statements_list(node.Children.Single(i => i.Value == "<Statements-list>"));
        }

        void statements_list(Node node)
        {
            if (node.Children.Last().Value == "<Empty>")
                return;

            statement(node.Children.Single(i => i.Value == "<Statement>"));
            if (!string.IsNullOrEmpty(_errorList))
                return;

            statements_list(node.Children.Single(i => i.Value == "<Statements-list>"));
        }

        void statement(Node node)
        {
            if (node.Children[0].Value == "<Condition-statement>") // Verify if it`s a condition statement
            {
                condition_statement(node.Children[0]);
            }
            else // if not - it`s a while loop
            {
                GenerateCode(ref _codeSegment, "; while"); // com

                var label1 = GetLabel();
                var label2 = GetLabel();

                GenerateCode(ref _codeSegment, "nop", label1 + ':');

                conditional_expression(node.Children
                    .Single(i => i.Value == "<Conditional-expression>")
                    , label2);
                if (!string.IsNullOrEmpty(_errorList))
                    return;

                GenerateCode(ref _codeSegment, "; do");// com

                statements_list(node.Children
                    .Single(i => i.Value == "<Statements-list>"));
                if (!string.IsNullOrEmpty(_errorList))
                    return;

                GenerateCode(ref _codeSegment, "JMP " + label1);
                GenerateCode(ref _codeSegment, "nop", label2 + ':');

                GenerateCode(ref _codeSegment, "; endwhile"); // com
            }
        }

        void condition_statement(Node node)
        {
            var label1 = GetLabel();
            var label2 = GetLabel();
            
            // if ... then branch
            incomplete_condition_statement(node.Children
                .Single(i => i.Value == "<Incomplete-condition-statement>")
                , label1);
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "JMP " + label2);
            GenerateCode(ref _codeSegment, "nop", label1 + ':');

            GenerateCode(ref _codeSegment, "; else"); // com

            // else branch
            alternative_part(node.Children
                .Single(i => i.Value == "<Alternative-part>"));
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "nop", label2 + ':');

            GenerateCode(ref _codeSegment, "; endif"); // com
        }

        void incomplete_condition_statement(Node node, string label1)
        {
            GenerateCode(ref _codeSegment, "; if"); // com
            // condition
            conditional_expression(node.Children
                .Single(i => i.Value == "<Conditional-expression>")
                , label1);
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "; then"); // com
            // then branch
            statements_list(node.Children
                .Single(i => i.Value == "<Statements-list>"));
        }

        void alternative_part(Node node)
        {
            if (node.Children.Last().Value == "<Empty>")
                return;

            statements_list(node.Children
                .Single(i => i.Value == "<Statements-list>"));
        }

        void conditional_expression(Node node, string label)
        {
            expression(node.Children
                .First(i => i.Value == "<Expression>")
                , "ax");
            if (!string.IsNullOrEmpty(_errorList))
                return;

            int signCode = comparison_operator(node.Children
                .Single(i => i.Value == "<Comparison-operator>"));

            expression(node.Children
                .Last(i => i.Value == "<Expression>")
                , "bx");
            if (!string.IsNullOrEmpty(_errorList))
                return;

            GenerateCode(ref _codeSegment, "cmp ax, bx");

            if (signCode == _infoTable.ShortDelimiters[">"])
                GenerateCode(ref _codeSegment, "JLE " + label);
            else if (signCode == _infoTable.LongDelimiters[">="])
                GenerateCode(ref _codeSegment, "JL " + label);
            else if (signCode == _infoTable.ShortDelimiters["="])
                GenerateCode(ref _codeSegment, "JNE " + label);
            else if (signCode == _infoTable.LongDelimiters["<>"])
                GenerateCode(ref _codeSegment, "JE " + label);
            else if (signCode == _infoTable.ShortDelimiters["<"])
                GenerateCode(ref _codeSegment, "JGE " + label);
            else if (signCode == _infoTable.LongDelimiters["<="])
                GenerateCode(ref _codeSegment, "JG " + label);
        }

        int comparison_operator(Node node)
        {
            return int.Parse(node.Children.Last().Value);
        }

        void expression(Node node, string reg)
        {
            if (node.Children.Last().Value == "<Variable-identifier>")
            {
                GenerateCode(ref _codeSegment,
                    "mov " + reg + ", " + variable_identifier(node.Children.Last()));
            }
            else
            {
                GenerateCode(ref _codeSegment,
                    "mov " + reg + ", " + unsigned_integer(node.Children.Last()));
            }
        }

        string procedure_identifier(Node node)
        {
            _procedureIdentifier = identifier(node.Children.Last());
            return _procedureIdentifier;
        }

        string variable_identifier(Node node)
        {
            var varIdent = identifier(node.Children.Last());
            if (varIdent == _procedureIdentifier)
            {
                AddError("Variale identifier name '" + varIdent +
                    "' cannot be equal program name");
                return ""; 
            }

            if (!_variableIdentifiers.Contains(varIdent))
                _variableIdentifiers.Add(varIdent);

            return varIdent;
        }

        string identifier(Node node)
        {
            return _infoTable.GetKey(int.Parse(node.Children.Last().Value));
        }

        string unsigned_integer(Node node)
        {
            return _infoTable.GetKey(int.Parse(node.Children.Last().Value));
        }

        public string GetErrorList()
        {
            if (string.IsNullOrEmpty(_errorList))
                return "Code generator: Error list is empty!\n";
            else
                return "Code generator: " + _errorList;
        }
        #endregion

    }
}
