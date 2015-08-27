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
    /// TODO RINOMINARE IN AgentReasoner
    /// </summary>
    public sealed class AgentScheduler
    {
        /// <summary>
        /// The quart.net scheduler class
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

        public AgentScheduler(Agent agent)
        {
            this.parentAgent = agent;
        }

        public void startReasoning()
        {
            //#Environement percept
            //-> percept beliefs
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