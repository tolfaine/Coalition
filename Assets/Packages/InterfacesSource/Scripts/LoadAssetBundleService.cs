using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	[Implements]
	public class LoadAssetBundleService
	{
		[Inject]
		public AssetBundleModel model { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public IAnalytics analytics { get; set; }

		[Inject(NamedInjectionsCore.PLATFORM)]
		public string platform { get; set; }

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextParent { get; set; }

		[Inject]
		public IRoutineRunner runner { get; set; }

		public IEnumerator LoadAssetBundle(string BundleNameToLoad, BUNDLE_TYPE assetBundleType)
		{
			if(model.AssetbundleToModelList.ContainsKey(BundleNameToLoad))
			{
				yield break;
			}

			SetAllObjectsToDoNotDestroyOnLoad(); //all initial objects shouldn't be killed

			analytics.Log("levelAssetbundleLoadTime" + BundleNameToLoad, true);

			var levelName = string.Format("{0}_{1}_{2}.unity3d", getTypePrefix(assetBundleType), platform, BundleNameToLoad);
			//Application.LoadLevelAdditiveAsync(levelName);

			var path = (Application.streamingAssetsPath + "/" + levelName);
			if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
				path = path.Replace("/", "\\");
			var uri = "file://" + path;
			if(Application.platform == RuntimePlatform.Android)
			{
				//looks like this should work: http://stackoverflow.com/questions/8246917/how-to-access-unity-assets-natively-on-android-or-iphone
				uri = path; //internally i think this ends up being "jar:file://" + Application.dataPath + "!/assets/";
			}
			//WWW www = new WWW("file://"+Path.Combine(Application.streamingAssetsPath, levelName));
			var www = new WWW(uri); //WWW.LoadFromCacheOrDownload(uri, 1);
			yield return www;
			while(string.IsNullOrEmpty(www.error) && !www.isDone)
			{
				yield return null;
			}

			analytics.Log("levelAssetbundleLoadTime" + BundleNameToLoad, true);
			if(!string.IsNullOrEmpty(www.error))
			{
				logger.LogError("Could not load level:" + levelName + " at uri" + uri);
				analytics.Log("Level load failure");
				analytics.Log("load level failure", new Dictionary<string, string> { { "error", www.error } });
				www.Dispose();
				yield break;
			}
			logger.Log("Success loading asset bundle:" + levelName);
			analytics.Log("Level load success");
			www.assetBundle.LoadAll();
			var meta = new AssetBundleMetadata
			{
#if UNITY_5
				Objects = www.assetBundle.LoadAllAssets(),
#endif
				Bundle = www.assetBundle,
				AssetBundleName = BundleNameToLoad
			};

			if(assetBundleType == BUNDLE_TYPE.LEVEL)
			{
				Application.LoadLevelAdditive(BundleNameToLoad.ToLower());
				yield return null;
				meta.ParentGo = GameObject.Find(BundleNameToLoad);
				logger.Log("post level load in asset bundle");
				if(meta.ParentGo == null)
				{
					logger.LogError("Did not find corresponding gameobject for level type bundle with gameobject name:" +
									BundleNameToLoad);
				}
				else
				{
					meta.ParentGo.transform.parent = contextParent.transform;
				}
			}
			model.AssetbundleToModelList.Add(BundleNameToLoad, meta);
			www.Dispose();
			logger.Log("done loading asset bundle:" + BundleNameToLoad);
		}

		private void debugAssetbundle(AssetBundleMetadata meta)
		{
#if UNITY_5
			foreach(var o in meta.Objects)
			{
				Debug.Log("Object:" + o.name + " instanceid:" + o.GetInstanceID());
			}
			Debug.Log("Scenes:" + meta.Bundle.GetAllScenePaths().JsonSerialize()); //["Assets/credits.unity"]
			Debug.Log("assets:" + meta.Bundle.GetAllAssetNames().JsonSerialize());
#endif
		}

		private void SetAllObjectsToDoNotDestroyOnLoad()
		{
			var allObjects = Object.FindObjectsOfType<GameObject>();
			foreach(var go in allObjects)
			{
				Object.DontDestroyOnLoad(go);
			}
		}

		public void DisableAllLevels()
		{
			foreach(var assetBundleMetadata in model.AssetbundleToModelList)
			{
				var levelParent = assetBundleMetadata.Value.ParentGo;
				if(levelParent != null)
				{
					levelParent.SetActive(false);
				}
			}
		}

		private string getTypePrefix(BUNDLE_TYPE type)
		{
			string typePrefix;
			switch(type)
			{
				case BUNDLE_TYPE.ASSETS:
					typePrefix = "Assets";
					break;
				case BUNDLE_TYPE.AUDIO:
					typePrefix = "AudioController";
					break;
				case BUNDLE_TYPE.LEVEL:
					typePrefix = "Level";
					break;
				default:
					logger.LogError("Unkown asset bundle type" + type);
					throw new Exception("Unkown level type!" + type);
			}
			return typePrefix;
		}

		#region unload

		public IEnumerator UnloadAssetBundle(string BundleNameToUnload)
		{
			if(!model.AssetbundleToModelList.ContainsKey(BundleNameToUnload))
				yield break;
			yield return runner.StartCoroutine(unloadLevel(model.AssetbundleToModelList[BundleNameToUnload], BundleNameToUnload))
				;
		}

		private IEnumerator unloadLevel(AssetBundleMetadata toUnload, string BundleNameToUnload)
		{
			logger.Log(
				string.Format("current level:" + Application.loadedLevelName + " level trying to unload:" + BundleNameToUnload +
							  " type:" + toUnload));

			model.AssetbundleToModelList.Remove(BundleNameToUnload);

			analytics.Log("levelUnloadTime" + BundleNameToUnload, true);
			logger.Log("unloading level:" + BundleNameToUnload);
			if(toUnload.ParentGo != null)
			{
				Object.Destroy(toUnload.ParentGo);
				yield return null;
			}
			toUnload.Bundle.Unload(true); //put this before destroying?

			analytics.Log("levelUnloadTime" + BundleNameToUnload, true);
		}

		#endregion //unload
	}
}