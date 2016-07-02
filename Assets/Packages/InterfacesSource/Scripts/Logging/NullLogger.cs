namespace interfaces
{
	//[Implements(typeof(SubLogger),InjectionBindingScope.SINGLE_CONTEXT,LogManager.NamedLogger.PRIMARY)]
	//[Implements(typeof(SubLogger), InjectionBindingScope.SINGLE_CONTEXT, LogManager.NamedLogger.SECONDARY)]
	//[Implements(typeof(SubLogger), InjectionBindingScope.SINGLE_CONTEXT, LogManager.NamedLogger.TERTIARY)]
	public class NullDebugLog : ILogger
	{
		public void Log(object toLog)
		{
		}

		public void LogFormat(string format, params object[] parameters)
		{
		}

		public void LogWarning(object toLog)
		{
		}

		public void LogWarningFormat(string format, params object[] parameters)
		{
		}

		public void LogError(object toLog)
		{
		}

		public void LogErrorFormat(string format, params object[] parameters)
		{
		}
	}
}