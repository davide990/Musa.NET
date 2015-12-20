using System.ServiceModel.Channels;
using System.ServiceModel;
using System;
using MusaWCFLogger;
using System.Threading.Tasks;

namespace MusaLogger
{
	/// <summary>
	/// The base WCF client used by WCFLogger.
	/// </summary>
	internal sealed class WCFClient : ClientBase<IMusaWCFLogger>, IMusaWCFLogger
	{
		public WCFClient (Binding binding, EndpointAddress address)
			: base (binding, address)
		{}

		public void Log (string level, string message)
		{
			Channel.Log (level, message);
		}

		public async void GetStringDataAsync(string level, string message)
		{
			await Task.Factory.StartNew(() => Channel.Log(level,message));
		}
	}
}

