using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public static class GameObjectExtensionMethods
	{
		public static void SetHideflagsRecursive(this MonoBehaviour mono, HideFlags flags)
		{
			var go = mono.gameObject;
			SetHideflagsRecursive(go, flags);
		}

		private static void SetHideflagsRecursive(GameObject go, HideFlags flags)
		{
			foreach(Transform child in go.transform)
			{
				SetHideflagsRecursive(child.gameObject, flags);
			}
			go.hideFlags = flags;
		}
		public static void DestroyImmediateRecursive(this MonoBehaviour mono, GameObject go)
		{
			DestroyImmediateRecursive(go);
		}

		private static void DestroyImmediateRecursive(GameObject go)
		{
			try
			{
				foreach(Transform child in go.transform)
				{
					DestroyImmediateRecursive(child.gameObject);
				}
				Object.DestroyImmediate(go);
			}
			catch(Exception e)
			{
				Debug.Log("Cannot destroy camera child due to :" + e);
			}
			//if(go != null)
			//	StartCoroutine(KillCameraDelayed(go));
		}
	}
}