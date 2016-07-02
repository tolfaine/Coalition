using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace interfaces
{
	public class RemoteLogger : ILogger
	{
		public static string RemoteFileLogSaveURL = "https://techiealex.com/lifecoach/saveLog.php";
		private FileStream _fs;
		private bool _initialized;
		private StreamWriter _writer;

		[Inject]
		public BuildConfigSO buildConfig { get; set; }

		[Inject]
		public IRoutineRunner runner { get; set; }

		[Inject]
		public IWebService webSerivice { get; set; }

		public void Log(object toLog)
		{
			Log(toLog.ToString());
		}

		public void LogFormat(string format, params object[] parameters)
		{
			Log(string.Format(format, parameters));
		}

		public void LogWarning(object toLog)
		{
			Log("WARNING:" + toLog);
		}

		public void LogWarningFormat(string format, params object[] parameters)
		{
			Log(string.Format("WARNING:" + format, parameters));
		}

		public void LogError(object toLog)
		{
			Log("ERROR:" + toLog);
		}

		public void LogErrorFormat(string format, params object[] parameters)
		{
			Log(string.Format("ERROR:" + format, parameters));
		}

		[PostConstruct]
		public void PostConstruct()
		{
#if !UNITY_5
			Application.RegisterLogCallback(ApplicationOnLogMessageReceived);
#else
			Application.logMessageReceived += ApplicationOnLogMessageReceived;
#endif
		}

		private void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			LogFormat("UNITYLOG TYPE:{0} condition:{1} stack:{2}", type, condition, stackTrace);
		}

		private void SendCachedFiles()
		{
			string[] logFiles = Directory.GetFiles(Application.temporaryCachePath, "LogFile*txt");
			runner.StartCoroutine(SendLogFiles(logFiles));
		}

		private IEnumerator SendLogFiles(string[] paths)
		{
			foreach (string path in paths)
			{
				//Debug.Log("Sending file:" + path);
				if(path.Contains(_fs.Name))
					continue;

				Log("Sending file:" + path);
				byte[] fileBytes = null;
				try
				{
					fileBytes = File.ReadAllBytes(path);
				}
				catch (Exception e)
				{
					Debug.Log("could not read(to send to server) file:" + path + " because of exception :" + e);
					continue;
				}

				var param = new WWWFormParams
				{
					BinaryPostParams = new Dictionary<string, byte[]> {{"theFile", fileBytes}},
					TypeOfRequest = WWWFormParams.RequestType.BINARY_POST,
					GetParams =
						new Dictionary<string, string>
						{
							{"User", SystemInfo.deviceUniqueIdentifier},
							{"Platform", Application.platform.ToString()},
							{"ProductName", buildConfig.ProductName},
							{"BuildNumber", buildConfig.BuildNumber.ToString()},
							{"BuildType", buildConfig.BuildType.ToString()},
						}
				};
				var results = new WebServiceStatus();
				string url = RemoteFileLogSaveURL;
				yield return runner.StartCoroutine(webSerivice.SendRequest(url, param, results));
				//Debug.Log("STATUS:" + results.JsonSerialize());
				if(results.Status == ReturnStatus.SUCCESS)
					File.Delete(path);
				yield return new WaitForSeconds(1f);
			}
		}

		private void Log(string toLog)
		{
			if(_fs == null)
			{
				string fileName = string.Format("LogFile_{0}_{1}_{2}.txt", SystemInfo.deviceUniqueIdentifier, Application.platform,
					DateTime.Now.ToString("yyyy_M_d_HH_mm_ss"));
				_fs = new FileStream(Application.temporaryCachePath + Path.DirectorySeparatorChar + fileName, FileMode.CreateNew);
			}
			if(_writer == null)
				_writer = new StreamWriter(_fs, Encoding.UTF8);
			_writer.WriteLine(toLog);
			if(!_initialized)
			{
				if(Application.isPlaying == false || Time.frameCount < 2)
					return;
				_initialized = true;
				SendCachedFiles();
			}
		}
	}
}