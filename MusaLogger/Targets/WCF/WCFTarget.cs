using NLog.Targets;
using System.ServiceModel;
using NLog;
using System;
using NLog.Config;
using MusaCommon;

namespace MusaLogger
{
	/// <summary>
	/// A custom nlog target using WCF service.
	/// </summary>
	[Target("WCFTarget")]
	internal class WCFTarget : TargetWithLayout
	{
		/// <summary>
		/// The client used for sending logs.
		/// </summary>
		WCFClient client;

		/// <summary>
		/// Gets or sets the endpoint address to which send logs.
		/// </summary>
		/// <value>The endpoint address.</value>
		[RequiredParameter]
		public string EndpointAddress { get; set; }

		/// <summary>
		/// Initializes this target.
		/// </summary>
		protected override void InitializeTarget ()
		{
			client = new WCFClient (new BasicHttpBinding (), new EndpointAddress (EndpointAddress));
		}

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="logEvent">The log to be written out.</param>
		protected override void Write (LogEventInfo logEvent)
		{
			string logMessage = Layout.Render(logEvent);
			client.Log (logEvent.Level.ToString (), logMessage);		
		}
	}
}

