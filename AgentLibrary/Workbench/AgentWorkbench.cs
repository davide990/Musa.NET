//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentWorkbench.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2015 Davide Guastella
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

using FormulaLibrary;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Runtime.Serialization;
using MusaCommon;


namespace AgentLibrary
{
    /// <summary>
    /// This enum is used to define the behaviour of a workbench when a formula is added to it
    /// </summary>
    public enum WorkbenchAddFormulaPolicy
    {
        NoAction,
        ThrowException
    }

    /// <summary>
    /// This enum is used to define the behaviour of a workbench when a formula is removed from it
    /// </summary>
    public enum WorkbenchRemoveFormulaPolicy
    {
        NoAction,
        ThrowException
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class AgentWorkbench
    {
        WorkbenchAddFormulaPolicy add_policy = WorkbenchAddFormulaPolicy.NoAction;
        WorkbenchRemoveFormulaPolicy remove_policy = WorkbenchRemoveFormulaPolicy.NoAction;

        /// <summary>
        /// The set of formula
        /// </summary>
        [DataMember]
        public ObservableCollection<AtomicFormula> Statements
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of assignment
        /// </summary>
        [DataMember]
        public ObservableCollection<AssignmentType> AssignmentSet
        {
            get;
            private set;
        }


        /// <summary>
        /// The agent this workbench belongs to
        /// </summary>
        private readonly Agent parentAgent;
        
        private ILogger logger;

        /// <summary>
        /// Create a new agent workbench
        /// </summary>
        public AgentWorkbench(Agent agent)
        {
            parentAgent = agent;
            Statements = new ObservableCollection<AtomicFormula>();
            AssignmentSet = new ObservableCollection<AssignmentType>();

            Statements.CollectionChanged += on_workbench_changed;
            AssignmentSet.CollectionChanged += on_assignment_set_changed;

            /*logger = MusaConfig.GetLogger();*/
            logger = ModuleProvider.Get().Resolve<ILogger>();
        }

        /// <summary>
        /// This is invoked every time a change occurs into the formula collection
        /// </summary>
        private void on_assignment_set_changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkMagenta);

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Added assignment: " + item);
                    break;  

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Removed assignment: " + item);
                    break;
            }
        }

        /// <summary>
        /// This is invoked every time a change occurs into the formula collection
        /// </summary>
        private void on_workbench_changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.DarkMagenta);
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Belief added: " + item);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        logger.Log(LogLevel.Trace, "[" + parentAgent.Name + "] Belief removed: " + item);
                    break;
            }
        }


        /// <summary>
        /// Check if this workbench contains a specific atomic formula
        /// </summary>
        public bool containsFormula(AtomicFormula f)
        {
            return Statements.Contains(f);
        }

        /// <summary>
        /// Add multiple statements to this workbench.
        /// </summary>
        /// <param name="f">A list of AtomicFormula to be added</param>
        public void AddStatement(IList f)
        {
            foreach (AtomicFormula ff in f)
                AddStatement(ff);
        }

        public void AddStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
                AddStatement(FormulaUtils.UnrollFormula(ff));
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void AddStatement(params AtomicFormula[] f)
        {
            addOrUpdateStatement(false, f);
            /*List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                //Convert ff to a simple (non parametric) formula, and get its variable terms as list
                variableTerms = ff.ConvertToSimpleFormula();

                //Continue if the statements set contains already the formula [ff]
                if (Statements.Contains(ff))
                    continue;

                //Iterate the formula's variable terms
                foreach (object varTerm in variableTerms)
                {
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(varTerm.GetType().GetGenericArguments()[0]);
                    
                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    //Add the assignment to the assignment set
                    AddAssignment((string)name, value);
                    //AssignmentSet.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this workbench
                Statements.Add(ff);
            }*/
        }

        private void addOrUpdateStatement(bool update = false, params AtomicFormula[] f)
        {
            List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                //Convert ff to a simple (non parametric) formula, and get its variable terms as list
                variableTerms = ff.ConvertToSimpleFormula();

                if (Statements.Contains(ff) && update)
                    RemoveStatement(ff);

                //Iterate the formula's variable terms
                foreach (object varTerm in variableTerms)
                {
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(varTerm.GetType().GetGenericArguments()[0]);

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    //Add the assignment to the assignment set
                    if (update)
                        UpdateAssignment((string)name, value);
                    else
                        AddAssignment((string)name, value);
                    //AssignmentSet.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this workbench
                Statements.Add(ff);
            }
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void RemoveStatement(IList f)
        {
            foreach (AtomicFormula ff in f)
                RemoveStatement(ff);
        }

        /// <summary>
        /// Remove a statement from this workbench
        /// </summary>
        public void RemoveStatement(params Formula[] f)
        {
            foreach (Formula ff in f)
                RemoveStatement(FormulaUtils.UnrollFormula(ff));
        }

        /// <summary>
        /// Remove a statement from this workbench
        /// </summary>
        public void RemoveStatement(params AtomicFormula[] f)
        {
            foreach (AtomicFormula ff in f)
            {
                ff.ConvertToSimpleFormula();

                if (Statements.Contains(ff))
                    Statements.Remove(ff);
            }
        }

        /// <summary>
        /// Add multiple statements to this workbench.
        /// </summary>
        /// <param name="f">A list of AtomicFormula to be added</param>
        public void UpdateStatement(IList f)
        {
            foreach (AtomicFormula ff in f)
                UpdateStatement(ff);
        }

        public void UpdateStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
                UpdateStatement(FormulaUtils.UnrollFormula(ff));
        }

        public void UpdateStatement(params AtomicFormula[] f)
        {
            addOrUpdateStatement(true, f);
        }

        #region Test condition methods

        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <returns>True if formula is satisfied in this workbench</returns>
        public bool TestCondition(Formula formula, out List<AssignmentType> generatedAssignment)
        {
            if (formula is NotFormula)
                return !TestCondition((formula as NotFormula).Formula, out generatedAssignment);
            else if (formula is OrFormula)
            {
                List<AssignmentType> l = null, r = null;
                bool left_result = TestCondition((formula as OrFormula).Left, out l);
                bool right_result = TestCondition((formula as OrFormula).Right, out r);
                generatedAssignment = new List<AssignmentType>(l);
                generatedAssignment.AddRange(r);
                return left_result | right_result;
            }
            else if (formula is AndFormula)
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
        public bool TestCondition(IFormula formula)
        {
            if (formula is NotFormula)
                return !TestCondition((formula as NotFormula).Formula);
            else if (formula is OrFormula)
                return TestCondition((formula as OrFormula).Left) | TestCondition((formula as OrFormula).Right);
            else if (formula is AndFormula)
                return TestCondition((formula as AndFormula).Left) & TestCondition((formula as AndFormula).Right);
            else
            {
                List<AssignmentType> l;
                return testCondition(formula as AtomicFormula, out l);
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

            foreach (AtomicFormula belief in Statements)
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
                        if (b.GetType().IsGenericType)
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
                        if (b is LiteralTerm)
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

        #endregion Test condition methods

        #region Assignment methods

        /// <summary>
        /// Set a value for a term by creating a specific assignment
        /// </summary>
        /// <param name="termName"></param>
        /// <param name="value"></param>
        public void AddAssignment(string termName, object value)
        {
            AssignmentType a = AssignmentType.CreateAssignmentForTerm(termName, value, value.GetType());

            if (!AssignmentSet.Contains(a))
                AssignmentSet.Add(a);
        }

        /// <summary>
        /// Check if this workbench contains an assignment for a given (literal) term
        /// </summary>
        /// <param name="termName">Term name.</param>
        public bool ExistsAssignmentForTerm(string termName)
        {
            foreach (var assignment in AssignmentSet)
            {
                if (assignment.Name.Equals(termName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if this workbench contains an assignment for a given (literal) term
        /// </summary>
        /// <returns><c>true</c>, if an assignment for the specified term 
        /// exists, <c>false</c> otherwise.</returns>
        /// <param name="termName">Term name.</param>
        /// <param name="assignmentValue">Assignment value.</param>
        public bool ExistsAssignmentForTerm(string termName, out object assignmentValue)
        {
            foreach (object assignment in AssignmentSet)
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

        /// <summary>
        /// Gets the assignment for the given term name. If no assignment exists, returns null.
        /// </summary>
        /// <returns>The assignment for term.</returns>
        /// <param name="termName">Term name.</param>
        public AssignmentType GetAssignmentForTerm(string termName)
        {
            foreach (object assignment in AssignmentSet)
            {
                if ((assignment as AssignmentType).Name.Equals(termName))
                {
                    Type varTermType = typeof(Assignment<>).MakeGenericType(assignment.GetType().GetGenericArguments()[0]);
                    object assignmentValue = varTermType.GetProperty("Value").GetValue(assignment, null);

                    return AssignmentType.CreateAssignmentForTerm(termName, assignmentValue, varTermType);
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the assignment related to the given term name.
        /// </summary>
        /// <param name="termName">Term name.</param>
        public void RemoveAssignment(string termName)
        {
            AssignmentType toRemove = GetAssignmentForTerm(termName);

            if (toRemove != null)
                AssignmentSet.Remove(toRemove);
        }

        /// <summary>
        /// Updates an assignment's value.
        /// </summary>
        /// <param name="termName">Term name.</param>
        /// <param name="newValue">New value.</param>
        public void UpdateAssignment(string termName, object newValue)
        {
            //remove the old assignment, if exists
            RemoveAssignment(termName);

            //add the new assignment
            AddAssignment(termName, newValue);
        }

        #endregion Assignment methods

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

            for (byte i = 0; i < Statements.Count; i++)
            {
                b.Append(Statements[i]);
                if (i != Statements.Count - 1)
                    b.Append("; ");
            }

            return b.ToString();
        }

    }
}