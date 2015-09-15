/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace FormulaLibrary.ANTLR.visitor
{
    public class FormulaVisitor : formula_grammarBaseVisitor<Formula>
    {
        public override Formula VisitFormula([NotNull] formula_grammarParser.FormulaContext context)
        {
            base.VisitFormula(context);
            return VisitDisjunction(context.disjunction());
        }

        public override Formula VisitDisjunction([NotNull] formula_grammarParser.DisjunctionContext context)
        {
            base.VisitDisjunction(context);

            if (context.ChildCount > 1)
                return new OrFormula(VisitConjunction(context.conjunction(0)), VisitConjunction(context.conjunction(1)));

            return VisitConjunction(context.conjunction(0));
        }

        public override Formula VisitConjunction([NotNull] formula_grammarParser.ConjunctionContext context)
        {
            base.VisitConjunction(context);

            if (context.ChildCount > 1)
                return new AndFormula(VisitNegation(context.negation(0)), VisitNegation(context.negation(1)));

            return VisitNegation(context.negation(0));
        }

        public override Formula VisitNegation([NotNull] formula_grammarParser.NegationContext context)
        {
            base.VisitNegation(context);
            bool isNegated = context.NOT() != null;
            IParseTree tree = null;

            if ((tree = context.predicate()) != null)
            {
                PredicateVisitor predicateVisitor = new PredicateVisitor();
                predicateVisitor.Visit(tree);
                
                if (isNegated)
                    return new NotFormula(predicateVisitor.ToAtomicFormula());
                else
                    return predicateVisitor.ToAtomicFormula();
            }
            else if((tree = context.formula()) != null)
            {
                if (isNegated)
                    return new NotFormula(VisitFormula(context.formula()));
                else
                    return VisitFormula(context.formula());
            }
            return null;
        }

        
    }
}
