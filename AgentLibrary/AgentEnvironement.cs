using AgentLibrary;
using FormulaLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using MusaConfiguration;
using System.Reflection;
using MusaCommon;

namespace AgentLibrary
{
    public sealed class AgentEnvironement
    {
        #region Fields

        /// <summary>
        /// The statements for this environment
        /// </summary>
        private ObservableCollection<AtomicFormula> statements;

        /// <summary>
        /// The set of attributes for the statement contained in this environment
        /// </summary>
        private ObservableCollection<AssignmentType> attributes;

        /// <summary>
        /// The agents registered to this environment
        /// </summary>
        private ObservableCollection<Agent> registeredAgents;

        public ObservableCollection<Agent> RegisteredAgents
        {
            get { return registeredAgents; }
            set
            {
                if (value != null)
                    registeredAgents = value;
            }
        }

        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        #endregion Fields

        #region Properties

        /// <summary>
        /// The moment in which this environment has been created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return creationTime; }
            private set { creationTime = value; }
        }

        private DateTime creationTime;

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
        /// Get the unique agent environement for this MUSA.NET process instance.
        /// </summary>
        public static AgentEnvironement GetInstance()
        {
            if (instance == null)
            {
                instance = new AgentEnvironement();

                instance.NetworkingEnabled = MusaConfig.GetConfig().NetworkingEnabled;

                if (instance.NetworkingEnabled)
                {
                    instance.Port = MusaConfig.GetConfig().MusaAddressPort;
                    instance.IPAddress = MusaConfig.GetConfig().MusaAddress;

                    EnvironmentServer srv = new EnvironmentServer(instance);
                    srv.StartNetworking(instance.Port.ToString(), instance.IPAddress);
                }

                //Inject the logger
                instance.logger = ModuleProvider.Get().Resolve<ILogger>();
            }

            return instance;
        }

        #region Constructors

        /// <summary>
        /// Create a new environment
        /// </summary>
        private AgentEnvironement()
        {
			
            statements = new ObservableCollection<AtomicFormula>();
            attributes = new ObservableCollection<AssignmentType>();
            registeredAgents = new ObservableCollection<Agent>();

            CreationDate = DateTime.Now;

            registeredAgents.CollectionChanged += RegisteredAgents_CollectionChanged;
            
            statements.CollectionChanged += Statements_CollectionChanged;
            attributes.CollectionChanged += Attributes_CollectionChanged;
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
                        logger.Log(LogLevel.Debug, "Added agent: " + newItem);    
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
            foreach (Agent a in registeredAgents)
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
            foreach (Agent a in registeredAgents)
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
            if (!registeredAgents.Contains(a))
                registeredAgents.Add(a);
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
            LoadExternalPlanLibraries();

            var ae = MusaConfig.GetConfig().Agents;

            foreach (AgentEntry ag in ae)
            {
                Agent new_agent = new Agent(ag.Name);

                foreach (string belief in ag.BeliefBase)
                {
                    var formula = FormulaParser.Parse(belief);
                    var unrolled_formula = FormulaUtils.UnrollFormula(formula).ToArray();
                    new_agent.AddBelief(unrolled_formula);
                }

                foreach (string plan in ag.Plans)
                {
                    try
                    {
                        var the_plan = Type.GetType(plan);
                        new_agent.AddPlan(the_plan);
                    }
                    catch (Exception e)
                    {
                        logger.Log(LogLevel.Error, "Failed to load plan '" + plan + "' for agent '" + ag.Name + "'.\n Error: " + e.Message + "\nStackTrace: " + e.StackTrace);
                    }
                }

                foreach (AgentEvent agent_event in parseEventFromConfiguration(ag))
                    new_agent.AddEvent(agent_event);

                //Register the agent to this environment
                RegisterAgent(new_agent);
            }
        }

        private void LoadExternalPlanLibraries()
        {
            //List<Assembly> assemblies = new List<Assembly>();
            foreach (string assembly_path in MusaConfig.GetConfig().PlanLibrariesPath)
                AppDomain.CurrentDomain.Load(Assembly.LoadFile(assembly_path).GetName());
            
            //assemblies.Add(Assembly.LoadFile(assembly_path));
        }

        public MusaConfig Serialize()
        {
            MusaConfig conf = new MusaConfig();
            conf.NetworkingEnabled = NetworkingEnabled;

            //conf.LoggerFragments = logger.Fragments;

            //conf.LoggerFragments.AddRange(logger.GetFragments());
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

                    AgentEventArgs eventArgs = null;
                    a.EventsArgs.TryGetValue(kvp.Key, out eventArgs);

                    if (eventArgs == null)
                    {
                        ae.Events.Add(ek);
                        continue;
                    }

                    foreach (KeyValuePair<string, string> arg in eventArgs)
                    {
                        var the_arg = new EventArgEntry();
                        the_arg.Name = arg.Key;
                        the_arg.Value = arg.Value;

                        ek.EventArgs.Add(the_arg);
                    }

                    ae.Events.Add(ek);
                }

