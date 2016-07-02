using System.Collections;

namespace interfaces
{
	//loads an asset bundle related to the level, and/or sets it active/inactive as appropriate
	[Implements]
	public class LoadLevelService
	{
		[Inject]
		public AssetBundleModel model { get; set; }

		[Inject]
		public LoadAssetBundleService AssetBundleService { get; set; }
		[Inject]
		public IRoutineRunner runner { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		public IEnumerator LoadLevel(string levelName, BUNDLE_TYPE bundleType)
		{
			if(bundleType == BUNDLE_TYPE.LEVEL)
			{
				AssetBundleService.DisableAllLevels();
				//NOTE: assumes only one active level at a time.  may want to change this later, or apss in an explicit list of levels to load
			}
			yield return runner.StartCoroutine(AssetBundleService.LoadAssetBundle(levelName, bundleType));

			if(!model.AssetbundleToModelList.ContainsKey(levelName))
			{
				logger.LogError("tried to load a level that is not in the list -- load asset bundle first, then load the level!  level name:" + levelName);
				yield break;
			}
			var meta = model.AssetbundleToModelList[levelName];
			if(meta.ParentGo != null)
			{
				meta.ParentGo.SetActive(true);
			}
			else
			{
				logger.LogError("could not find parent gameobject for level, likely due to a misnamed root game object in the scene that does not correspond to :" + levelName);
			}
			logger.Log("loaded level:" + levelName);
			yield return null;
			logger.Log("Frame after loading");
		}
	}
}