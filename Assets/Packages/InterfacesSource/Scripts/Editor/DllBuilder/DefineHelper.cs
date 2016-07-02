using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace interfaces
{
	public class DefineHelper
	{
		//read a .rsp file which has a list of custom defines for the project.
		//NOTE: untested.  not sure how this file is actually suppsed to look
		//http://docs.unity3d.com/Manual/PlatformDependentCompilation.html
		private static List<string> getCustomDefinesFromRSP(string file)
		{
			List<string> finalList = new List<string>();
			UnityDirectoryAdapter otherDir = new UnityDirectoryAdapter();
			string fullPath = otherDir.assetPathToFullPath(file);
			if(!File.Exists(fullPath)) return finalList;

			string rawDefines = File.ReadAllText(fullPath);

			rawDefines = rawDefines.Replace("-define:", "");
			finalList.AddRange(rawDefines.Split(new char[] { ';' }));
			return finalList;
		}

		//Assumes that you've switched the build target to the appropriate platform, outputs the header files
		//TODO: get the defines for our *target* not just the current platform
		public static List<string> getBuildFlags(bool isEditor, BuildTargetGroup buildTargetGroup)//EditorUserBuildSettings.selectedBuildTargetGroup
		{
			List<string> customDefines = new List<string>();
			string rawCustomImports =
				PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			customDefines.AddRange(rawCustomImports.Split(new char[] { ';' }));

			customDefines.AddRange(getCustomDefinesFromRSP("Assets/gmcs.rsp"));

			if(isEditor)
			{
				customDefines.AddRange(getCustomDefinesFromRSP("Assets/smcs.rsp"));

				customDefines.AddRange(new List<string>()
				{
				"UNITY_EDITOR",
#if UNITY_EDITOR_WIN
				"UNITY_EDITOR_WIN",
#endif
#if UNITY_EDITOR_OSX
				"UNITY_EDITOR_OSX",
#endif
				});
			}

			var list = new List<string>(customDefines)
			{
#if UNITY_STANDALONE
				"UNITY_STANDALONE",
#endif
#if UNITY_4_5
				"UNITY_4_5",
#endif
#if UNITY_4_5_2
				"UNITY_4_5_2",
#endif
#if WINDOWS_STORE
				"WINDOWS_STORE",
#endif
#if UNITY3D || true
				"UNITY3D",
#endif
#if UNITY_IPHONE
			"UNITY_IPHONE",
#endif
#if UNITY_WINRT
				"UNITY_WINRT",
#endif
#if UNITY_METRO
				"UNITY_METRO",
#endif
#if UNITY_WP8
				"UNITY_WP8",
#endif
#if UNITY_ANDROID
				"UNITY_ANDROID",
#endif
#if UNITY_STANDALONE
				"UNITY_STANDALONE",
#endif
#if UNITY_STANDALONE_WIN
				"UNITY_STANDALONE_WIN",
#endif
#if UNITY_STANDALONE_MAC
				"UNITY_STANDALONE_MAC",
#endif
#if UNITY_STANDALONE_LINUX
				"UNITY_STANDALONE_LINUX",
#endif
			};
			return list;
		}
	}
}