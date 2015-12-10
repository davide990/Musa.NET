using System.Xml.Serialization;
using System.ServiceModel;
using System;

namespace MusaConfig
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class WCFLogger : MusaLogger
	{
		[XmlAttribute("Enabled")]
		public bool Enabled { get; set; }

		[XmlAttribute ("EndpointAddress")]
		public string EndpointAddress { get; set; }

		[XmlIgnore()]
		private WCFClient client;

		[XmlIgnore ()]
		private bool configured;

		public void configure()
		{
			client = new WCFClient (new BasicHttpBinding (), new EndpointAddress (EndpointAddress));
		}

		public override void Log (NLog.LogLevel level, string message)
		{
			if (!configured) 
			{
				//Configure at the first log, since FileName and Layout are not avaible until the constructor 
				//method ends
				configured = true;
				configure ();
			}

			if (Enabled) 
			{
				try
				{
					client.Log (level.ToString (), message);
				}
				catch(EndpointNotFoundException e)
				{
					Console.WriteLine ("An error occurred while logging using WCF service.\n" + e.InnerException);
				}
			}
		}


	}
}

