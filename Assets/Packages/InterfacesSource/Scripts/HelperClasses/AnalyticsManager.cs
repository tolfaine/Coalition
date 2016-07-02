using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace interfaces
{
	public enum InterfaceNamedInjections
	{
		ANALYTICS_INITIALIZATION = 1000
	}

	public class ListOfAnalyitics
	{
		public List<IAnalytics> listOfAnalytics;
	}

	[Implements(typeof(IAnalytics))]
	public class StubAnalyticsManager : IAnalytics
	{
		//[Inject]
		//public ListOfAnalyitics ListOfAnalyitics { get; set; }
		//[Inject(InterfaceNamedInjections.ANALYTICS_INITIALIZATION)]
		//public List<string> analyticsInitialization { get; set; }
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
	public class AnalyticsManager : IAnalytics
	{
		[Inject]
		public List<IAnalytics> AnalyticsSystemsToUse { get; set; }

		//public ListOfAnalyitics ListOfAnalyitics { get; set; }
		[Inject(InterfaceNamedInjections.ANALYTICS_INITIALIZATION)]
		public List<string> analyticsInitialization { get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			//AnalyticsSystemsToUse = ListOfAnalyitics.listOfAnalytics.ToArray();
		}
		public void Init(List<string> initialization)
		{
			InitInternal();
		}
		void InitInternal()
		{
			if(analyticsInitialization != null)
			{
				for(var i = 0; i < analyticsInitialization.Count; i++)
				{
					if(i >= AnalyticsSystemsToUse.Count)
						break;
					var currentAnalyticsSystem = AnalyticsSystemsToUse[i];
					var list = analyticsInitialization[i];
					var subList = new List<string>();
					if (list != null)
					{
						try
						{
							subList = list.JsonDeserialize<List<string>>();
						}
						catch (Exception e)
						{
							UnityEngine.Debug.LogError("Could not initiale analytics sublist:"+e.Message);
						}
					}
					currentAnalyticsSystem.Init(subList);
				}
			}
		}

		public void StartSession()
		{
			Init(null);
			for(var i = 0; i < AnalyticsSystemsToUse.Count; i++)
			{
				AnalyticsSystemsToUse[i].StartSession();
			}
		}

		public void EndSession()
		{
			for(var i = 0; i < AnalyticsSystemsToUse.Count; i++)
			{
				AnalyticsSystemsToUse[i].EndSession();
			}
		}

		public void Log(string EventName, bool isTimed = false)
		{
			for(var i = 0; i < AnalyticsSystemsToUse.Count; i++)
			{
				AnalyticsSystemsToUse[i].Log(EventName, isTimed);
			}
		}

		public void Log(string EventName, Dictionary<string, string> parameters)
		{
			for(var i = 0; i < AnalyticsSystemsToUse.Count; i++)
			{
				AnalyticsSystemsToUse[i].Log(EventName, parameters);
			}
		}
	}
}