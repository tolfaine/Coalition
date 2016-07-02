using System;
using System.Collections.Generic;

namespace interfaces
{
	[Serializable]
	public class InterfaceMap
	{
		public Dictionary<string, string> interfaceMap = new Dictionary<string, string>();
		public Dictionary<string, List<string>> interfaceToMultipleMaps = new Dictionary<string, List<string>>(); //for analyticsMultiManager
		public Dictionary<string, string> namedStringTypes = new Dictionary<string, string>();
		public Dictionary<string, int> namedIntTypes = new Dictionary<string, int>();
		public List<NamedInterfaceBinding> namedInterfaceBindings = new List<NamedInterfaceBinding>();

		[Serializable]
		public class NamedInterfaceBinding
		{
			public string InterfaceToMap;
			public string TargetType;
			public string nameToMapTo;
		}
	}
}