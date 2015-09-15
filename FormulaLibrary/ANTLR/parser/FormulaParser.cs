/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FormulaLibrary.ANTLR.visitor;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FormulaLibrary.ANTLR
{
    public static class FormulaParser
    {
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Convert a formula in the form of string to a <typeparamref name="Formula"/>
        /// </summary>
        /// <param name="formula">a formula in the form of string</param>
        /// <returns></returns>
        public static Formula ConvertStringToFormula(string formula)
        {
            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(/*RemoveWhitespace(formula)*/formula);

            MemoryStream m_stream = new MemoryStream(byteArray);

            // convert stream to string
            StreamReader reader = new StreamReader(m_stream);

            AntlrInputStream stream = new AntlrInputStream(reader);
            ITokenSource lexer = new formula_grammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            formula_grammarParser parser = new formula_grammarParser(tokens);
            parser.BuildParseTree = true;
            
            IParseTree tree = parser.disjunction();
            FormulaVisitor vv = new FormulaVisitor();

            return vv.Visit(tree);
        }


        public static Formula ConvertXMLtoFormula()
        {
            return null;
        }

    }
}
