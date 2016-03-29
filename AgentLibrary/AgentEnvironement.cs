//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  AgentEnvironement.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
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

using AgentLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using MusaConfiguration;
using System.Reflection;
using MusaCommon;
using System.Linq;
using System.IO;

namespace AgentLibrary
{
    public sealed class AgentEnvironement
    {
        #region Fields

        /// <summary>
        /// The statements for this environment
        /// </summary>
        private ObservableCollection<IFormula> Statements
        {
            get;
            set;
        }

        /// <summary>
        /// The agents registered to this environment
        /// </summary>
        public ObservableCollection<Agent> RegisteredAgents
        {
            get;
            private set;
        }

        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        #endregion Fields

        #region Properties

        /// <summary>
        /// The moment in which this environment has been created.
        /// </summary>
        public DateTime CreationDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Count how many much time elapsed between the moment in which this environment 
        /// has been created and now.
        /// </summary>
        public TimeSpan UpTime
        {
            get { return DateTime.Now.Subtract(CreationDate); }
        }

        /// <summary>
        /// Gets a value indicating whether networking is enabled for this 
        /// environment.
        /// </summary>
        /// <value><c>true</c> if networking enabled; otherwise, <c>false
        /// </c>.</value>
        public bool NetworkingEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Return the IP address of the machine in which this environment 
        /// is located
        /// </summary>
        public string IPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get;
            private set;
        }

        #endregion Properties

        private static AgentEnvironement instance;

        /// <summary>
        /// The logger this environment uses.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The environment server used for communications
        /// </summary>
        internal EnvironmentServer EnvironmentServer
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the unique agent environement for this MUSA.NET process instance.
        /// </summary>
        public static AgentEnvironement GetInstance()
        {
            if (instance != null)
                return instance;

            instance = new AgentEnvironement();
            instance.NetworkingEnabled = MusaConfig.GetConfig().NetworkingEnabled;

            if (instance.NetworkingEnabled)
            {
                instance.Port = MusaConfig.GetConfig().MusaAddressPort;
                instance.IPAddress = MusaConfig.GetConfig().MusaAddress;
                instance.EnvironmentServer = new EnvironmentServer(instance);
                instance.EnvironmentServer.StartNetworking(instance.Port.ToString(), instance.IPAddress);
            }

            //Inject the logger
            instance.logger = ModuleProvider.Get().Resolve<ILogger>();

            return instance;
        }

        #region Constructors

        /// <summary>
        /// Create a new environment
        /// </summary>
        private AgentEnvironement()
        {

            Statements = new ObservableCollection<IFormula>();
            RegisteredAgents = new ObservableCollection<Agent>();
            RegisteredAgents.CollectionChanged += RegisteredAgents_CollectionChanged;
            Statements.CollectionChanged += Statements_CollectionChanged;
            CreationDate = DateTime.Now;
        }

