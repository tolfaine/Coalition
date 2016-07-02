using System;
using System.Collections;
using System.Collections.Generic;
using interfaces;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using UnityEngine;
using Object = UnityEngine.Object;

namespace implementations
{
	public class LoadNextLevelCommand : Command
	{
		//private const string SceneName = "level"; //TODO: get this from some sort of config

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextParent { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public IAnalytics analytics { get; set; }

		[Inject]
		public BuildConfigSO buildConfig { get; set; }

		[Inject]
		public ISoundPlayer soundPlayer { get; set; }

		[Inject]
		public IRoutineRunner runner { get; set; }

		[Inject]
		public VRCameraFactory cameraFactory { get; set; }

		[Inject(NamedInjectionsCore.PLATFORM)]
		public string platform { get; set; }

		[Inject]
		public LevelLoadedSignal levelLoaded { get; set; }

		public override void Execute()
		{
			Retain();
			analytics.Init(null);

			logger.Log("setting up level for platform:" + platform);
			analytics.Log("loading level");

			runner.StartCoroutine(loadLevel());
		}

		private void KillOldWorld()
		{
			GameObject world = GameObject.Find("World");
			if(world != null)
				KillGORecursive(world);
		}

		private void KillGORecursive(GameObject go)
		{
			foreach (Transform children in go.transform)
			{
				KillGORecursive(children.gameObject);
			}
			Object.Destroy(go);
		}

		private IEnumerator loadLevel()
		{
			KillOldWorld();
			//NOTE: the yield return null below should cover the issues related to startup

			//yield return null; //let them get cleaned up
			analytics.Log("levelLoadTime", true);

			var exportedLevelManifest = Resources.Load<ExportedLevelManfest>("ExportedLevelManfest");
			//var exportedLevelManifest = (ExportedLevelManfest)www.assetBundle.Load("ExportedLevelManfest", typeof(ExportedLevelManfest));
			//there's an android thing that's supposed to go here...

			int levelIndexToLoad = 0;
			List<string> availableLevelsByName = exportedLevelManifest.exportedLevelNames;
			logger.Log(
				string.Format("current level:" + Application.loadedLevelName + " all levels:" +
							string.Join(",", availableLevelsByName.ToArray())));
			if(availableLevelsByName.Contains(Application.loadedLevelName))
			{
				levelIndexToLoad = availableLevelsByName.IndexOf(Application.loadedLevelName);
				if(levelIndexToLoad < availableLevelsByName.Count)
					levelIndexToLoad++;
			} else
				logger.Log("first level loaded");
			string sceneNameToLoad = availableLevelsByName[levelIndexToLoad];
			logger.Log("loading level:" + sceneNameToLoad);
			Application.LoadLevel(sceneNameToLoad);

			//yield return null;
			//Application.LoadLevel(SceneName);

			yield return null;
			GameObject world = GameObject.Find("World");
			world.transform.parent = contextParent.transform;

			//yield return new WaitForSeconds(1f); //make sure there is an audio listener
			var audioListener = cameraFactory.GetComponentInChildren<AudioListener>();
			if(audioListener == null)
				throw new Exception("could not find audio listener");
			yield return runner.StartCoroutine(soundPlayer.Init()); //TODO: this should have some correlation with the level?
			analytics.Log("levelLoadTime", true);
			levelLoaded.Dispatch();

			Release();
		}
	}

	[Implements]
	public class LevelLoadedSignal : Signal
	{
	}
}