using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class ExportSelectionAsPackage
	{
		private static readonly UnityLogger logger = new UnityLogger();
		[MenuItem("Assets/print assets directory")]
		public static string subProjectsPath()
		{
			//recurse backwards through the directories until you hit the subprojects path
			var startingPath = Path.GetFullPath(Application.dataPath);
			Debug.Log("starting full path:"+startingPath);
			var path = startingPath;
			while(!string.IsNullOrEmpty(path) && path.Length > 5)
			{
				var speculativeSubprojectsDir = Path.Combine(path, "subProjects");
				Debug.Log("Checking:"+speculativeSubprojectsDir);
				if(Directory.Exists(speculativeSubprojectsDir))
					return speculativeSubprojectsDir;

				path = Directory.GetParent(path).FullName;
			}
			throw new Exception("no subprojects path found");
		}

		public static bool isMainProject()
		{
			return (Application.dataPath.ToLower().Contains("mainproject")) ;
		}
		public static string absolutePathFromProjectPath(string projectPath)
		{
			return Path.GetFullPath(string.Join(Path.DirectorySeparatorChar + "", new[] { Application.dataPath, projectPath }));
		}

		[MenuItem("Assets/Export as Package %e")]
		public static void ExportAsPackage()
		{
			//AssetDatabase.Refresh();
			if(isMainProject())
			{
				Debug.LogError("Tried to export package as main project -- this has overwritten analytics many times over, so disablign this");
				return;
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			var packagePath = absolutePathFromProjectPath("Packages");
			var isPackageExport = Directory.Exists(packagePath);
			var pathToExport = absolutePathFromProjectPath("Interfaces");
			if(isPackageExport)
				pathToExport = Directory.GetDirectories(packagePath)[0];

			var pathElements = pathToExport.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			var packageName = pathElements[pathElements.Length - 1];
			var destinationOfpackage = subProjectsPath();

			var completeDestinationPath = subProjectsPath() + Path.DirectorySeparatorChar + packageName + ".unitypackage";
			logger.LogFormat("DestinationOfPackage:{0} packageName:{1} path to export{2} output unitypackage:{3}",
				destinationOfpackage, packageName, pathToExport, completeDestinationPath);
			var pathsToExport = new List<string> { isPackageExport ? "Assets/Packages" : "Assets/Interfaces" };

			//Debug.Log(isPackageExport);
			if(isPackageExport)
			{
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
			var pluginsDir = absolutePathFromProjectPath("Plugins");
			if(Directory.Exists(pluginsDir))
				pathsToExport.Add("Assets/Plugins");
			//TODO: create the manifest in a .meta file
			//just check to see if the paths have packages, interfaces, or plugins in the anem
			//AssetDatabase.GetAllAssetPaths()
			AssetDatabase.ExportPackage(pathsToExport.ToArray(), completeDestinationPath, ExportPackageOptions.Recurse);

			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			
		}

		[MenuItem("Assets/asink import packages %p")]
		public static void ImportPackages()
		{
			var guids = AssetDatabase.FindAssets("PackageNameInfo");
			foreach(var guid in guids)
			{
				var ta = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(TextAsset)) as TextAsset;
				var project = ta.text.JsonDeserialize<ProjectSetup>();
				foreach(var package in project.packagesToUse)
				{
					Debug.Log("Importing package:"+package.Name);
					ImportPackageByName(package.Name);
				}
			}
			//AssetDatabase.
			//EditorPrefs.GetString()
		}

		private static void ImportPackageByName(string packageName)
		{
			var sourceOfPackage = subProjectsPath() + Path.DirectorySeparatorChar;

			var completeDestinationPath = sourceOfPackage + packageName + ".unitypackage";
			AssetDatabase.ImportPackage(completeDestinationPath, false);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
		}

		[Serializable]
		public class PackageInfo
		{
			//public List<PackageInfo> dependencies;
			public string Name;
		}

		[Serializable]
		public class ProjectSetup : object
		{
			public List<PackageInfo> packagesToUse;
		}

		//to allow uninstall, etc
		[Serializable]
		public class InternalPackageRegistry
		{
			public List<string> guidsInPackage;
		}
	}
}