using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class ConfigBasedBuild
	{
		//TODO: move this to a test
		//public static string testConfigString =
		//	@"{""app_output_name"":""myTest"",""buildOptions"":""CompressTextures"",""editorUserBuildSettings"":{""allowDebugging"":false,""currentBuildTarget"":""StandaloneWindows"",""currentBuildTargetGroup"":""Standalone"",""development"":false,""explicitNullChecks"":false,""subtarget"":""Generic"",""symlinkLibraries"":false},""buildConfig"":{""bundleIdentifier"":""com.Company.ProductName"",""bundleVersion"":""1.0"",""companyName"":""DefaultCompany"",""defaultInterfaceOrientation"":""AutoRotation"",""defaultIsFullScreen"":true,""defaultScreenHeight"":768,""defaultScreenWidth"":1024,""displayResolutionDialog"":""Disabled"",""productName"":""Interfaces"",""renderingPath"":""Forward"",""scriptingDefineSymbols"":[""""],""stripUnusedMeshComponents"":false,""supportedAspectRatios"":[""AspectOthers"",""Aspect4by3"",""Aspect5by4"",""Aspect16by10"",""Aspect16by9""],""use32BitDisplayBuffer"":true,""useDirect3D11"":true,""usePlayerLog"":true},""InterfaceMapping"":{},""packagesToAdd"":[""foo"",""bar"",""baz""]}";

		[MenuItem("asink/CI/print current build settings(no packages for now)")]
		private static void PrintBuildSettings()
		{
			//should this be pulled from the first scene? or no?
			//what's the relationship to the original scene
			var interfaceMap = new InterfaceMap
			{
				interfaceMap = new Dictionary<string, string> { { "IVR", "DiveVR" } },
				interfaceToMultipleMaps = new Dictionary<string, List<string>>(),
				namedIntTypes = new Dictionary<string, int>(),
				namedStringTypes = new Dictionary<string, string>()
			};

			var config = new BuildConfiguration
			{
				app_output_name = "myTest",
				//buildOptions = Application.build
				editorUserBuildSettings = new SerializedEditorUserBuildSettings(),
				buildConfig = new SerializedPlayerSettings(),
				InterfaceMapping = interfaceMap,
				packagesToAdd = new List<string> { "foo", "bar", "baz" }
			};
			config.editorUserBuildSettings.GetFromStaticInstance();

			config.buildConfig.GetFromStaticInstance();
			//SetDefaultMappingsFile(config.InterfaceMapping);

			Debug.Log("Sample config :" + config.JsonSerialize());
		}

		private static void SetDefaultMappingsFile(InterfaceMap map)
		{
			var pathOfDefaultMappings = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("DefaultMappings"));
			//Debug.Log("path of default mappings:" + pathOfDefaultMappings + " text:" + File.ReadAllText(pathOfDefaultMappings));
			File.WriteAllText(pathOfDefaultMappings, map.JsonSerialize());
			AssetDatabase.Refresh(); //make sure that unity knows the file changed... may not need to do this
			AssetDatabase.SaveAssets();
		}

		[MenuItem("asink/CI/Load config file for current platform")]
		public static BuildConfiguration LoadConfigFileForCurrentPlatform()
		{
			return LoadConfigFile(CurrentPlatformConfigFile());
		}
		[MenuItem("asink/CI/Load config android dive ")]
		public static BuildConfiguration LoadAndroidDiveConfig()
		{
			return LoadConfigFile("Android_ETC2PackageNameInfoDive.txt");
		}
		public static string CurrentPlatformConfigFile()
		{
			return currentHumanReadiblePlatformName() + "PackageNameInfo.txt";
		}

#if UNITY_4_6

		public static string PlatformConfigFile(BuildTarget target, AndroidBuildSubtarget subtarget = AndroidBuildSubtarget.Generic)
		{
			return HumanReadiblePlatformName(target, subtarget) + "PackageNameInfo.txt";
		}

		private static string currentHumanReadiblePlatformName()
		{
			return HumanReadiblePlatformName(EditorUserBuildSettings.activeBuildTarget,
				EditorUserBuildSettings.androidBuildSubtarget);
		}

		private static string HumanReadiblePlatformName(BuildTarget target, AndroidBuildSubtarget subtarget)
		{
			return target == BuildTarget.Android
				? target + "_" + subtarget
				: target.ToString();
		}

#else

	public static string PlatformConfigFile(BuildTarget target,
		MobileTextureSubtarget subtarget = MobileTextureSubtarget.Generic)
	{
		return HumanReadiblePlatformName(target, subtarget) + "PackageNameInfo.txt";
	}

	private static string currentHumanReadiblePlatformName()
	{
		return HumanReadiblePlatformName(EditorUserBuildSettings.activeBuildTarget,
			EditorUserBuildSettings.androidBuildSubtarget);
	}

	private static string HumanReadiblePlatformName(BuildTarget target,
		MobileTextureSubtarget subtarget = MobileTextureSubtarget.Generic)
	{
		return target == BuildTarget.Android ? target + "_" + subtarget : target.ToString();
	}

