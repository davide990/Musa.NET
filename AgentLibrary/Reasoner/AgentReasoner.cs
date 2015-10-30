/**
         __  __                                     _   
        |  \/  |                                   | |  
        | \  / | _   _  ___   __ _     _ __    ___ | |_ 
        | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
        | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
        |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
using System.Collections.Generic;
using Quartz;
using System.Collections;
using System.Threading;
using System;
using AgentLibrary.Networking;
using FormulaLibrary.ANTLR;
using FormulaLibrary;

namespace AgentLibrary
{
    /// <summary>
    /// A convinient enum to mark the changes that can occur within the environment
    /// </summary>
    public enum PerceptionType
    {
        Null                = 0x0,              //No changes occurred
        AddBelief           = 0x1,              //A belief has been added
        RemoveBelief        = 0x2,              //A belief has been removed
        UpdateBeliefValue   = 0x3,              //A belief's value must be updated
        SetBeliefValue      = 0x4,              //A belief's value must be set
        UnSetBeliefValue    = 0x5               //A belief's value must be unset
    }

    public sealed class AgentReasoner
    {
        private int currentReasoningCycle = 0;
		private readonly int ReasoningUpdateTime = 5000;

        /// <summary>
        /// The agent this reasoner belongs to
        /// </summary>
        private readonly Agent parentAgent;

        /// <summary>
        /// Gets or sets the agent thread.
        /// </summary>
        private Thread AgentThread
        {
			get { return parentAgent.Thread; }
			set { parentAgent.Thread = value; }
        }

        public AgentReasoner(Agent agent)
        {
            parentAgent = agent;
            AgentThread      = new Thread(new ThreadStart(agentReasoningMain));
            AgentThread.Name = parentAgent.Name;
        }

        internal void startReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui inizia il reasoning
            Console.WriteLine("Starting agent [" + parentAgent.Name + "] reasoning...");
            AgentThread.Start();
        }

        public void stopReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui ferma il reasoning

        }

        public void pauseReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui pausa il reasoning
            
        }

        public void resumeReasoning()
        {
			//TODO log this
            //TODO segna un timestamp in cui ripristina il reasoning
            
        }

		/// <summary>
		/// This is the main agent's reasoning method. It is executed on an agent-related unique thread.
		/// </summary>
        private void agentReasoningMain()
        {
            while(true)
            {
                Console.WriteLine("reasoning cycle #"+ currentReasoningCycle++);
                Console.WriteLine("Checking mail box...");
                lock (parentAgent.lock_mailBox)
                {
                    //Check the mail box
                    checkMailBox();

					//check events

                    //Clear it
                    parentAgent.mailBox.Clear();


					//check intentions (plans)



                    Thread.Sleep(1000);
                }

                Console.WriteLine("Checking environment changes...");
                lock (parentAgent.lock_perceivedEnvironmentChanges)
                {
                    //Update the agent workbench
                    perceptEnvironement();

                    //Trigger possible events caused by the update of agent's workbench
                    triggerEvents();

                    //Clear the agent's queue of perceived environment changes
                    parentAgent.perceivedEnvironementChanges.Clear();
                }

                Console.WriteLine("######### done reasoning...");
                Thread.Sleep(ReasoningUpdateTime);
                //#Environment percept
                //-> percept belief changes in environment
                //-> percept/trigger events
                //#Jobs scheduling
                //-> dequeue and execute
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void checkMailBox()
        {
            foreach (KeyValuePair<AgentPassport, AgentMessage> p in parentAgent.mailBox)
            {
                switch (p.Value.InfoType)
                {
                    case InformationType.Tell:
                        Console.WriteLine("[" + parentAgent.Name + "] perceiving TELL: " + p.Value.ToString());
                        parentAgent.Workbench.AddStatement(FormulaParser.Parse(p.Value.Message as string));
                        break;

                    case InformationType.Untell:
                        Console.WriteLine("[" + parentAgent.Name + "] perceiving UNTELL: " + p.Value.ToString());
                        parentAgent.Workbench.RemoveStatement(FormulaParser.Parse(p.Value.Message as string));
                        break;

                    case InformationType.Achieve:

                        //!!!

                        break;
                }
            }
        }

        /// <summary>
        /// Percept the environment changes and updates the workbench of the parent agent.
        /// </summary>
        private void perceptEnvironement()
        {
            if (parentAgent.perceivedEnvironementChanges.Count <= 0)
                return;

            foreach(KeyValuePair<IList, PerceptionType> p in parentAgent.perceivedEnvironementChanges)
            {
                switch (p.Value)
                {
                    case PerceptionType.AddBelief:
                            Console.WriteLine("[" + parentAgent.Name + "] perceiving ADD: " + p.Value.ToString());
                            parentAgent.Workbench.AddStatement(p.Key);
                        break;

                    case PerceptionType.RemoveBelief:
                            parentAgent.Workbench.AddStatement(p.Key);
                        break;

                    case PerceptionType.SetBeliefValue:
                        break;

                    case PerceptionType.UnSetBeliefValue:
                        break;

                    case PerceptionType.UpdateBeliefValue:
                        break;

                    case PerceptionType.Null:
                        break;
                }
            }
        }

        /// <summary>
        /// Trigger events. Events triggers may be caused by the agent's perception of changes within the environment.
        /// </summary>
        private void triggerEvents()
        {
            if (parentAgent.perceivedEnvironementChanges.Count <= 0)
                return;

            foreach (KeyValuePair<IList, PerceptionType> p in parentAgent.perceivedEnvironementChanges)
            {
                switch (p.Value)
                {
                    case PerceptionType.AddBelief:
                        //TRIGGER
                        break;

                    case PerceptionType.RemoveBelief:
                        //TRIGGER
                        break;

                    case PerceptionType.SetBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.UnSetBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.UpdateBeliefValue:
                        //TRIGGER
                        break;

                    case PerceptionType.Null:
                        break;
                }
            }
        }
    }
}