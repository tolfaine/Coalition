using System.Collections.Generic;

namespace interfaces
{
	public interface IAnalytics
	{
		void Init(List<string> initializationInfo);

		void StartSession();

		void EndSession();

		void Log(string EventName, bool isTimed = false);

		//TODO: expose app specific parameter dictionary that is strongly typed and parameters reused
		void Log(string EventName, Dictionary<string, string> parameters);
	}
}