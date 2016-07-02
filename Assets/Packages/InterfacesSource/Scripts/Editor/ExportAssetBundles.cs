// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	//orginal export from selection scripts http://docs.unity3d.com/ScriptReference/BuildPipeline.BuildAssetBundle.html
	public class ExportAssetBundles
	{
		public static string LevelSuffix = "Default";

		public static string CombinePaths(string first, params string[] others)
		{
			// Put error checking in here :)
			var path = first;
			foreach(var section in others)
			{
				path = Path.Combine(path, section);
			}
			return path;
		}

		[MenuItem("asink/level assetbundle/export to all default level targets")]
		public static void ExportAllCurrentTargets()
		{
			ExportPlatformResource(BuildTarget.Android);
			ExportPlatformResource(BuildTarget.iPhone);
			ExportPlatformResource(BuildTarget.StandaloneWindows);
			ExportPlatformResource(BuildTarget.StandaloneWindows64);
			ExportPlatformResource(BuildTarget.StandaloneOSXIntel64);
		}

		[MenuItem("asink/level assetbundle/Build android asset bundle from all loaded scenes")]
		public static void ExportAndroidResource()
		{
			ExportPlatformResource(BuildTarget.Android);
		}

		[MenuItem("asink/level assetbundle/Build ios asset bundle from all loaded scenes")]
		public static void ExportResource()
		{
			ExportPlatformResource(BuildTarget.iPhone);
		}

		[MenuItem("asink/level assetbundle/Build win32 asset bundle from all loaded scenes")]
		public static void Exportwin32Resource()
		{
			ExportPlatformResource(BuildTarget.StandaloneWindows);
		}

		[MenuItem("asink/level assetbundle/Build win64 asset bundle from all loaded scenes")]
		public static void Exportwin64Resource()
		{
			ExportPlatformResource(BuildTarget.StandaloneWindows64);
		}

		[MenuItem("asink/level assetbundle/Build mac64 asset bundle from all loaded scenes")]
		public static void ExportmacResource()
		{
			ExportPlatformResource(BuildTarget.StandaloneOSXIntel64);
		}

		public static void ExportPlatformResource(BuildTarget targetPlatform,
			AndroidBuildSubtarget androidsubTarget = AndroidBuildSubtarget.ETC2)
		{
			var fileName = "Level_" + targetPlatform + "_" + LevelSuffix + ".unity3d";
			var path = Path.GetFullPath(CombinePaths(Application.dataPath, "StreamingAssets", fileName));
			//Path.GetFullPath(CombinePaths(Application.dataPath, "..", "testExported" + targetPlatform + "Level.unity3d"));


			//BuildOptions.BuildAdditionalStreamedScenes
			var paths = new List<string>();
			foreach(var scene in EditorBuildSettings.scenes)
			{
				paths.Add(scene.path);
			}
			BuildPipeline.BuildStreamedSceneAssetBundle(paths.ToArray(), path, targetPlatform);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			//export level manifest as a part of the default level package for now
			var levelManifests = AssetDatabase.FindAssets("t:ExportedLevelManfest", null);
			if(levelManifests == null || levelManifests.Length == 0)
			{
				SOCreator.SOCreateInstance<ExportedLevelManfest>();

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
				levelManifests = AssetDatabase.FindAssets("t:ExportedLevelManfest", null);
			}
			if(levelManifests.Length > 1)
				Debug.LogError("More than one level manifest found : " + string.Join(",", levelManifests));
			var levelManifestPath = AssetDatabase.GUIDToAssetPath(levelManifests[0]);
			//Debug.Log(levelManifestPath);
			var manifest =
				AssetDatabase.LoadAssetAtPath(levelManifestPath, typeof(ExportedLevelManfest)) as ExportedLevelManfest;
			if(manifest.exportedLevelNames == null)
			{
				manifest.exportedLevelNames = new List<string>();
			}

			manifest.exportedLevelNames.Clear();
			foreach(var editorBuildSettingsScene in EditorBuildSettings.scenes)
			{
				var levelName = Path.GetFileNameWithoutExtension(editorBuildSettingsScene.path);
				Debug.Log("Adding level with name:" + levelName);
				manifest.exportedLevelNames.Add(levelName);
			}
			EditorUtility.SetDirty(manifest);
			//save the updated levels
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			paths.Add(levelManifestPath);
			Debug.Log("all paths exported:" + string.Join(",", paths.ToArray()));
			//bundle.Load("ExportedLevelManifest", typeof (ExportedLevelManfest));

			//export package for consumption for local builds, default levels

			var packagePath =
				Path.GetFullPath(CombinePaths(ConfigBasedBuild.subProjectsPath(),
					"Level_" + LevelSuffix + "_" + targetPlatform + ".unitypackage"));
			//Debug.Log("path output:" + packagePath + " input:" + path);
			//BuildPipeline.BuildAssetBundle(AssetDatabase.LoadAssetAtPath(outputPath, typeof (Object)), new Object[] {},
			//	Path.GetFullPath(CombinePaths(ConfigBasedBuild.subProjectsPath(), "Level_" + LevelSuffix + ".unitypackage")));
			AssetDatabase.ExportPackage("Assets/StreamingAssets/" + fileName, packagePath, ExportPackageOptions.Recurse);

		}
	}
}