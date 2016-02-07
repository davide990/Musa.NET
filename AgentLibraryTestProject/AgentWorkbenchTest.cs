using AgentLibrary;
using FormulaLibrary.ANTLR;
using FormulaLibrary;
using NUnit.Framework;
using MusaCommon;

namespace AgentLibraryTest
{
    [TestFixture]
    public class AgentWorkbenchTest
    {
        [TestCase("y(x <-int(3))", Result = true)]
        [TestCase("k(x)", Result = false)]
        [TestCase("k(x,y)", Result = false)]
        [TestCase("y(x,y)", Result = false)]
        [TestCase("y(x)", Result = true)]
        [TestCase("o(l<-int(2),k)", Result = true)]
        [TestCase("h(s,o,a)", Result = true)]
        [Test]
        public bool test_simple(string formula)
        {
            var FormulaParser = new FormulaParser();
            AgentWorkbench wb = new AgentWorkbench(null);
            wb.AddStatement(FormulaParser.Parse("y(x<-int(3))") as AtomicFormula,
                            FormulaParser.Parse("f(x)") as AtomicFormula,
                            FormulaParser.Parse("h(s,o,a<-string(\"ciao mondo\"))") as AtomicFormula,
                            FormulaParser.Parse("o(m,s<-char('d')") as AtomicFormula);

            AtomicFormula test = FormulaParser.Parse(formula) as AtomicFormula;
            
            return wb.TestCondition(test);
        }

        [TestCase("y(x)&f(x)", Result = true)]
        [TestCase("f(x)&o(m,k)", Result = true)]
        [TestCase("f(x)&o(m)", Result = false)]
        [TestCase("f(x)|o(m)", Result = true)]
        [TestCase("!f(x,u,a)", Result = true)]
        [TestCase("!f(x,u,a)&(y(l)|!(l(q,w,e,r,t,y)))", Result = true)]
        [Test]
        public bool test_composite(string formula)
        {
            var FormulaParser = new FormulaParser();
            AgentWorkbench wb = new AgentWorkbench(null);
            wb.AddStatement(FormulaParser.Parse("y(x<-int(3))") as AtomicFormula,
                            FormulaParser.Parse("f(x)") as AtomicFormula,
                            FormulaParser.Parse("h(s,o,a<-string(\"ciao mondo\"))") as AtomicFormula,
                            FormulaParser.Parse("o(m,s<-char('d')") as AtomicFormula);

            IFormula test = FormulaParser.Parse(formula);

            return wb.TestCondition(test);
        }

        /// <summary>
        /// Unroll a generic formula and add its inner atomic formulas to an agent workbench
        /// </summary>
        [TestCase("!(((f(w)&j(g))|(t(r)&m(w))))", Result = 4)]
        [TestCase("(((l(e)|c(pghy<-float(0.7339609)))&(d(y)|g(v)))|!((d(uvrh<-bool(\"false\"))|g(gjes<-bool(\"false\")))))", Result = 6)]
        [TestCase("(!(!(x(l)))|((r(g)&r(l))&(l(w)|v(xrov<-float(0.258401)))))", Result = 5)]
        [TestCase("(!((x(p)&j(suni<-bool(\"false\"))))&((r(o)&d(nlzx<-float(0.3891769)))&!(b(mbcv<-bool(\"true\")))))", Result = 5)]
        [TestCase("!((!(e(q))|(h(l)|s(t))))", Result = 3)]
        [TestCase("(!((w(oqer<-float(0.2587696))|q(yhfm<-int(772061452))))&(!(m(h))&(z(i)|s(g))))", Result = 5)]
        [TestCase("((!(v(ovgb<-bool(\"true\")))|(c(c)|f(m)))|((u(pmgg<-int(44924880))|u(jqqs<-bool(\"true\")))&(o(hyrz<-bool(\"false\"))&m(t))))", Result = 7)]
        [TestCase("(!((e(u)|p(t)))&!(!(p(lnlz<-string(\"p\")))))", Result = 3)]
        [TestCase("(!((f(v)|x(mmui<-bool(\"true\"))))|((x(h)&m(s))|(t(w)&h(jylw<-string(\"c\")))))", Result = 6)]
        [TestCase("(((f(rjpf<-string(\"h\"))&t(e))&(j(p)|n(z)))|!((j(q)|w(ycuz<-int(1541384213)))))", Result = 6)]
        [TestCase("(!(!(x(g)))&((j(cqyr<-float(0.3891904))&b(r))|(f(kxgy<-bool(\"true\"))&k(s))))", Result = 5)]
        [TestCase("!(((r(frnw<-int(16347287))&l(cyfp<-string(\"o\")))&!(c(k))))", Result = 3)]
        [TestCase("!((!(j(tsly<-int(2044629610)))&!(z(k))))", Result = 2)]
        [TestCase("!f(x,u,a)&(y(l)|!(l(q,w,e,r,t,y)))", Result = 3)]
        [Test]
        public int unroll_formula_test(string formula)
        {
            var FormulaParser = new FormulaParser();
            AgentWorkbench wb = new AgentWorkbench(null);
            wb.AddStatement(FormulaParser.Parse(formula));
            return wb.Statements.Count;
        }
    }
}
