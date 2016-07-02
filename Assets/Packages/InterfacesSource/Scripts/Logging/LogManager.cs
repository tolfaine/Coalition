using System.Collections.Generic;

namespace interfaces
{
	[Implements]
	public class LogManager : ILogger
	{
		//TODO: maybe inject these?
		public List<ILogger> Loggers; //= new List<ILogger> {new NullDebugLog(), new UnityLogger()};

		public enum NamedLogger
		{
			PRIMARY,
			SECONDARY,
			TERTIARY
		}

		[Inject(NamedLogger.PRIMARY)]
		public ILogger logger { get; set; }

		[Inject(NamedLogger.SECONDARY)]
		public ILogger logger2 { get; set; }

		[Inject(NamedLogger.TERTIARY)]
		public ILogger logger3 { get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			Loggers = new List<ILogger>()
			{
				logger,logger2,logger3
			};
		}

		public void Log(object toLog)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].Log(toLog);
			}
		}

		public void LogFormat(string format, params object[] parameters)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].LogFormat(format, parameters);
			}
		}

		public void LogWarning(object toLog)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].Log(toLog);
			}
		}

		public void LogWarningFormat(string format, params object[] parameters)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].LogWarningFormat(format, parameters);
			}
		}

		public void LogError(object toLog)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].Log(toLog);
			}
		}

		public void LogErrorFormat(string format, params object[] parameters)
		{
			for(int i = 0; i < Loggers.Count; i++)
			{
				Loggers[i].LogErrorFormat(format, parameters);
			}
		}
	}
}