        private void RegisteredAgents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (RegisteredAgents.Count > 0)
                mre.Reset();
            else
                mre.Set();

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                        logger.Log(LogLevel.Debug, "Added agent: " + (newItem as Agent).Name);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var newItem in e.NewItems)
                        logger.Log(LogLevel.Debug, "Removed agent: " + newItem);
                    break;
            }
        }

        #endregion

        #region Methods

        #region Collection delegates

        /// <summary>
        /// Method invoked when a changes that involves the attributes occurs into the environment's statement 
        /// list. It is responsible for communicating the changes to the registered agents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Agent a in RegisteredAgents)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    a.notifyEnvironementChanges(AgentPerception.SetBeliefValue, e.NewItems);
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    a.notifyEnvironementChanges(AgentPerception.UnSetBeliefValue, e.OldItems);
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                    a.notifyEnvironementChanges(AgentPerception.UpdateBeliefValue, e.OldItems);
            }
        }

        /// <summary>
        /// Method invoked when a changes that involves the statements occurs into the environment's statement 
        /// list. It is responsible for communicating the changes to the registered agents.
        /// </summary>
        private void Statements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Agent a in RegisteredAgents)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    a.notifyEnvironementChanges(AgentPerception.AddBelief, e.NewItems);
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    a.notifyEnvironementChanges(AgentPerception.RemoveBelief, e.OldItems);
            }
        }

        #endregion

        /// <summary>
        /// Register an agent to this environment
        /// </summary>
        public void RegisterAgent(Agent a)
        {
			if (!RegisteredAgents.Contains (a)) 
				RegisteredAgents.Add(a);
        }

        public Agent GetAgent(string name)
        {
            return RegisteredAgents.FirstOrDefault(x => x.Name.Equals(name));
        }

        /// <summary>
        /// Clears the current environment (removes all agents and all the 
        /// statements) and register agent from configuration.
        /// </summary>
        public void ClearAndRegisterAgentFromConfiguration()
        {
            //TODO Implementami
        }

        /// <summary>
        /// Clears the current environment (removes all agents and all the 
        /// statements) and register agent from configuration.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public void ClearAndRegisterAgentFromConfiguration(MusaConfig a)
        {
            //TODO Implementami
        }

        /// <summary>
        /// Registers the agents found within the MUSA configuration file.
        /// </summary>
        public void RegisterAgentFromConfiguration()
        {
            //Inject the logger
            var FormulaUtils = ModuleProvider.Get().Resolve<IFormulaUtils>();

            //Get the agents configuration
            var ae = MusaConfig.GetConfig().Agents;

            //Load all the plans from the specified plan libraries dll in the config xml file
            List<Type> external_plans = GetPlansFromExternalLibraries();

            //Get all the plans in the calling assembly
            IEnumerable<Type> calling_assembly_plans = Assembly.GetCallingAssembly().GetTypes().Where(x => typeof(IPlanModel).IsAssignableFrom(x));

            foreach (AgentEntry ag in ae)
            {
                Agent new_agent = new Agent(ag.Name);

                foreach (BeliefEntry belief in ag.BeliefBase)
                {
                    var formula = FormulaUtils.Parse(belief.Value);
                    var unrolled_formula = FormulaUtils.UnrollFormula(formula).ToArray();
                    new_agent.AddBelief(unrolled_formula);
                }

                foreach (string plan in ag.Plans)
                {
                    try
                    {
                        var the_plan = external_plans.Find(x => x.Name.Equals(plan));
                        if (the_plan == null)
                        {
                            var other_plan = calling_assembly_plans.FirstOrDefault(x => x.Name.Equals(plan));
                            if (other_plan == null)
                                throw new Exception("Plan '" + plan + "' not found in neither external libs or current project.");
                            new_agent.AddPlan(other_plan);
                        }
                        else
                            new_agent.AddPlan(the_plan);
                    }
                    catch
                    {
                        logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Red);
                        logger.Log(LogLevel.Error, "Failed to load plan '" + plan + "' for agent '" + ag.Name + "'. Make sure you have specified the correct path to the plan library dll (in the config xml) where to find this plan.\n");
                    }
                }

                foreach (AgentEvent agent_event in parseEventFromConfiguration(ag, new_agent.Plans))
                    new_agent.AddEvent(agent_event);

                //Register the agent to this environment
                RegisterAgent(new_agent);
            }
        }

        /// <summary>
        /// Gets the plans types from external libraries specified within the musa configuration file.
        /// </summary>
        /// <returns>The plans from external libraries.</returns>
        private List<Type> GetPlansFromExternalLibraries()
        {
            var plan_types = new List<Type>();

            //For each plan library
            foreach (string assembly_path in MusaConfig.GetConfig().PlanLibrariesPath)
            {
                Assembly assembly = null;
                try
                {
                    //Load the assembly
                    assembly = Assembly.LoadFrom(assembly_path);
                }
                catch (FileNotFoundException)
                {
                    logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Red);
                    logger.Log(LogLevel.Error, "Failed to load assembly '" + assembly_path + "'. Make sure the path is right.");
                    continue;
                }

                //Get all types, from the loaded assembly, which implement IPlanModel interface
                var this_assembly_plans = assembly.GetTypes().Where(x => typeof(IPlanModel).IsAssignableFrom(x));

                //Add to the output types list
                plan_types.AddRange(this_assembly_plans);
            }
            return plan_types;
        }

        #region Serialization

        public MusaConfig Serialize()
        {
            MusaConfig conf = MusaConfig.GetConfig(false);
            conf.NetworkingEnabled = NetworkingEnabled;
            conf.AddLoggerFragment(logger.GetFragments());
            conf.MusaAddress = IPAddress;
            conf.MusaAddressPort = Port;

            conf.Agents.AddRange(serializeAgentSet());

            return conf;
        }

        private List<AgentEntry> serializeAgentSet()
        {
            List<AgentEntry> agents = new List<AgentEntry>();

            foreach (Agent a in RegisteredAgents)
            {
                var ae = new AgentEntry();
                ae.Name = a.Name;

                //Parse each event
                foreach (KeyValuePair<AgentEventKey, Type> kvp in a.Events)
                {
                    var ek = new EventEntry();
                    ek.formula = kvp.Key.Formula;
                    ek.perception = kvp.Key.Perception.ToString();
                    ek.plan = kvp.Value.Name;

                    PlanArgs eventArgs = null;
                    a.EventsArgs.TryGetValue(kvp.Key, out eventArgs);

                    if (eventArgs == null)
                    {
                        ae.Events.Add(ek);
                        continue;
                    }

                    foreach (KeyValuePair<string, object> arg in eventArgs)
                    {
                        var the_arg = new EventArgEntry();
                        the_arg.Name = arg.Key;
                        the_arg.Value = arg.Value as string;

                        ek.EventArgs.Add(the_arg);
                    }

                    ae.Events.Add(ek);
                }

                foreach (Type plan in a.Plans)
                    ae.Plans.Add(plan.Name);

                foreach (IFormula ff in a.Beliefs)
                {
                    var the_belief = new BeliefEntry();
                    the_belief.Value = ff.ToString();
                    ae.BeliefBase.Add(the_belief);
                }

                //agent's assignment?

                agents.Add(ae);
            }

            return agents;
        }

        #endregion

        /// <summary>
        /// Parses the events for a specific agent from the MUSA xml configuration file.
        /// </summary>
        /// <returns>The events parsed from configuration for the agent [ag].</returns>
        /// <param name="ag">The agent configuration.</param>
        /// <param name="ag_plans">The plans type that [ag] supports.</param>
        private List<AgentEvent> parseEventFromConfiguration(AgentEntry ag, List<Type> ag_plans)
        {
            List<AgentEvent> events = new List<AgentEvent>();

            foreach (EventEntry ev in ag.Events)
            {
                //Parse event args
                PlanArgs event_args = null;
                if (ev.EventArgs.Count > 0)
                {
                    event_args = new PlanArgs();
                    foreach (EventArgEntry arg_entry in ev.EventArgs)
                    {
                        object the_value = Convert.ChangeType(arg_entry.Value, arg_entry.ValueType);
                        event_args.Add(arg_entry.Name, the_value);
                    }
                }

                try
                {
                    //Parse the perception this event reacts to
                    var perception = (AgentPerception)Enum.Parse(typeof(AgentPerception), ev.perception);

                    //Parse the plan that must be invoked when this event is triggered
                    var the_plan = ag_plans.Find(x => x.Name.Equals(ev.plan));

                    if (the_plan == null)
                        throw new Exception();
                    events.Add(new AgentEvent(ev.formula, perception, the_plan, event_args));
                }
                catch
                {
                    logger.SetColorForNextConsoleLog(ConsoleColor.Black, ConsoleColor.Red);
                    logger.Log(LogLevel.Error, "An error occurred while parsing event '" + ev + "' for agent '" + ag.Name + "'.\n Error: plan '" + ev.plan + "' not found.\n");
                }
            }
            return events;
        }


        /// <summary>
        /// Add a statement (as atomic formula) into this environment. The addition of the formula(s) is notified to 
        /// every registered agent (to this environment).
        /// </summary>
        public void RegisterStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                if (ff.IsAtomic())
                    //Add the formula to this environment
                    Statements.Add(ff);
                else
                {
                    var unrolled = ModuleProvider.Get().Resolve<IFormulaUtils>().UnrollFormula(ff);
                    foreach (IFormula uf in unrolled)
                        Statements.Add(uf);
                }

            }
        }

        /// <summary>
        /// Given a set of atomic formulas, this method removes the exact matching formulas from this environment.
        /// </summary>
        public void DeleteStatement(params IFormula[] f)
        {
            foreach (IFormula ff in f)
            {
                //Remove the formula from this environment
                Statements.Remove(ff);
            }
        }

        /// <summary>
        /// Check if an agent is registered to this environment
        /// </summary>
        public bool IsAgentRegistered(Agent a)
        {
            return RegisteredAgents.Contains(a);
        }

        public void SetAccessPolicy()
        {
            /*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
            throw new NotImplementedException();
        }

        public void SetExternalCommunicationPolicy()
        {
            /*
                pubblico
                privato
                solo manager di progetto
                solo se disposti di una chiave
            */
            throw new NotImplementedException();
        }

        /// <summary>
        /// Block the execution of the main process until at least one agent is registered within this environment.
        /// </summary>
        public void WaitForAgents()
        {
            mre.WaitOne();
        }


        #endregion

    }
}
