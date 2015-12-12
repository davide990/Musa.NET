using System.ServiceModel;
using System.ServiceModel.Web;

namespace MusaLogger
{
	/// <summary>
	/// The common interface that defines how logs are sent remotely using WCF service.
	/// </summary>
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

