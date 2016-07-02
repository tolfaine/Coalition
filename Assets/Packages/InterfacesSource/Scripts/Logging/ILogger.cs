namespace interfaces
{
	public interface ILogger
	{
		void Log(object toLog);

		void LogFormat(string format, params object[] parameters);

		void LogWarning(object toLog);

		void LogWarningFormat(string format, params object[] parameters);

		void LogError(object toLog);

		void LogErrorFormat(string format, params object[] parameters);
	}
}