#endif

		public static BuildConfiguration LoadConfigFile(string fileName)
		{
			var filePath = subProjectsPath() + Path.DirectorySeparatorChar + fileName;
			var text = File.ReadAllText(filePath);
			var config = text.JsonDeserialize<BuildConfiguration>();

			config.editorUserBuildSettings.SetStaticInstanceFromThis();
			config.buildConfig.SetStaticInstanceFromThis(config.editorUserBuildSettings.currentBuildTargetGroup);
			SetDefaultMappingsFile(config.InterfaceMapping);

			return config;
		}

		[MenuItem("asink/CI/Save config file from currentSettings")]
		private static void SaveConfigFile()
		{
			var configFileName = currentHumanReadiblePlatformName() + "PackageNameInfo.txt";
			var interfaceMap = new InterfaceMap
			{
				interfaceMap = new Dictionary<string, string> { { "IVR", "NullVR" } },
				interfaceToMultipleMaps = new Dictionary<string, List<string>>(),
				namedIntTypes = new Dictionary<string, int>(),
				namedStringTypes = new Dictionary<string, string>()
			};

			var config = new BuildConfiguration
			{
				app_output_name = PlayerSettings.productName,
				buildOptions = BuildOptions.None,
				//buildOptions = Application.build
				editorUserBuildSettings = new SerializedEditorUserBuildSettings(),
				buildConfig = new SerializedPlayerSettings(),
				InterfaceMapping = interfaceMap,
				packagesToAdd = new List<string>()
			};
			config.editorUserBuildSettings.GetFromStaticInstance();

			config.buildConfig.GetFromStaticInstance();

			File.WriteAllText(subProjectsPath() + Path.DirectorySeparatorChar + configFileName, config.JsonSerialize());
		}

		public static string subProjectsPath()
		{
			//recurse backwards through the directories until you hit the subprojects path
			var startingPath = Path.GetFullPath(Application.dataPath);
			var path = startingPath;
			while(!string.IsNullOrEmpty(path) && path.Length > 5)
			{
				var speculativeSubprojectsDir = Path.Combine(path, "subProjects");
				//Debug.Log("Checking:"+speculativeSubprojectsDir);
				if(Directory.Exists(speculativeSubprojectsDir))
					return speculativeSubprojectsDir;

				path = Directory.GetParent(path).FullName;
			}
			throw new Exception("no subprojects path found");
		}

		public static void ImportPackage(string packageName)
		{
			string packagePath = subProjectsPath() + Path.DirectorySeparatorChar + packageName + ".unitypackage";
			Debug.Log("Package path is:"+packagePath);
			AssetDatabase.ImportPackage(packagePath, false);
		}

		[Serializable]
		public class BuildConfiguration
		{
			//for builder
			//maybe add the scenes to use explicitly?
			public string app_output_name; //ie MyApp or MyApp.apk depending on the platform

			public SerializedPlayerSettings buildConfig;
			public BuildOptions buildOptions;
			//http://docs.unity3d.com/ScriptReference/BuildOptions.html uncompressed, allow debugging, etc

			public SerializedEditorUserBuildSettings editorUserBuildSettings;
			public InterfaceMap InterfaceMapping;

			//for first pass installation of packages
			public List<string> packagesToAdd;
		}

		//so we can run external builds against multiple skus from a config file
		//from http://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.html
		[Serializable]
		public class SerializedEditorUserBuildSettings
		{
			public bool allowDebugging;
			public BuildTarget currentBuildTarget;
			public BuildTargetGroup currentBuildTargetGroup;
			//assuming EditorUserBuildSettings.selectedBuildTargetGroup gets set when we switch groups

			//public bool appendProject;  //is now BiuldOptions.AcceptExternalModificationsToPlayer
			public bool development;

			public bool explicitNullChecks;
#if UNITY_4_6
			public AndroidBuildSubtarget subtarget; //was: activeBuildTarget
#else
		public MobileTextureSubtarget subtarget; //was: activeBuildTarget
#endif
			public bool symlinkLibraries;

			public void SetStaticInstanceFromThis()
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(currentBuildTarget);
				EditorUserBuildSettings.allowDebugging = allowDebugging;
				EditorUserBuildSettings.androidBuildSubtarget = subtarget;
				//EditorUserBuildSettings.
				EditorUserBuildSettings.development = development;
				EditorUserBuildSettings.explicitNullChecks = explicitNullChecks;
				EditorUserBuildSettings.selectedBuildTargetGroup = currentBuildTargetGroup;
				EditorUserBuildSettings.symlinkLibraries = symlinkLibraries;
			}

			public void GetFromStaticInstance()
			{
				allowDebugging = EditorUserBuildSettings.allowDebugging;
				subtarget = EditorUserBuildSettings.androidBuildSubtarget;
				//NOTE: this will likely change, and not work for tizen,etc

				currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
				currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

				development = EditorUserBuildSettings.development;
				explicitNullChecks = EditorUserBuildSettings.explicitNullChecks;

				symlinkLibraries = EditorUserBuildSettings.symlinkLibraries;
			}
		}

		//TODO: Handheld -- this is where use32BitDisplayBuffer	,GetActivityIndicatorStyle, etc all work
		//getting,setting, saving PlayerSettings to allow for different SKUs http://docs.unity3d.com/ScriptReference/PlayerSettings.html
		//NOTE: this should probably be set after setting up the EidtorUserBuildSettings
		//TODO: things with no setters ie gpuskinning
		[Serializable]
		public class SerializedPlayerSettings
		{
			public string bundleIdentifier = "com.asink.unspecifiedBuild";

			//public string bundleVersion = "1.0";
			public string companyName = "asink";

			public UIOrientation defaultInterfaceOrientation = UIOrientation.AutoRotation;
			public bool defaultIsFullScreen;
			public int defaultScreenHeight;
			public int defaultScreenWidth;
			public ResolutionDialogSetting displayResolutionDialog;
			public string productName;
			public RenderingPath renderingPath;
			public List<string> scriptingDefineSymbols;
			public bool stripUnusedMeshComponents;
			public List<AspectRatio> supportedAspectRatios;
			public bool use32BitDisplayBuffer;
#if UNITY_4_6
			public bool useDirect3D11;
#endif
			public bool usePlayerLog;

			//public UIOrientation defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

			public void SetStaticInstanceFromThis(BuildTargetGroup buildTargetGroupIAMBuildingAgainst)
			{
				PlayerSettings.bundleIdentifier = bundleIdentifier;
				//PlayerSettings.bundleVersion = bundleVersion;
				PlayerSettings.companyName = companyName;
				PlayerSettings.defaultInterfaceOrientation = defaultInterfaceOrientation;
				PlayerSettings.defaultIsFullScreen = defaultIsFullScreen;
				PlayerSettings.defaultScreenHeight = defaultScreenHeight;
				PlayerSettings.defaultScreenWidth = defaultScreenWidth;
				PlayerSettings.displayResolutionDialog = displayResolutionDialog;

				foreach(AspectRatio ratio in Enum.GetValues(typeof(AspectRatio)))
				{
					if(supportedAspectRatios == null) break;
					PlayerSettings.SetAspectRatio(ratio, supportedAspectRatios.Contains(ratio));
				}
				PlayerSettings.productName = productName;
				PlayerSettings.renderingPath = renderingPath;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroupIAMBuildingAgainst,
					(scriptingDefineSymbols != null) ? scriptingDefineSymbols.ToArray().Join(",") : "");
				PlayerSettings.stripUnusedMeshComponents = stripUnusedMeshComponents;
				PlayerSettings.use32BitDisplayBuffer = use32BitDisplayBuffer;
				#if UNITY_4_6
				PlayerSettings.useDirect3D11 = useDirect3D11;
#endif
				PlayerSettings.usePlayerLog = usePlayerLog;
			}

			public void GetFromStaticInstance()
			{
				bundleIdentifier = PlayerSettings.bundleIdentifier;
				//bundleVersion = PlayerSettings.bundleVersion;
				//PlayerSettings.targetGlesGraphics;
				companyName = PlayerSettings.companyName;
				defaultInterfaceOrientation = PlayerSettings.defaultInterfaceOrientation;
				defaultIsFullScreen = PlayerSettings.defaultIsFullScreen;
				defaultScreenHeight = PlayerSettings.defaultScreenHeight;
				defaultScreenWidth = PlayerSettings.defaultScreenWidth;
				displayResolutionDialog = PlayerSettings.displayResolutionDialog;
				supportedAspectRatios = new List<AspectRatio>();
				foreach(AspectRatio ratio in Enum.GetValues(typeof(AspectRatio)))
				{
					if(supportedAspectRatios == null) break;
					if(PlayerSettings.HasAspectRatio(ratio))
						supportedAspectRatios.Add(ratio);
				}
				productName = PlayerSettings.productName;
				renderingPath = PlayerSettings.renderingPath;
				scriptingDefineSymbols = new List<string>();
				scriptingDefineSymbols.Add(
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
				stripUnusedMeshComponents = PlayerSettings.stripUnusedMeshComponents;
				use32BitDisplayBuffer = PlayerSettings.use32BitDisplayBuffer;
#if UNITY_4_6
				useDirect3D11 = PlayerSettings.useDirect3D11;
#endif
				usePlayerLog = PlayerSettings.usePlayerLog;
				//Write a log file with debugging information --reccomended to turn off in prod
			}
		}
	}
}