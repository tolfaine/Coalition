using System;
using System.Collections.Generic;
using strange.extensions.injector.api;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	[Serializable]
	[Implements(InjectionBindingScope.CROSS_CONTEXT)]
	public class AssetBundleModel
	{
		public Dictionary<string, AssetBundleMetadata> AssetbundleToModelList = new Dictionary<string, AssetBundleMetadata>();
	}

	[Serializable]
	public class AssetBundleMetadata
	{
		public string AssetBundleName;
		public AssetBundle Bundle;
#if UNITY_5
		public Object[] Objects;
#endif
		public GameObject ParentGo;
		//public List<string> levelsContainedIn; //right now I'm assuming the bundle and the level match, which will not always be true in the future
	}

	public enum BUNDLE_TYPE
	{
		LEVEL,
		ASSETS,
		AUDIO,
	}
}