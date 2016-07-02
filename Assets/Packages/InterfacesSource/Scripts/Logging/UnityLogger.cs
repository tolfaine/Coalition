using UnityEngine;

namespace interfaces
{
	public class UnityLogger : ILogger
	{
		public void Log(object toLog)
		{
			Debug.Log(toLog);
		}

		public void LogFormat(string format, params object[] parameters)
		{
			Debug.Log(string.Format(format, parameters));
		}

		public void LogWarning(object toLog)
		{
			Debug.LogWarning(toLog);
		}

		public void LogWarningFormat(string format, params object[] parameters)
		{
			Debug.LogWarning(string.Format(format, parameters));
		}

		public void LogError(object toLog)
		{
			Debug.LogError(toLog);
		}

		public void LogErrorFormat(string format, params object[] parameters)
		{
			Debug.LogError(string.Format(format, parameters));
		}
	}
}