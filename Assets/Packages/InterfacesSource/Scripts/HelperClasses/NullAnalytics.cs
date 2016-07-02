using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	public class NullAnalytics : IAnalytics
	{
		public void Init(List<string> initializationInfo)
		{
		}

		public void StartSession()
		{
		}

		public void EndSession()
		{
		}

		public void Log(string EventName, bool isTimed = false)
		{
		}

		public void Log(string EventName, Dictionary<string, string> parameters)
		{
		}
	}
}