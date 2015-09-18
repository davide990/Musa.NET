using FormulaLibrary;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections.ObjectModel;
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


    // Quando una formula viene aggiunta e questa contiene termini variabili, si dovrebbe valutare la possibilità di convertire tutti i termini variabili in letterali
    // e inserire i valori come assignment
    public class AgentWorkbench
    {
        WorkbenchAddFormulaPolicy add_policy        = WorkbenchAddFormulaPolicy.Default;
        WorkbenchRemoveFormulaPolicy remove_policy  = WorkbenchRemoveFormulaPolicy.Default;
        
        /// <summary>
        /// The set of formula
        /// </summary>
        private ObservableCollection<AtomicFormula> workbench;

        private List<AssignmentType> assignment_set;

        /// <summary>
        /// Create a new workbench
        /// </summary>
        public AgentWorkbench()
        {
            workbench       = new ObservableCollection<AtomicFormula>();
            assignment_set  = new List<AssignmentType>();

            workbench.CollectionChanged += on_workbench_changed;
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
            return workbench.Contains(f);
        }
        
        /// <summary>
        /// Add a belief (as atomic formula) into this workbench.
        /// </summary>
        public void addCondition(params AtomicFormula[] f)
        {
            List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                variableTerms = ff.ConvertToSimpleFormula();

                //
                if (workbench.Contains(ff))
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

                    assignment_set.Add((AssignmentType)createAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this workbench
                workbench.Add(ff);
            }
                
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
                return testCondition(formula as AtomicFormula);
        }

        /// <summary>
        /// Test the parametric condition f into this workbench. Since it is
        /// a parametric formula, at least one term has an associated value in 
        /// form of assignment, contained in this workbench.
        /// </summary>
        /// <returns>True if f is satisfied in this workbench</returns>
        private bool testCondition(AtomicFormula f)
        {
            Term a, b;
            bool success = true;
            List<AssignmentType> generatedAssignment = new List<AssignmentType>();

            bool belief_term_has_assignment;

            foreach (AtomicFormula belief in workbench)
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
                            generatedAssignment.Add((AssignmentType)createAssignmentForTerm(b.Name, a_value, a_value.GetType()));
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
                            generatedAssignment.Add((AssignmentType)createAssignmentForTerm(a.Name, b_value, type_of_b.GetGenericArguments()[0]));
                        }
                    }
                }
                if (success)
                    return true;
            }
            return false;
        }
        

        /// <summary>
        /// Create a new assignment
        /// </summary>
        /// <returns></returns>
        private object createAssignmentForTerm(string assignmentName, object value, Type type)
        {
            Type varTermType = typeof(Assignment<>).MakeGenericType(type);
            object theAssignment = Activator.CreateInstance(varTermType, assignmentName, value);

            return theAssignment;
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

    }
}