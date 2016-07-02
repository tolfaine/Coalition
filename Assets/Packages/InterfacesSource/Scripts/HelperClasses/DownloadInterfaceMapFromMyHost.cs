using System;
using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class DownloadInterfaceMapFromMyHost
	{
		public DocumentResult output = new DocumentResult();
		public string RequestUrl = "https://techiealex.com/lifecoach/getConfig.php";

		//
		public IEnumerator getCollabEditDocument()
		{
			var build = ResourceLoader.Load<BuildConfigSO>();
			var www = new WWW(string.Format("{0}?User={1}&Platform={2}&BUILD_NUMBER={3}&BUILD_TYPE={4}",
				RequestUrl, SystemInfo.deviceUniqueIdentifier, Application.platform, build.BuildNumber, build.BuildType));
			yield return www;
			try
			{
				if(string.IsNullOrEmpty(www.error))
				{
					output.map = www.text.JsonDeserialize<InterfaceMap>();
					//output.text = www.text;
					output.success = true;
				}
				else
				{
					output.error = www.error;
					Debug.LogError("ERROR downloading mappings:" + www.error);
				}
			}
			catch(Exception e)
			{
				output.error = "Unhandled exception:" + e;
				Debug.LogError("Unhandled exception message when handling www call:" + e.Message + " stack:" + e.StackTrace);
			}
			finally
			{
				www.Dispose();
			}
		}

		public class DocumentResult
		{
			public string error;
			public bool success;

			//public string text;
			public InterfaceMap map;
		}
	}
}