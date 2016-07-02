using strange.extensions.signal.impl;
using System.Collections;
using System.Collections.Generic;

namespace interfaces
{
	public enum ReturnStatus
	{
		NOT_FINISHED,
		SUCCESS,
		TRANSPORT_ERROR,
		APPLICATION_ERROR,
		INTERNAL_ERROR,
	}

	//NOTE: does not check to see if you did something silly like sent post params in a GET request, or Binary post params in a GET request
	public class WWWFormParams
	{
		public enum RequestType
		{
			GET,
			POST,
			BINARY_POST,
		}

		public RequestType TypeOfRequest;

		public Dictionary<string, string> GetParams;
		public Dictionary<string, string> PostParams;
		public Dictionary<string, byte[]> BinaryPostParams;

		public Dictionary<string, string> headers;
	}

	public class WebServiceStatus
	{
		public ReturnStatus Status { get; set; }

		public string Text { get; set; }

		public string ErrorMessage { get; set; }
	}

	public interface IWebService
	{
		ReturnStatus Status { get; }

		IEnumerator SendRequest(string url, WWWFormParams toAddToUrl, WebServiceStatus returnStatus);

		void SendRequest(string url, WWWFormParams toAddToUrl = null);

		Signal<WebServiceStatus> OnWebserviceFinishedSignal { get; }  //return self so status can be checked up front
	}
}