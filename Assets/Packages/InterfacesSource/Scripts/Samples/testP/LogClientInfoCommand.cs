using strange.extensions.command.impl;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	public class LogClientInfoCommand : Command
	{
		[Inject]
		public IAnalytics analytics { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public BuildConfigSO buildConfig { get; set; }
		[Inject]
		public IMonobehaviourLifecycleMessages lifecycle { get; set; }

		public override void Execute()
		{
			var clientInfo = new Dictionary<string, string>
			{
				{"productName", buildConfig.ProductName},
				{"buildNumber", buildConfig.BuildNumber.ToString()},
				{"buildType", buildConfig.BuildType.ToString()},
				{"platform", Application.platform.ToString()},
				{"UserId", SystemInfo.deviceUniqueIdentifier}
			};
			analytics.StartSession();
			analytics.Log("clientinfo", clientInfo);
			logger.Log(clientInfo.JsonSerialize());
		}
	}
}