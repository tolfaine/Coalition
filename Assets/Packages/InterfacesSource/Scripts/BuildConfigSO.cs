using UnityEngine;

namespace interfaces
{
	//TODO: property drawer and SerializedProperty example
	public class BuildConfigSO : ScriptableObject
	{
#pragma warning disable 0649

		public enum BuildTypes
		{
			Development,
			Testing,
			ReleaseCandidate,
			Release,
		}

		//[EnumPopup(BuildTypes)]
		public BuildTypes BuildType;

		//[Popup("String", "String1", "String2", "String3", "String4")]
		public int BuildNumber;

		public string ProductName;
#pragma warning restore 0649
	}
}