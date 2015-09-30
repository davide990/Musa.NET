using FormulaLibrary;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Runtime.Serialization;
/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
*/
namespace AgentLibrary
{
    /// <summary>
    /// This enum is used to define the behaviour of a workbench when a formula is added to it
    /// </summary>
    public enum WorkbenchAddFormulaPolicy
    {
        Default,
        ThrowException
    }

    /// <summary>
    /// This enum is used to define the behaviour of a workbench when a formula is removed from it
    /// </summary>
    public enum WorkbenchRemoveFormulaPolicy
    {
        Default,
        ThrowException
    }



    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [KnownType(typeof(Formula))]
    [KnownType(typeof(AtomicFormula))]
    [KnownType(typeof(AssignmentType))]
    public class AgentWorkbench
    {
        WorkbenchAddFormulaPolicy add_policy        = WorkbenchAddFormulaPolicy.Default;
        WorkbenchRemoveFormulaPolicy remove_policy  = WorkbenchRemoveFormulaPolicy.Default;
        
        /// <summary>
        /// The set of formula
        /// </summary>
        [DataMember]
        public ObservableCollection<AtomicFormula> Statements
        {
            get { return statements; }
            private set { statements = value; }
        }
        private ObservableCollection<AtomicFormula> statements;

        /// <summary>
        /// The set of assignment
        /// </summary>
        [DataMember]
        public ObservableCollection<AssignmentType> AssignmentSet
        {
            get { return assignment_set; }
            private set { assignment_set = value; }
        }
        private ObservableCollection<AssignmentType> assignment_set;

        /// <summary>
        /// The agent this workbench belongs to
        /// </summary>
        private readonly Agent parentAgent;
        
        /// <summary>
        /// Create a new agent workbench
        /// </summary>
        public AgentWorkbench(Agent agent)
        {
            parentAgent     = agent;
            statements       = new ObservableCollection<AtomicFormula>();
            assignment_set  = new ObservableCollection<AssignmentType>();

            assignment_set.CollectionChanged    += on_assignment_set_changed;
            statements.CollectionChanged        += on_workbench_changed;
        }

        /// <summary>
        /// This is invoked every time a change occurs into the formula collection
        /// </summary>
        private void on_assignment_set_changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            object assignment = e.NewItems[0];
            Type assignmentType = typeof(VariableTerm<>).MakeGenericType(assignment.GetType().GetGenericArguments()[0]);
            

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Console.WriteLine("[WORKBENCH] ITEM ADDED");
                    break;  

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    //ACTION ON REMOVE FORMULA
                    //Console.WriteLine(e.OldItems[0].ToString());

