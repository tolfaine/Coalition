using strange.extensions.signal.impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Log = UnityEngine.Debug;

namespace interfaces
{
	//Like a facade around unity code in order to help unit tests to be able to use some other interface
	//[Implements(IWebService), InjectionBindingScope.CROSS_CONTEXT)]
	[Implements(typeof(IWebService))]
	public class UnityWWWWebSerivce : IWebService
	{
		public WebServiceStatus status = new WebServiceStatus();
		

		[Inject]
		public IRoutineRunner CoroutineRunner { get; set; }

		//private readonly ILogger Log = LogManager.GetLogger<UnityWWWWebSerivce>();

		public ReturnStatus Status { get; private set; }

		public Signal<WebServiceStatus> OnWebserviceFinishedSignal { get; private set; }

		public IEnumerator SendRequest(string url, WWWFormParams toAddToUrl, WebServiceStatus returnStatus)
		{
			var toWaitOn = InternalSendRequest(url, toAddToUrl, returnStatus);
			yield return CoroutineRunner.StartCoroutine(toWaitOn);
			//toWaitOn.MoveNext();
			//while(toWaitOn.Current != null && toWaitOn.MoveNext())
			//{
			//	yield return toWaitOn.Current;
			//	yield return null;
			//}

			returnStatus.ErrorMessage = status.ErrorMessage;
			returnStatus.Text = status.Text;
			returnStatus.Status = status.Status;
		}

		public void SendRequest(string url, WWWFormParams toAddToUrl = null)
		{
			var routine = InternalSendRequest(url, toAddToUrl, status);
			CoroutineRunner.StartCoroutine(routine);
			Status = status.Status;
		}

		[PostConstruct]
		public void PostConstruct()
		{
			OnWebserviceFinishedSignal = new Signal<WebServiceStatus>();
		}

		private IEnumerator InternalSendRequest(string url, WWWFormParams toAddToUrl, WebServiceStatus returnStatus)
		{
			WWW www;
			try
			{
				WWWForm form = null;
				if(toAddToUrl != null && toAddToUrl.TypeOfRequest != WWWFormParams.RequestType.GET)
				{
					form = new WWWForm();
					if(toAddToUrl.PostParams != null)
					{
						foreach(var kvp in toAddToUrl.PostParams)
						{
							form.AddField(kvp.Key, kvp.Value);
						}
					}

					if(toAddToUrl.BinaryPostParams != null)
					{
						foreach(var kvp in toAddToUrl.BinaryPostParams)
						{
							form.AddBinaryData(kvp.Key, kvp.Value);
						}
					}
				}
				if(toAddToUrl != null && toAddToUrl.headers != null)
				{
					if(form == null)
						form = new WWWForm();
					foreach(var header in toAddToUrl.headers)
					{
						if(toAddToUrl.headers.ContainsKey(header.Key))
							form.headers[header.Key] = header.Value;
						else
							form.headers.Add(header.Key, header.Value);
					}
				}
				string fullUrlWithGetParams;
				if(toAddToUrl != null)
					fullUrlWithGetParams = GetFullUrl(url, toAddToUrl.GetParams);
				else
					fullUrlWithGetParams = GetFullUrl(url);
				if(form == null)
					www = new WWW(fullUrlWithGetParams);
				else
					www = new WWW(fullUrlWithGetParams, form);
			}
			catch(Exception e)
			{
				Debug.LogError("Error in setting up with www call" + e);
				Log.LogError("Error in setting up with www call" + e);
				returnStatus.ErrorMessage = "Error setting up www call" + e.Message;
				returnStatus.Status = ReturnStatus.INTERNAL_ERROR;
				OnWebserviceFinishedSignal.Dispatch(status);
				yield break;
			}

			yield return www;
			while(!www.isDone)
			{
				yield return null;
			}
			yield return null;

			if(www.error != null)
			{
				//TODO: find the appropriate codes/messages for transport error
				returnStatus.Status = ReturnStatus.TRANSPORT_ERROR;
				returnStatus.Status = ReturnStatus.INTERNAL_ERROR;
				returnStatus.ErrorMessage = www.error;
				www.Dispose();
				OnWebserviceFinishedSignal.Dispatch(status);
				yield break;
			}

			try
			{
				status.Text = www.text;
			}
			catch(Exception e)
			{
				Log.LogError("Error in reading values from www object after yielding on it" + e);
				status.ErrorMessage = "Error in reading values from www object after yielding on it" + e.Message;
				status.Status = ReturnStatus.INTERNAL_ERROR;
				Status = ReturnStatus.INTERNAL_ERROR;

				www.Dispose();
				OnWebserviceFinishedSignal.Dispatch(status);
				yield break;
			}

			www.Dispose();

			status.Status = ReturnStatus.SUCCESS;
			try
			{
				OnWebserviceFinishedSignal.Dispatch(status);
			}
			catch(Exception e)
			{
				Log.LogError("Error returning from web service" + e);
				status.ErrorMessage = "Error returning from web service" + e.Message + " stack: " + e.StackTrace;
				status.Status = ReturnStatus.INTERNAL_ERROR;
			}
		}

		private string GetFullUrl(string baseUrl, Dictionary<string, string> GetParameters = null)
		{
			if(GetParameters == null || GetParameters.Count == 0) return baseUrl;
			var finalUrlBuilder = new StringBuilder(baseUrl);
			finalUrlBuilder.Append("?");
			foreach(var keyValuePair in GetParameters)
			{
				finalUrlBuilder.AppendFormat("{0}={1}&", WWW.EscapeURL(keyValuePair.Key), WWW.EscapeURL(keyValuePair.Value));
			}
			return finalUrlBuilder.ToString();
		}
	}
}