using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using NLog;

namespace MusaConfig
{
	[ServiceContract]
	public interface IMusaWCFLogger
	{
		[WebInvoke (Method = "POST",
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json)]
		[OperationContract]
		void Log (string level, string message);
	}
}

