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

namespace AgentLibrary
{
    /// <summary>
    /// A convinient enum to mark the changes that can occur within the environement
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
        /// <summary>
        /// The quartz.net scheduler class
        /// </summary>
        private IScheduler job_scheduler;
        
        /// <summary>
        /// The job schedules
        /// </summary>
        private List<Schedule> schedules;

        /// <summary>
        /// The agent this reasoner belongs to
        /// </summary>
        private readonly Agent parentAgent;

        public AgentReasoner(Agent agent)
        {
            parentAgent = agent;
        }

        public void startReasoning()
        {
            //#Environement percept
            //-> percept belief changes in environement
            //-> percept events
            //#Jobs scheduling
            //-> dequeue and execute
        }

        public void stopReasoning()
        {

        }

        public void pauseReasoning()
        {

        }

        public void resumeReasoning()
        {

        }



        private void perceptEnvironement()
        {

        }

        private void perceptEvents()
        {

        }
    }
}