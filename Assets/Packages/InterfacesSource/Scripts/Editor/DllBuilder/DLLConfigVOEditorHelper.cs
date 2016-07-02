using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public class DLLConfigVOEditorHelper
	{
		private AssetdatabaseFileAdapter db = new AssetdatabaseFileAdapter();
		public DLLConfigVO config;

		public string baseDir()
		{
			return db.directoryFromObjectReference(config.getBaseDirectory());
		}

		public List<string> getSourcePaths(DLLConfigVO.SubBuild subBuild)
		{
			List<string> sourcePaths = getDirectoriesToSearch(subBuild);

			if(sourcePaths.Count == 0)
			{
				return new List<string>() { baseDir() };
			}
			return sourcePaths;
		}

		public class DLLConfigVOComparer : IComparer<DLLConfigVO>
		{
			public int Compare(DLLConfigVO x, DLLConfigVO y)
			{
				if(x == null || y == null) throw new Exception("Trying to compare null dll config references!");
				if(x.getAllDependencies().Contains(y))
				{
					return 1;
				}
				if(y.getAllDependencies().Contains(x))
				{
					return -1;
				}
				return 1;//if neither, just say that we're greater than.. for kicks
			}
		}

		#region subbuild helpers

		public List<string> getDirectoriesToSearch(DLLConfigVO.SubBuild build)
		{
			return getListOfPathsFromListOfUnityObjects(build.getUnityObjectDirectoriesToSearch());
		}

		public List<string> getBlackListedDirectories(DLLConfigVO.SubBuild build)
		{
			var black = getListOfPathsFromListOfUnityObjects(build.getUnityObjectBlacklistedDirectories());
			foreach(string blacklist in black)
			{
				Debug.Log("BLACKLIST: " + blacklist);
			}
			return black;
		}

		private List<string> getListOfPathsFromListOfUnityObjects(List<Object> toFind)
		{
			List<string> directories = new List<string>();
			foreach(Object obj in toFind)
			{
				directories.Add(db.FullDirectoryFromObjectReference(obj));
			}
			return directories;
		}

		public List<string> getDLLsToCopy(DLLConfigVO.SubBuild subBuild)
		{
			var list = new List<string>();
#if UNITY_EDITOR
			foreach(Object dllObj in subBuild.getUnityObjectDllsToCopy())
			{
				string path = AssetDatabase.GetAssetPath(dllObj);
				Debug.Log("Object dll to copy :" + path);
				if(isDLL(path))
					list.Add(path);
			}
#endif
			return list;
		}

		private bool isDLL(string path)
		{
			if(File.Exists(path))
			{
				if(Path.GetFileName(path).Contains(".dll"))
					return true;
			}
			return false;
		}

		#endregion subbuild helpers
	}
}