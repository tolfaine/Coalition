using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if NUNIT
using NUnit.Framework;
using UnityTest;
#endif

//TODO: pass in BUILD_NUMBER in System.Environment.GetCommandLineArgs

namespace interfaces
{
	public class CIRunner
	{
		private static readonly ILogger Log = new UnityLogger();
		private const string TARGET_DIR = "build"; //Application.dataPath + "/../build""/../build";

#if NUNIT
	//asink/Unit Tests/Run Unit Tests %#r
	[MenuItem("asink/run tests %#t")]
	private static void RunTests()
	{
		UnitTestView.RunAllTestsBatch();
		//NunitTestRunner.RunAllTests(EditorUserBuildSettings.activeBuildTarget.ToString());
	}
#endif //NUNITY

		//waiting on EditorApplication.isUpdating and EditorApplication.isCompiling is the *correct* solution
		//the workable one is to split in 2 parts -- import configs, and then do build
		[MenuItem("asink/CI/import packages from config based build")]
		public static void ImportPackagesFromConfigBuild()
		{
			DeletePackageFolders();
			_ImportPackagesFromConfigBuild();
			_ImportPackagesFromConfigBuild();
		}
		[MenuItem("asink/CI/import packages from config based build writeover")]
		public static void ImportPackagesFromConfigBuildTryToReimport()
		{
			DeletePackageFolders();
			_ImportPackagesFromConfigBuild();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			_ImportPackagesFromConfigBuild();
		}
		[MenuItem("asink/CI/dive import packages from config based build writeover")]
		public static void DiveImportPackagesFromConfigBuildTryToReimport()
		{
			ImportPackagesForSpecifiedBuildType(ConfigBasedBuild.LoadAndroidDiveConfig());
		}
		public static void ImportPackagesForSpecifiedBuildType(ConfigBasedBuild.BuildConfiguration buildConfig)
		{
			DeletePackageFolders();
			_ImportPackagesFromConfigBuild(buildConfig);
			EditorApplication.SaveAssets(); //this is to make sure that the platform gets saved, as the switch would not get recognized previously
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			_ImportPackagesFromConfigBuild(buildConfig);
		}

		private static float timeToWait = 5f;
		public static void StayOpenFor5Seconds()
		{
			DeletePackageFolders();

			_ImportPackagesFromConfigBuild();
			_ImportPackagesFromConfigBuild();
			EditorApplication.ExecuteMenuItem("asink/CI/import packages from config based build");
			EditorApplication.update += quitAfterTime;
		}

		private static bool didSecondImport = false;
		protected static void quitAfterTime()
		{
			if(!didSecondImport && EditorApplication.timeSinceStartup > timeToWait/2f)
			{
				_ImportPackagesFromConfigBuild();
				didSecondImport = true;
			}
			if(EditorApplication.timeSinceStartup > timeToWait)
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
				try
				{
					if(Directory.Exists("StreamingAssets"))
						Debug.Log("Files in streamingassets:" + Directory.GetFiles("StreamingAssets/").JsonSerialize());
				}
				catch (Exception e)
				{
					Debug.LogError("Error while looking at streaming assets:"+e);
				}
				
				EditorApplication.Exit(0);
			}
		}

		public static void DeletePackageFolders()
		{
			DeleteDirectoryIfItExists("Interfaces");
			DeleteDirectoryIfItExists("Packages");
			DeleteDirectoryIfItExists("Plugins");
			try
			{
				if(Directory.Exists("StreamingAssets"))
				{
					var files = Directory.GetFiles("StreamingAssets", "*unity3d");
					foreach(var file in files)
					{
						string fullPath = "StreamingAssets" + Path.DirectorySeparatorChar + file;
						try
						{
							File.Delete(fullPath);
						}
						catch(Exception e)
						{
							Debug.Log("could not delete file:" + fullPath + " because:" + e.Message + " full :" + e);
						}
					}
				}
			}
			catch(Exception e)
			{
				Debug.LogError("Error deleting streamingAssets:" + e);
			}
			
		}

