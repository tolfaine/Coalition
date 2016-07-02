using System;

namespace interfaces
{
	public class CommandLineHelpers
	{
		public enum ParameterNames
		{
			BUILD_CONFIG_FILE_NAME,
			BUILD_NUMBER
		}

		private static readonly ILogger Log = new LogManager();

		private string getFlagStringFromName(ParameterNames name)
		{
			return string.Format("-{0}=", name);
		}

		public string GetStringFromCommandLine(ParameterNames name)
		{
			var flagStringParamName = getFlagStringFromName(name);
			try
			{
				var args = Environment.GetCommandLineArgs();
				foreach(var arg in args)
				{
					if(!arg.Contains(flagStringParamName)) continue;
					var configFileName = arg.Replace(flagStringParamName, "");

					return configFileName;
				}
			}
			catch(Exception e)
			{
				Log.LogError("Error getting " + name + " ( " + flagStringParamName + ") : Exception-" + e);
				throw;
			}
			throw new Exception("Could not find " + name + " ( " + flagStringParamName + ") in command line arguments");
		}

		public int GetIntFromCommandLine(ParameterNames name)
		{
			try
			{
				return int.Parse(GetStringFromCommandLine(name));
			}
			catch(Exception e)
			{
				Log.LogError("Error getting " + name + " : Exception-" + e);
				throw;
			}
		}
	}
}