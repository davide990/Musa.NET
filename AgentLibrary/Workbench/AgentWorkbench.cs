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

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Runtime.Serialization;
using MusaCommon;
using System.Linq;


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
    public class AgentWorkbench : IAgentWorkbench
    {
        WorkbenchAddFormulaPolicy add_policy = WorkbenchAddFormulaPolicy.NoAction;
        WorkbenchRemoveFormulaPolicy remove_policy = WorkbenchRemoveFormulaPolicy.NoAction;

        /// <summary>
        /// The set of formula
        /// </summary>
        [DataMember]
        public ObservableCollection<IFormula> Statements
        {
            get;
            private set;
        }

        /// <summary>
        /// The agent this workbench belongs to
        /// </summary>
        private readonly Agent parentAgent;

        private ILogger logger;
        private IAssignmentFactory assignmentFactory;
        private IFormulaUtils FormulaUtils;

        /// <summary>
        /// Create a new agent workbench
        /// </summary>
        public AgentWorkbench(Agent agent)
        {
            parentAgent = agent;
            Statements = new ObservableCollection<IFormula>();

            Statements.CollectionChanged += on_workbench_changed;

            logger = ModuleProvider.Get().Resolve<ILogger>();
            assignmentFactory = ModuleProvider.Get().Resolve<IAssignmentFactory>();
            FormulaUtils = ModuleProvider.Get().Resolve<IFormulaUtils>();
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
        public bool containsFormula(IFormula f)
        {
            return Statements.Contains(f);
        }

        /// <summary>
        /// Add multiple statements to this workbench.
        /// </summary>
        /// <param name="f">A list of AtomicFormula to be added</param>
        public void AddStatement(IList f)
        {
            foreach (IFormula ff in f)
                AddStatement(ff);
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void AddStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                if (!ff.IsAtomic())
                    addOrUpdateStatement(false, FormulaUtils.UnrollFormula(ff).ToArray());
                else
                    addOrUpdateStatement(false, ff as IAtomicFormula);
            }
        }

        private void addOrUpdateStatement(bool update = false, params IAtomicFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                //If the formula to be added is satisfied in this workbench, so it is
                //assignable from another one, the formula is not added
                if (TestCondition(ff) && !update)
                {
                    logger.Log(LogLevel.Warn, "[" + parentAgent.Name + "] Cannot add formula '" + ff
                        + "': is assignable from another formula in this workbench.");
                    continue;
                }


                if (Statements.Contains(ff) && update)
                    RemoveStatement(ff);

                //Add the formula to this workbench
                Statements.Add(ff);
            }
        }

        /// <summary>
        /// Add a statement (as atomic formula) into this workbench.
        /// </summary>
        public void RemoveStatement(IList f)
        {
            foreach (IFormula ff in f)
                RemoveStatement(ff);
        }

        /// <summary>
        /// Remove a statement from this workbench
        /// </summary>
        public void RemoveStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                if (!ff.IsAtomic())
                {
                    var unrolled = FormulaUtils.UnrollFormula(ff);

                    foreach (IFormula unrolledFormula in unrolled)
                    {
                        if (Statements.Contains(unrolledFormula))
                            Statements.Remove(unrolledFormula);
                    }
                    continue;
                }
                else
                {
                    if (Statements.Contains(ff))
                        Statements.Remove(ff);
                }
            }
        }

        /// <summary>
        /// Add multiple statements to this workbench.
        /// </summary>
        /// <param name="f">A list of AtomicFormula to be added</param>
        public void UpdateStatement(IList f)
        {
            foreach (IFormula ff in f)
                UpdateStatement(ff);
        }

        public void UpdateStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                if (ff.IsAtomic())
                {
                    addOrUpdateStatement(true, ff as IAtomicFormula);
                    continue;
                }

                addOrUpdateStatement(true, FormulaUtils.UnrollFormula(ff).ToArray());
            }
        }

        #region Test condition methods

        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <returns>True if formula is satisfied in this workbench</returns>
        public bool TestCondition(IFormula formula)
        {
            if (formula.GetFormulaType() == FormulaType.NOT_FORMULA)
                return !TestCondition((formula as INotFormula).GetFormula());
            else if (formula.GetFormulaType() == FormulaType.OR_FORMULA)
                return TestCondition((formula as IOrFormula).GetLeft()) | TestCondition((formula as IOrFormula).GetRight());
            else if (formula.GetFormulaType() == FormulaType.AND_FORMULA)
                return TestCondition((formula as IAndFormula).GetLeft()) & TestCondition((formula as IAndFormula).GetRight());
            else
            {
                List<IAssignment> l;
                //here, formula is atomic
                foreach (IFormula f in Statements)
                {
                    if (f.MatchWith(formula, out l))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <returns><c>true</c>, if condition was tested, <c>false</c> otherwise.</returns>
        /// <param name="formula">Formula.</param>
        /// <param name="generatedAssignment">Generated assignment.</param>
        public bool TestCondition(IFormula formula, out List<IAssignment> generatedAssignment)
        {
            if (formula.GetFormulaType() == FormulaType.NOT_FORMULA)
                return !TestCondition((formula as INotFormula).GetFormula(), out generatedAssignment);
            else if (formula.GetFormulaType() == FormulaType.OR_FORMULA)
            {
                List<IAssignment> rightAssignment, leftAssignment;
                bool leftResult = TestCondition((formula as IOrFormula).GetLeft(), out leftAssignment);
                bool rightResult = TestCondition((formula as IOrFormula).GetRight(), out rightAssignment);
                generatedAssignment = leftAssignment.Union(rightAssignment).ToList();
                return leftResult | rightResult;
            }
            else if (formula.GetFormulaType() == FormulaType.AND_FORMULA)
            {
                List<IAssignment> rightAssignment, leftAssignment;
                bool leftResult = TestCondition((formula as IAndFormula).GetLeft(), out leftAssignment);
                bool rightResult = TestCondition((formula as IAndFormula).GetRight(), out rightAssignment);
                generatedAssignment = leftAssignment.Union(rightAssignment).ToList();
                return leftResult & rightResult;
            }
            else
            {
                //here, formula is atomic
                foreach (IFormula f in Statements)
                {
                    if (f.MatchWith(formula, out generatedAssignment))
                        return true;
                }
                generatedAssignment = new List<IAssignment>();
                return false;
            }
        }

        /// <summary>
        /// Test if a formula is verified into this workbench.
        /// </summary>
        /// <param name="formula">The formula to be verified</param>
        /// <param name="unifiedPredicates">The list of predicates which match with the input formula when 
        /// unified with a specific set of assignments</param>
        /// <param name="generatedAssignment">The assignment set generated from this test</param>
        /// <returns>True if at lest one predicates match with the input formula, false otherwise.</returns>
        public bool TestCondition(IFormula formula, out List<IFormula> unifiedPredicates, out List<IAssignment> generatedAssignment)
        {
            if (formula.GetFormulaType() == FormulaType.NOT_FORMULA)
                return !TestCondition((formula as INotFormula).GetFormula(), out unifiedPredicates, out generatedAssignment);
            else if (formula.GetFormulaType() == FormulaType.OR_FORMULA)
            {
                List<IAssignment> rightAssignment, leftAssignment;
                bool leftResult = TestCondition((formula as IOrFormula).GetLeft(), out unifiedPredicates, out leftAssignment);
                bool rightResult = TestCondition((formula as IOrFormula).GetRight(), out unifiedPredicates, out rightAssignment);
                generatedAssignment = leftAssignment.Union(rightAssignment).ToList();
                return leftResult | rightResult;
            }
            else if (formula.GetFormulaType() == FormulaType.AND_FORMULA)
            {
                List<IAssignment> rightAssignment, leftAssignment;
                bool leftResult = TestCondition((formula as IAndFormula).GetLeft(), out unifiedPredicates, out leftAssignment);
                bool rightResult = TestCondition((formula as IAndFormula).GetRight(), out unifiedPredicates, out rightAssignment);
                generatedAssignment = leftAssignment.Union(rightAssignment).ToList();
                return leftResult & rightResult;
            }
            else
            {
                //here, formula is atomic
                unifiedPredicates = new List<IFormula>();
                generatedAssignment = new List<IAssignment>();
                bool success = false;

                foreach (IFormula f in Statements)
                {
                    List<IAssignment> current_assignments;
                    if (!f.MatchWith(formula, out current_assignments))
                        continue;

                    success = true;
                    generatedAssignment.AddRange(current_assignments);
                    unifiedPredicates.Add(f);
                }
                return success;
            }

        }
        #endregion Test condition methods

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