                    break;
            }
        }

        /// <summary>
        /// This is invoked every time a change occurs into the formula collection
        /// </summary>
        private void on_workbench_changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    //ACTION ON ADD FORMULA
                    //Console.WriteLine(e.NewItems[0].ToString());

                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    //ACTION ON REMOVE FORMULA
                    //Console.WriteLine(e.OldItems[0].ToString());

                    break;
            }
        }


        /// <summary>
        /// Check if this workbench contains a specific atomic formula
        /// </summary>
        public bool containsFormula(AtomicFormula f)
        {
            return statements.Contains(f);
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void addStatement(IList f)
        {
            foreach(AtomicFormula ff in f)
                addStatement(ff);
        }
        
        /// <summary>
        /// Given a generic formula, return its inner atomic formulas.
        /// </summary>
        private List<AtomicFormula> UnrollFormula(Formula f)
        {
            List<AtomicFormula> ff = new List<AtomicFormula>();

            if (f is AndFormula)
            {
                ff.AddRange(UnrollFormula((f as AndFormula).Left));
                ff.AddRange(UnrollFormula((f as AndFormula).Right));
            }
            else if (f is OrFormula)
            {
                ff.AddRange(UnrollFormula((f as OrFormula).Left));
                ff.AddRange(UnrollFormula((f as OrFormula).Right));
            }
            else if(f is NotFormula)
            {
                ff.AddRange(UnrollFormula((f as NotFormula).Formula));
            }
            else
                ff.Add(f as AtomicFormula);

            return ff;
        }

        public void addStatement(params Formula[] f)
        {
            foreach(Formula ff in f)
                addStatement(UnrollFormula(ff));
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void addStatement(params AtomicFormula[] f)
        {
            List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                variableTerms = ff.ConvertToSimpleFormula();

                if (statements.Contains(ff))
                    continue;

                foreach (object varTerm in variableTerms)
                {
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(varTerm.GetType().GetGenericArguments()[0]);
                    ConstructorInfo cinfo = variableTermType.GetConstructor(new[] { typeof(string), varTerm.GetType().GetGenericArguments()[0] });

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    assignment_set.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this workbench
                statements.Add(ff);
            }
        }


        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void removeStatement(IList f)
        {
            foreach (AtomicFormula ff in f)
                removeStatement(ff);
        }


        /// <summary>
        /// Remove a statement from this workbench
        /// </summary>
        public void removeStatement(params AtomicFormula[] f)
        {
            foreach (AtomicFormula ff in f)
            {
                ff.ConvertToSimpleFormula();

                if (statements.Contains(ff))
                    statements.Remove(ff);
            }
        }


        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <returns>True if formula is satisfied in this workbench</returns>
        public bool TestCondition(Formula formula, out List<AssignmentType> generatedAssignment)
        {
            if (formula is NotFormula)
                return !TestCondition((formula as NotFormula).Formula, out generatedAssignment);
            else
            if (formula is OrFormula)
            {
                List<AssignmentType> l = null, r = null;
                bool left_result = TestCondition((formula as OrFormula).Left, out l);
                bool right_result = TestCondition((formula as OrFormula).Right, out r);
                generatedAssignment = new List<AssignmentType>(l);
                generatedAssignment.AddRange(r);
                return left_result | right_result;
            }
            else
            if (formula is AndFormula)
            {
                List<AssignmentType> l = null, r = null;
                bool left_result = TestCondition((formula as AndFormula).Left, out l);
                bool right_result = TestCondition((formula as AndFormula).Right, out r);
                generatedAssignment = new List<AssignmentType>(l);
                generatedAssignment.AddRange(r);
                return left_result & right_result;
            }
            else
                return testCondition(formula as AtomicFormula, out generatedAssignment);
        }

        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <returns>True if formula is satisfied in this workbench</returns>
        public bool TestCondition(Formula formula)
        {
            if (formula is NotFormula)
                return !TestCondition((formula as NotFormula).Formula);
            else
            if (formula is OrFormula)
                return TestCondition((formula as OrFormula).Left) | TestCondition((formula as OrFormula).Right);
            else
            if (formula is AndFormula)
                return TestCondition((formula as AndFormula).Left) & TestCondition((formula as AndFormula).Right);
            else
            {
                List<AssignmentType> l;
                return testCondition(formula as AtomicFormula, out l);
                l.Clear();
            }
        }

        /// <summary>
        /// Test the parametric condition f into this workbench. Since it is
        /// a parametric formula, at least one term has an associated value in 
        /// form of assignment, contained in this workbench.
        /// </summary>
        /// <returns>True if f is satisfied in this workbench</returns>
        private bool testCondition(AtomicFormula f, out List<AssignmentType> generatedAssignment)
        {
            Term a, b;
            bool success = true;
            generatedAssignment = new List<AssignmentType>();

            bool belief_term_has_assignment;

            foreach (AtomicFormula belief in statements)
            {
                if (belief.TermsCount != f.TermsCount || !belief.Functor.Equals(f.Functor))
                    continue;

                generatedAssignment.Clear();
                success = true;
                for (int i = 0; i < belief.TermsCount; i++)
                {
                    a = belief.Terms[i];
                    b = f.Terms[i];

                    object a_value = null;
                    belief_term_has_assignment = ExistsAssignmentForTerm(a.Name, out a_value);

                    if (belief_term_has_assignment)
                    {
                        if(b.GetType().IsGenericType)
                        {
                            //a and b are both variable 
                            //se sono uguali i valori procedi, altrimenti le formule sono diverse per contenuto. Esci
                            Type type_of_b = b.GetType();
                            object b_value = type_of_b.GetProperty("Value").GetValue(b);
                            if (a_value.Equals(b_value))
                                continue;
                            else
                            { 
                                success = false;
                                break;
                            }
                        }
                        else
                        {
                            //a is variable term, b is literal - CREA ASSIGNMENT
                            generatedAssignment.Add(AssignmentType.CreateAssignmentForTerm(b.Name, a_value, a_value.GetType()));
                        }
                    }
                    else
                    {
                        if(b is LiteralTerm)
                        {
                            //b and a are both literal
                            if (a.Equals(b))
                                continue;
                            else
                            {
                                success = false;
                                break;
                            }
                        }
                        else
                        {
                            //b is variable, a is literal - CREA ASSIGNMENT
                            Type type_of_b = b.GetType();
                            object b_value = type_of_b.GetProperty("Value").GetValue(b);
                            generatedAssignment.Add(AssignmentType.CreateAssignmentForTerm(a.Name, b_value, type_of_b.GetGenericArguments()[0]));
                        }
                    }
                }
                if (success)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Set a value for a term by creating a specific assignment
        /// </summary>
        /// <param name="termName"></param>
        /// <param name="value"></param>
        public void SetValueForTerm(string termName, object value)
        {
            AssignmentType a = AssignmentType.CreateAssignmentForTerm(termName, value, value.GetType());

            if (!assignment_set.Contains(a))
                assignment_set.Add(a);
        }

        /// <summary>
        /// Check if this workbench contains an assignment for a given (literal) term
        /// </summary>
        /// <param name="t"></param>
        public bool ExistsAssignmentForTerm(string termName)
        {
            foreach (var assignment in assignment_set)
            {
                if (assignment.Name.Equals(termName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if this workbench contains an assignment for a given (literal) term
        /// </summary>
        /// <param name="t"></param>
        public bool ExistsAssignmentForTerm(string termName, out object assignmentValue)
        {
            
            foreach (object assignment in assignment_set)
            {
                if ((assignment as AssignmentType).Name.Equals(termName))
                {
                    Type varTermType = typeof(Assignment<>).MakeGenericType(assignment.GetType().GetGenericArguments()[0]);
                    assignmentValue = varTermType.GetProperty("Value").GetValue(assignment, null);
                    
                    return true;
                }
            }

            assignmentValue = null;
            return false;
        }
        
        public void setAddFormulaPolicy(WorkbenchAddFormulaPolicy p)
        {
            add_policy = p;
        }

        public void setRemoveFormulaPolicy(WorkbenchRemoveFormulaPolicy p)
        {
            remove_policy = p;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("[" + parentAgent.Name + "] ");

            for (byte i = 0; i < statements.Count; i++)
            {
                b.Append(statements[i]);
                if (i != statements.Count - 1)
                    b.Append("; ");
            }

            return b.ToString();
        }

    }
}