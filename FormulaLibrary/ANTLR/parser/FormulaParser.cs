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
using System.IO;
using System.Text;
using System;

namespace FormulaLibrary.ANTLR
{
    public static class FormulaParser
    {
        /// <summary>
        /// Convert a formula in the form of string to a <typeparamref name="Formula"/>
        /// </summary>
        /// <param name="formula">a formula in the form of string</param>
        /// <returns></returns>
        public static Formula Parse(string formula)
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


        public static Formula ConvertXMLtoFormula()
        {
            return null;
        }

    }
}
