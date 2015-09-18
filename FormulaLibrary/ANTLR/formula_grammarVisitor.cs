//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from formula_grammar.g by ANTLR 4.5.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="formula_grammarParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.1")]
[System.CLSCompliant(false)]
public interface Iformula_grammarVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondition([NotNull] formula_grammarParser.ConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.formula"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFormula([NotNull] formula_grammarParser.FormulaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.disjunction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDisjunction([NotNull] formula_grammarParser.DisjunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.conjunction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConjunction([NotNull] formula_grammarParser.ConjunctionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.negation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNegation([NotNull] formula_grammarParser.NegationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.predicate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicate([NotNull] formula_grammarParser.PredicateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.predicateTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicateTuple([NotNull] formula_grammarParser.PredicateTupleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTerm([NotNull] formula_grammarParser.TermContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.literal_term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteral_term([NotNull] formula_grammarParser.Literal_termContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.variable_term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable_term([NotNull] formula_grammarParser.Variable_termContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.varValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarValue([NotNull] formula_grammarParser.VarValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier([NotNull] formula_grammarParser.IdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.varType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarType([NotNull] formula_grammarParser.VarTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.simple_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSimple_type([NotNull] formula_grammarParser.Simple_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.numeric_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumeric_type([NotNull] formula_grammarParser.Numeric_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.integral_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIntegral_type([NotNull] formula_grammarParser.Integral_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.floating_point_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFloating_point_type([NotNull] formula_grammarParser.Floating_point_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.nullable_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNullable_type([NotNull] formula_grammarParser.Nullable_typeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="formula_grammarParser.string_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString_type([NotNull] formula_grammarParser.String_typeContext context);
}