using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public class CameraSettingsSO : ScriptableObject
	{
		[SerializeField]
		private Object PrefabToInstantiate;

		[NonSerialized]
		private GameObject myCamera;

		public GameObject Create()
		{
			if(myCamera != null)
			{
				return myCamera;
			}
			myCamera = Instantiate(PrefabToInstantiate) as GameObject;
			return myCamera;
		}

		public static List<CameraSettingsSO> loadAllCameraSettingsSo()
		{
			return new List<CameraSettingsSO>(Resources.LoadAll<CameraSettingsSO>(""));
			//TODO: put camera settings folder in it's own thing
		}
	}
}