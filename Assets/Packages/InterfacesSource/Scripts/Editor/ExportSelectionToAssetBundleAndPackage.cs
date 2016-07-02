using System.IO;
using interfaces;
using UnityEditor;
using UnityEngine;

public class ExportSelectionToAssetBundleAndPackage
{
	[MenuItem("asink/export assets/ExportSelection to win32")]
	public static void ExportSelectionToWindows()
	{
		var obj = Selection.activeObject;
		var filePath = AssetDatabase.GetAssetPath(obj);
		Export(filePath, BuildTarget.StandaloneWindows);
	}

	[MenuItem("asink/export assets/ExportSelection to all known export targets")]
	public static void ExportToAllExportTargets()
	{
		var obj = Selection.activeObject;
		var filePath = AssetDatabase.GetAssetPath(obj);

		Export(filePath, BuildTarget.StandaloneWindows64);
		Export(filePath, BuildTarget.StandaloneOSXIntel64);
		Export(filePath, BuildTarget.iPhone);
		Export(filePath, BuildTarget.Android);

		Export(filePath, BuildTarget.StandaloneWindows);
	}

	public static void Export(string assetPath, BuildTarget buildTarget)
	{
		var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof (Object));
		var fileName = Path.GetFileNameWithoutExtension(assetPath) + "_" + buildTarget + ".unity3d";
		var destinationPath =
			Path.GetFullPath(ExportAssetBundles.CombinePaths(Application.dataPath, "StreamingAssets", fileName));
		//Debug.Log("Destination path :" + destinationPath);
		BuildPipeline.BuildAssetBundle(obj, new[] {obj}, destinationPath, BuildAssetBundleOptions.CollectDependencies,
			buildTarget);

		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		var packagePath =
			Path.GetFullPath(ExportAssetBundles.CombinePaths(ConfigBasedBuild.subProjectsPath(),
				Path.GetFileNameWithoutExtension(assetPath) + "_" + buildTarget + ".unitypackage"));
		//Debug.Log("path output:" + packagePath + " input:" + assetPath + " where i put the first stage bundle output:" +
				//destinationPath);
		//BuildPipeline.BuildAssetBundle(AssetDatabase.LoadAssetAtPath(outputPath, typeof (Object)), new Object[] {},
		//	Path.GetFullPath(CombinePaths(ConfigBasedBuild.subProjectsPath(), "Level_" + LevelSuffix + ".unitypackage")));
		AssetDatabase.ExportPackage("Assets/StreamingAssets/" + fileName, packagePath, ExportPackageOptions.Recurse);
	}
}