                foreach (Type plan in a.Plans)
                    ae.Plans.Add(plan.Name);

                foreach (AtomicFormula ff in a.Beliefs)
                {
                    ae.BeliefBase.Add(ff.ToString());
                }

                //agent's assignment?

                agents.Add(ae);
            }

            return agents;
        }

        /// <summary>
        /// Parses the events for a specific agent from the MUSA XML 
        /// configuration file.
        /// </summary>
        private List<AgentEvent> parseEventFromConfiguration(AgentEntry ag)
        {
            List<AgentEvent> events = new List<AgentEvent>();
            /*
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string assembly_path in MusaConfig.GetConfig().PlanLibraries)
                assemblies.Add(Assembly.LoadFile(assembly_path));
            */
            //var cd = AppDomain.CurrentDomain.GetAssemblies();

            //TODO da finire
            foreach (EventEntry ev in ag.Events)
            {
                //Parse event args
                AgentEventArgs event_args = null;
                if (ev.EventArgs.Count > 0)
                {
                    event_args = new AgentEventArgs();
                    foreach (EventArgEntry arg_entry in ev.EventArgs)
                        event_args.Add(arg_entry.Name, arg_entry.Value);
                }

                try
                {
                    //Parse the perception this event reacts to
                    var perception = (AgentPerception)Enum.Parse(typeof(AgentPerception), ev.perception);

                    //Parse the plan that must be invoked when this event is
                    //triggered
                    //var all_assemblies = Assembly.GetExecutingAssembly();//CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in MusaConfig.GetConfig().PlanLibraries)
                    {
                        var all_types = assembly.GetTypes();
                        
                        foreach (var type in all_types)
                        {
                            if (type.Name.Equals(ev.plan))
                            {
                                events.Add(new AgentEvent(ev.formula, perception, type, event_args));
                            }
                        }
                    }
                    /*Type type = assembly.GetType() //GetType(ev.plan);
                        if (type != null)
                            events.Add(new AgentEvent(ev.formula, perception, type, event_args));*/
                    //}

                    //var plan = Type.GetType(ev.plan);
                    //events.Add(new AgentEvent(ev.formula, perception, plan, event_args));
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, "An error occurred while parsing event '" + ev + "' for agent '" + ag.Name + "'.\n Error: " + e.Message + "\nStackTrace: " + e.StackTrace);
                }
            }
            return events;
        }


        /// <summary>
        /// Add a statement (as atomic formula) into this environment. The addition of the formula(s) is notified to 
        /// every registered agent (to this environment).
        /// </summary>
        public void RegisterStatement(params AtomicFormula[] f)
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

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    attributes.Add(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Add the formula to this environment
                statements.Add(ff);
            }
        }

        /// <summary>
        /// Given a set of atomic formulas, this method removes the matching formulas from this environment and also its
        /// corresponding assignments.
        /// </summary>
        public void DeleteStatementAndAssignment(params AtomicFormula[] f)
        {
            List<object> variableTerms = new List<object>();
            foreach (AtomicFormula ff in f)
            {
                variableTerms = ff.ConvertToSimpleFormula();

                if (!statements.Contains(ff))
                    continue;

                foreach (object varTerm in variableTerms)
                {
                    //get the type info for the current term
                    Type variableTermType = typeof(VariableTerm<>).MakeGenericType(varTerm.GetType().GetGenericArguments()[0]);

                    //get the value of the current term
                    object name = Convert.ChangeType(variableTermType.GetProperty("Name").GetValue(varTerm), typeof(string));
                    object value = variableTermType.GetProperty("Value").GetValue(varTerm);
                    value = Convert.ChangeType(value, varTerm.GetType().GetGenericArguments()[0]);

                    attributes.Remove(AssignmentType.CreateAssignmentForTerm((string)name, value, varTerm.GetType().GetGenericArguments()[0]));
                }

                //Remove the formula from this environment
                statements.Remove(ff);
            }
        }

        /// <summary>
        /// Given a set of atomic formulas, this method removes the matching formulas from this environment.
        /// </summary>
        public void DeleteStatement(params AtomicFormula[] f)
        {
            foreach (AtomicFormula ff in f)
            {
                //Convert the formula to a simple formula
                ff.ConvertToSimpleFormula();

                if (!statements.Contains(ff))
                    continue;

                //Remove the formula from this environment
                statements.Remove(ff);
            }
        }

        /// <summary>
        /// Check if an agent is registered to this environment
        /// </summary>
        public bool IsAgentRegistered(Agent a)
        {
            return registeredAgents.Contains(a);
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
