//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  FormulaParser.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FormulaLibrary.ANTLR.visitor;
using System.IO;
using System.Text;
using MusaCommon;

namespace FormulaLibrary
{
    [Register(typeof(IFormulaParser))]
    public class FormulaParser : MusaModule, IFormulaParser
    {
        /// <summary>
        /// Convert a formula in the form of string to a <typeparamref name="Formula"/>
        /// </summary>
        /// <param name="formula">a formula in the form of string</param>
        /// <returns></returns>
        public IFormula Parse(string formula)
        {
            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(formula);

            MemoryStream m_stream = new MemoryStream(byteArray);

            // convert stream to string
            StreamReader reader = new StreamReader(m_stream);
            AntlrInputStream stream = new AntlrInputStream(reader);
            ITokenSource lexer = new formula_grammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            formula_grammarParser parser = new formula_grammarParser(tokens);

            //Invoke the parser
            IParseTree tree = parser.disjunction();	
            /*
			catch(Exception e)
			{
				if (e is InputMismatchException)
					Console.WriteLine ("Input '" + formula + "' is not a valid formula.\n" + e.Message);
				if (e is RecognitionException)
					Console.WriteLine ("Recognition exception with formula '" + formula + "'.\n" + e.Message);

				return null;
			}                
            */
            Formula formulaObject;
            using (FormulaVisitor vv = new FormulaVisitor())
            {
                //Visit the parse tree
                formulaObject = vv.Visit(tree);	     
            }

            stream.Reset();
            parser.Reset();

            return formulaObject;
        }

        public IFormula ConvertXMLtoFormula()
        {
            return null;
        }

    }
}