		static void ImportPackage(string packageName)
		{
			if(packageName.ToLower().Contains("interfaces"))
			{
				Debug.LogError("not imoprting interfaces package for now -- leaving that alone");
				return;
			}
			try
			{
				Debug.Log("IMPORTING PACKAGE: " + packageName);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

				ConfigBasedBuild.ImportPackage(packageName);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
			catch(Exception e)
			{
				Debug.LogError("Exception while importing package:" + packageName + " error:" + e);
				try
				{
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					ConfigBasedBuild.ImportPackage(packageName);
				}
				catch
				{
					Debug.LogError("Could not import package(again):" + packageName + " error:" + e);
				}
			}
			Debug.Log("imported package:" + packageName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}

		private static void _ImportPackagesFromConfigBuild()
		{
			_ImportPackagesFromConfigBuild(ConfigBasedBuild.LoadConfigFileForCurrentPlatform());
		}
		private static void _ImportPackagesFromConfigBuild(ConfigBasedBuild.BuildConfiguration buildToLoad)
		{
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			//make sure we have the stuff from the last update

			var buildConfig = buildToLoad;//ConfigBasedBuild.LoadConfigFileForCurrentPlatform();
			buildConfig.editorUserBuildSettings.SetStaticInstanceFromThis();
			buildConfig.buildConfig.SetStaticInstanceFromThis(buildConfig.editorUserBuildSettings.currentBuildTargetGroup);
			Debug.Log("test1");
			
			//TODO: plugins, straemingassets except for donotdelete and donotdelete.meta
			Debug.Log("test2");
			foreach(var package in buildConfig.packagesToAdd)
			{
				ImportPackage(package);
			}
			Debug.Log("test3");
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			Debug.Log("test4");
			


			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
		}

		private static void DeleteDirectoryIfItExists(string directory)
		{
			if(Directory.Exists(directory))
				Directory.Delete(directory);
		}

		//waiting on EditorApplication.isUpdating and EditorApplication.isCompiling is the *correct* solution
		//the workable one is to split in 2 parts -- import configs, and then do build
		[MenuItem("asink/CI/config current based build")]
		public static void PerformConfigBuildForCurrentPlatform()
		{
			BuildFromFile(ConfigBasedBuild.CurrentPlatformConfigFile());
		}

		[MenuItem("asink/CI/config current based build")]
		public static void PerformConfigBuild()
		{
			var configFile = _cliHelper.GetStringFromCommandLine(CommandLineHelpers.ParameterNames.BUILD_CONFIG_FILE_NAME);
			var buildConfig = ConfigBasedBuild.LoadConfigFile(configFile);
			ImportPackagesForSpecifiedBuildType(buildConfig);
			//ImportPackagesFromConfigBuildTryToReimport(buildConfig);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			
			Debug.Log("Building with config file:" + configFile);
			BuildFromFile(configFile);

			Debug.Log("Done with config build");
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
		}

		public static void BarTest()
		{
			Debug.Log("FOO TEST");
		}

		[MenuItem("asink/CI/foo test")]
		public static void FooTest()
		{
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			Debug.Log("FOO TEST");
		}

		private static void BuildFromFile(string fileName)
		{
			try
			{
				SetBuildNumber(_cliHelper.GetIntFromCommandLine(CommandLineHelpers.ParameterNames.BUILD_NUMBER));
			}
			catch
			{
			}

			var buildConfig = ConfigBasedBuild.LoadConfigFile(fileName);

			var cibuilder = new CIBuilder
			{
				buildTarget = EditorUserBuildSettings.activeBuildTarget,
				buildOptions = buildConfig.buildOptions,
				scenes = FindEnabledEditorScenes(),
				targetLocation = TARGET_DIR,
				targetName = buildConfig.app_output_name
			};

			Debug.Log(string.Format("{0} location {1} finalPath{2}", buildConfig.app_output_name, cibuilder.targetLocation,
				cibuilder.GetFinalPathName()));
			cibuilder.RunTests();
			cibuilder.Build();
		}

		[MenuItem("asink/CI/pc build")]
		private static void PerformPCConfigBuild()
		{
			BuildFromFile(ConfigBasedBuild.PlatformConfigFile(BuildTarget.StandaloneWindows));
		}

		[MenuItem("asink/CI/android build")]
		private static void PerformAndroidConfigBuild()
		{
#if UNITY_4_6
			BuildFromFile(ConfigBasedBuild.PlatformConfigFile(BuildTarget.Android, AndroidBuildSubtarget.ETC2));
#else
		BuildFromFile(ConfigBasedBuild.PlatformConfigFile(BuildTarget.Android, MobileTextureSubtarget.ETC2));
#endif
		}

		private static string[] FindEnabledEditorScenes()
		{
			var EditorScenes = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes)
			{
				if(!scene.enabled) continue;
				EditorScenes.Add(scene.path);
			}
			return EditorScenes.ToArray();
		}

		private static readonly CommandLineHelpers _cliHelper = new CommandLineHelpers();

		private static void SetBuildNumber(int buildNumber)
		{
			try
			{
				Log.Log("build number: " + buildNumber);
				PlayerSettings.bundleVersion = buildNumber.ToString(); //this may not belong here...
				var configSO = ResourceLoader.Load<BuildConfigSO>();
				configSO.BuildNumber = buildNumber;

				EditorUtility.SetDirty(configSO);

				AssetDatabase.SaveAssets();
				//AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Log.LogError("Error setting build number" + e);
			}
		}

		private class CIBuilder
		{
			private readonly ILogger Log = new UnityLogger();
			public string[] scenes { get; set; }
			public string targetLocation { get; set; }
			public string targetName { get; set; }
			public BuildTarget buildTarget { get; set; }
			public BuildOptions buildOptions { get; set; }

			public void Build()
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
				Log.Log("Active build: " + buildTarget + " to path : " + GetFinalPathName() + " Scenes: " + string.Join(",", scenes));

				var suffix = "";
				//some platforms require the target to be a foldername, others require a file with a specific extension
				switch (buildTarget)
				{
					case BuildTarget.StandaloneWindows:
					case BuildTarget.StandaloneWindows64:
						suffix = Path.DirectorySeparatorChar + targetName + ".exe";
						break;

					case BuildTarget.StandaloneOSXIntel:
					case BuildTarget.StandaloneOSXIntel64:
						suffix = Path.DirectorySeparatorChar + targetName + ".app";
						break;

					case BuildTarget.Android:
					case BuildTarget.Tizen:
					case BuildTarget.SamsungTV:
						suffix = Path.DirectorySeparatorChar + targetName + ".apk";
						break;

#if UNITY_4_6
					case BuildTarget.iPhone:
#else
				case BuildTarget.iOS:
#endif
					case BuildTarget.WebPlayer:
						suffix = Path.DirectorySeparatorChar + targetName;
						break;
				}
				var targetToBuildFinal = GetFinalPathName() + suffix;
				Log.Log("path name passed into build player: " + targetToBuildFinal);
				var res = BuildPipeline.BuildPlayer(scenes, targetToBuildFinal, buildTarget, buildOptions);
				if(res.Length > 0)
					throw new Exception("BuildPlayer failure: " + res);

				Log.Log("Output of running build file : " + res);
			}

			public string GetFinalPathName()
			{
				//todo: string formatting
				var targetDir = targetLocation;
				//var targetDir = targetLocation + Path.DirectorySeparatorChar + buildTarget;
				targetDir = Path.GetFullPath(targetDir);

				if(Directory.Exists(targetDir))
					Directory.Delete(targetDir, true); //AssetDatabase.DeleteAsset(targetDir); //Directory.Delete(targetDir, true);

				Directory.CreateDirectory(targetDir);
				/*
				var rawFullPathFromdataPath =
					string.Format(
						"{0}" + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "{1}" + Path.DirectorySeparatorChar +
						"{2}", Application.dataPath, targetLocation, buildTarget);
				*/
				var rawFullPathFromdataPath =
					string.Format("{0}" + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "{1}",
						Application.dataPath, targetLocation);
				var fullpath = Path.GetFullPath(rawFullPathFromdataPath);

				Log.Log("Final fullpath: " + fullpath);

				if(!Directory.Exists(fullpath))
					Directory.CreateDirectory(fullpath);

				return fullpath; //note this is because fb's api shits itself without the full path
			}

			public void RunTests()
			{
#if NUNIT
			UnitTestView.RunAllTestsBatch();
			//NunitTestRunner.RunAllTests(buildTarget.ToString());
			RunEditorTests.RunAllTests(buildTarget.ToString());
#endif //#if NUNIT
			}
		}
	}
}