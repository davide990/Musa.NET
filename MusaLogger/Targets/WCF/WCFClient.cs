using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace MusaConfig
{
	public class WCFClient : ClientBase<IMusaWCFLogger>, IMusaWCFLogger
	{
		public WCFClient (Binding binding, EndpointAddress address)
			: base (binding, address)
		{}

		public void Log (string level, string message)
		{
			Channel.Log (level, message);
		}
	}
}

