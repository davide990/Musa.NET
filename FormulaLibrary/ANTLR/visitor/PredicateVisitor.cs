/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace FormulaLibrary.ANTLR.visitor
{
    class PredicateVisitor : formula_grammarBaseVisitor<string>, IDisposable
    {
        /// <summary>
        /// The terms of this predicate
        /// </summary>
        public List<Term> Terms
        {
            get { return terms; }
        }
        private List<Term> terms;

        /// <summary>
        /// The functor of a parsed predicate
        /// </summary>
        public string Functor
        {
            get { return functor; }
            private set { functor = value; }
        }
        private string functor;

        /// <summary>
        /// Creates a new instance of predicate_visitor
        /// </summary>
        public PredicateVisitor()
        {
            terms = new List<Term>();
        }
        
        /// <summary>
        /// Return an AtomicFormula object that corresponds to the predicate this visitor parses.
        /// </summary>
        public AtomicFormula ToAtomicFormula()
        {
            return new AtomicFormula(Functor, terms);
        }

        public override string VisitPredicate([NotNull] formula_grammarParser.PredicateContext context)
        {
            base.VisitPredicate(context);
            Functor = context.functor.expr;
            
            return "";
        }
        
        public override string VisitLiteral_term([NotNull] formula_grammarParser.Literal_termContext context)
        {
            base.VisitLiteral_term(context);
            LiteralTerm lt = new LiteralTerm(context.name.expr);
            terms.Add(lt);
            return "";
        }

        public override string VisitVariable_term([NotNull] formula_grammarParser.Variable_termContext context)
        {
            base.VisitVariable_term(context);

            //Create a new Type declaration using the specified type within the formula
            Type type = Type.GetType(Visit(context.varType().children[0]));

            //Get the term name and value
            //string str_value = context.value.expr;
            string str_value = null;
            if ((str_value = context.value.expr) == null)
                throw new Exception("Something went wrong in parsing formula.\n");

            string name = context.name.expr;

            //Create a new VariableTerm object
            Type genericVariableTermType = typeof(VariableTerm<>);
            Type varTermType = genericVariableTermType.MakeGenericType(type);
            object varTerm = Activator.CreateInstance(varTermType, name);

            //Cast the term value to the specified type
            object value = null;

            //In case the value is a char or a string, trim the initial and the ending (double)quotes
            TrimQuotesChar(ref str_value);

            if (typeof(string).Equals(type))
            {
                //In the simplest case, the type if a string. No casting operations are needed here
                varTermType.GetProperty("Value").SetValue(varTerm, str_value);
            }
            else
            {
                //Create a new object referencing to the class the value belongs to
                object termValueClassRef = Activator.CreateInstance(type);
                try
                {
                    //Try executing the "Parse" method to parse the string value to the right value type
                    MethodInfo parse_method = type.GetMethod("Parse", new[] { typeof(string), typeof(CultureInfo) });
                    value = parse_method.Invoke(termValueClassRef, new object[] { str_value, CultureInfo.InvariantCulture });
                }
                catch (NullReferenceException e)
                {
                    //a NullReferenceException exception can occour if the Parse method is not included within a given type reference class.
                    //In this case use a simple ChangeType to cast the string value to the right value type.
                    value = Convert.ChangeType(str_value, type);
                }
                finally
                {
                    //Finally, assign the value to the variable term
                    varTermType.GetProperty("Value").SetValue(varTerm, value);
                }
            }
            
            //Add the term to the terms list
            terms.Add((Term)varTerm);

            return "";
        }

        /// <summary>
        /// Trim (double)quotes from a string
        /// </summary>
        private void TrimQuotesChar(ref string str)
        {
            if (str.StartsWith("\"") || str.StartsWith("'"))
                str = str.Substring(1);

            if (str.EndsWith("\"") || str.EndsWith("'"))
                str = str.Substring(0, str.Length - 1);
        }


        public override string VisitSimple_type([NotNull] formula_grammarParser.Simple_typeContext context)
        {
            base.VisitSimple_type(context);
            return context.expr;
        }

        public override string VisitNullable_type([NotNull] formula_grammarParser.Nullable_typeContext context)
        {
            base.VisitNullable_type(context);
            return context.expr;
        }

        public void Dispose()
        {
            terms.Clear();
        }
    }
}
