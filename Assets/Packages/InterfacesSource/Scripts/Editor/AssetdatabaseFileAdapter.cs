using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public class AssetdatabaseFileAdapter
	{
		private readonly UnityDirectoryAdapter otherDir = new UnityDirectoryAdapter();

		public string directoryFromObjectReference(Object obj)
		{
			string directoryPath = AssetDatabase.GetAssetPath(obj);
			string fullDirectoryPath = otherDir.assetPathToFullPath(directoryPath) + Path.DirectorySeparatorChar;

			if(string.IsNullOrEmpty(directoryPath) || !Directory.Exists(fullDirectoryPath))
			{
				throw new InvalidDataException("asset reference is is not a directory assetpath:" + directoryPath +
											   " and full path:" + fullDirectoryPath);
			}
			return directoryPath;
		}

		public string FullDirectoryFromObjectReference(Object obj)
		{
			string directoryPath = AssetDatabase.GetAssetPath(obj);
			string fullDirectoryPath = otherDir.assetPathToFullPath(directoryPath) + Path.DirectorySeparatorChar;

			if(string.IsNullOrEmpty(directoryPath) || !Directory.Exists(fullDirectoryPath))
			{
				throw new InvalidDataException("asset reference is is not a directory assetpath:" + directoryPath +
											   " and full path:" + fullDirectoryPath);
			}
			return otherDir.assetPathToFullPath(directoryPath);
		}

		public void makeSureDirectoryExists(string path)
		{
			string projectPath =
				Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar);

			path = path.Replace("\\", "/");
			string[] pathParts = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
			var pathBuilder = new StringBuilder();
			for(int i = 0; i < pathParts.Length; i++)
			{
				if(!Directory.Exists(projectPath + pathBuilder + Path.DirectorySeparatorChar + pathParts[i]))
				{
					AssetDatabase.CreateFolder(pathBuilder.ToString(), pathParts[i]);
				}

				Debug.Log("Building path:" + pathBuilder + " on top of dir:" + pathParts[i]);
				if(i != 0)
					pathBuilder.Append('/');
				pathBuilder.Append(pathParts[i]);

				//AssetDatabase.WriteImportSettingsIfDirty(pathBuilder.ToString());
			}
		}
	}
}