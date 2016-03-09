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

namespace FormulaLibrary
{
    class PredicateVisitor : formula_grammarBaseVisitor<string>, IDisposable
    {
        /// <summary>
        /// The Terms of this predicate
        /// </summary>
        public List<Term> Terms
        {
            get;
            private set;
        }

        /// <summary>
        /// The functor of a parsed predicate
        /// </summary>
        public string Functor
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of predicate_visitor
        /// </summary>
        public PredicateVisitor()
        {
            Terms = new List<Term>();
        }

        /// <summary>
        /// Return an AtomicFormula object that corresponds to the predicate this visitor parses.
        /// </summary>
        public AtomicFormula ToAtomicFormula()
        {
            return new AtomicFormula(Functor, Terms);
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
            Terms.Add(lt);
            return "";
        }

        #region Valued Term

        public override string VisitThe_string_type(formula_grammarParser.The_string_typeContext context)
        {
            base.VisitThe_string_type(context);

            ValuedTerm<string> aa = new ValuedTerm<string>(context.value);
            Terms.Add(aa);
            return "";
        }

        public override string VisitThe_boolean_type(formula_grammarParser.The_boolean_typeContext context)
        {
            base.VisitThe_boolean_type(context);

            ValuedTerm<bool> aa = new ValuedTerm<bool>(context.value);
            Terms.Add(aa);
            return "";
        }

        public override string VisitThe_int_type(formula_grammarParser.The_int_typeContext context)
        {
            base.VisitThe_int_type(context);

            ValuedTerm<int> aa = new ValuedTerm<int>(context.value);
            Terms.Add(aa);    

            return "";
        }

        public override string VisitThe_char_type(formula_grammarParser.The_char_typeContext context)
        {
            base.VisitThe_char_type(context);

            ValuedTerm<char> aa = new ValuedTerm<char>(context.value);
            Terms.Add(aa);    

            return "";
        }

        public override string VisitThe_float_type(formula_grammarParser.The_float_typeContext context)
        {
            base.VisitThe_float_type(context);
            ValuedTerm<float> aa = new ValuedTerm<float>(context.value);
            Terms.Add(aa);    

            return "";
        }

        #endregion Valued Term

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

        public void Dispose()
        {
            Terms.Clear();
        }
    }
}
