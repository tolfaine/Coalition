using System;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	public class SingletonManager : MonoBehaviour
	{
		private static SingletonManager _instance;
		private readonly Dictionary<Type, Component> _knownTypes = new Dictionary<Type, Component>();

		public static SingletonManager instance
		{
			get
			{
				if(_instance != null) return _instance;
				//this is needed because at the momentt, the singletonmonobehaviours set themselves as donotdestroyonload and stick around between levelss
				if(Application.isEditor)
				{
					SingletonManager[] oldInstances = FindObjectsOfType<SingletonManager>();
					//Debug.Log("Other singleton managers:" + oldInstances.Length);
					for(int i = 0; i < oldInstances.Length; i++)
					{
						SingletonManager oldInstance = oldInstances[i];
						if(oldInstance != null)
							DestroyImmediate(oldInstance.gameObject);
					}
				}

				var go = new GameObject("Singleton Manager instance(runtime only)");
				//caused the leaks
				//HideFlags.DontSave |HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor
				go.hideFlags = HideFlags.NotEditable;
				_instance = go.AddComponent<SingletonManager>();
				DontDestroyOnLoad(go);
				return _instance;
			}
		}

		public void Awake()
		{
			if(_instance != this)
			{
				//DestroyImmediate(gameObject);
			}
		}

		public T AddSingleton<T>() where T : MonoBehaviour
		{
			Type targetType = typeof(T);
			if(!_knownTypes.ContainsKey(targetType))
				_knownTypes.Add(targetType, gameObject.AddComponent<T>());
			return _knownTypes[targetType] as T; //casting isn't great at runtime, but neither are singletons.
			//there's an intelligent solution and this isn't it.  this is a start
		}
	}
}