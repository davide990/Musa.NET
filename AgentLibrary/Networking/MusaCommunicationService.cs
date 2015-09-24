using System;

namespace AgentLibrary.Networking
{
    public class MusaCommunicationService : IMusaCommunicationService
    {
        public string GetData(int value)
        {
            return "hi " + value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="senderData">Sender data.</param>
        /// <param name="receiverData">Receiver data.</param>
        /// <param name="message">Message.</param>
        public bool sendAgentMessage(AgentPassport senderData, AgentPassport receiverData, AgentMessage message)
        {
            Console.WriteLine("### AGENT-AGENT MESSAGE ###");
            Console.WriteLine(senderData.ToString());
            Console.WriteLine(receiverData.ToString());
            Console.WriteLine(message.ToString());
            return true;
        }

        public bool sendBroadcastMessage(AgentPassport senderData, EnvironementData receiverData, AgentMessage message)
        {
            Console.WriteLine("### BROADCAST MESSAGE ###");
            Console.WriteLine(senderData.ToString());
            Console.WriteLine(receiverData.ToString());
            Console.WriteLine(message.ToString());
            return true;
        }

        public bool AgentIsActive(AgentPassport senderData, EnvironementData receiverData)
        {
            Console.WriteLine("### AGENT PING TEST ###");
            Console.WriteLine(senderData.ToString());
            Console.WriteLine(receiverData.ToString());
            return true;
        }

        public bool RequestAuthorizationKey(EnvironementData env)
        {
            Console.WriteLine("### AUTH KEY REQUEST ###");
            Console.WriteLine(env.ToString());
            return true;
        }
    }
}
