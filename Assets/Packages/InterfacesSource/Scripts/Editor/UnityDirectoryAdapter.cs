using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class UnityDirectoryAdapter
	{
		private readonly string baseProjectPath = Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "..");

		public string assetPathToFullPath(string assetBasedPathIn)
		{
			string[] directories = assetBasedPathIn.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
			var sb = new StringBuilder(baseProjectPath);
			sb.Append(Path.DirectorySeparatorChar);
			for(int i = 0; i < directories.Length; i++)
			{
				sb.Append(directories[i]);
				sb.Append(Path.DirectorySeparatorChar);
			}
			return sb.ToString();
		}

		public string fullPathToAssetsPath(string fullPathIn)
		{
			string fullPath = fullPathIn.Replace("\\", "/");
			string assetPath = FileUtil.GetProjectRelativePath(fullPath);

			//Debug.Log("Full path to assets path:" + fullPathIn + " assets path:" + assetPath + " base proj path:" + baseProjectPath);

			if(string.IsNullOrEmpty(assetPath) || fullPath.Contains(baseProjectPath) == false)
			{
				//Debug.LogError("Full path to assets path:" + fullPathIn + " assets path:"+ assetPath);
				throw new DirectoryNotInProjectPathException("fullPathIn not found in asset path:" + fullPathIn);
			}

			return assetPath;
		}

		public void createDirectory(string fullPath)
		{
			string pathClean = Path.GetFullPath(fullPath);
			if(!Directory.Exists(pathClean))
				Directory.CreateDirectory(pathClean);
		}

		public string cleanPath(string fullPath)
		{
			return Path.GetFullPath(fullPath);
		}

		public void deleteAllFilesInDirectory(string directory)
		{
			foreach(string file in Directory.GetFiles(directory))
			{
				File.Delete(file);
			}
		}

		public void deleteFileIfItExists(string fullPath)
		{
			if(File.Exists(fullPath))
				File.Delete(fullPath);
		}

		public void deleteFileInDirectoryIfItExists(string directory, string fileNameToDelete)
		{
			foreach(string file in Directory.GetFiles(directory))
			{
				if(file == fileNameToDelete)
					File.Delete(file);
			}
		}

		public List<string> readAllFiles(List<string> fileList)
		{
			var fileContents = new List<string>();
			foreach(string fileName in fileList)
			{
				fileContents.Add(File.ReadAllText(fileName));
			}
			return fileContents;
		}

		public List<string> getAllMatchingFilesRecursive(string directoryIn, List<string> blackList, string wildcardToLookFor)
		{
			var fileList = new List<string>();
			string directory = cleanPath(directoryIn);
			if(!Directory.Exists(directory))
			{
				return fileList;
			}

			string effectiveDir = directory;
			if(effectiveDir.EndsWith(Path.DirectorySeparatorChar + ""))
				effectiveDir = effectiveDir.Substring(0, effectiveDir.Length - 1);
			foreach(string file in Directory.GetFiles(effectiveDir, wildcardToLookFor))
			{
				fileList.Add(file);
			}

			//how to handle /plugins and /editor inside of folder paths?
			foreach(string dir in Directory.GetDirectories(directory))
			{
				if(isBlackListed(dir, blackList))
				{
#if DEBUG
					Debug.LogWarning("Blacklisted dir :" + dir);
#endif
					continue;
				}

				fileList.AddRange(getAllMatchingFilesRecursive(dir, blackList, wildcardToLookFor));
			}
			return fileList;
		}

		private bool isBlackListed(string fullPath, List<string> blackList)
		{
			string blackListedDirectoryPrefix = ".";
			//kludgy "regex" on the last directory to see if it starts with "." (right now)
			string[] subPaths = fullPath.Split(Path.DirectorySeparatorChar);
			if(subPaths[subPaths.Length - 1].StartsWith(blackListedDirectoryPrefix))
				return true;

			foreach(string blacklistedString in blackList)
			{
				string toIgnore = blacklistedString;
				if(blacklistedString.EndsWith("" + Path.DirectorySeparatorChar))
					toIgnore = toIgnore.Substring(0, toIgnore.Length - 1);
				//Debug.Log("THINK:" + blacklistedString + " vs:"+fullPath);
				if(fullPath.Contains(toIgnore))
					return true;
				//if(blacklistedString.Contains(fullPath))
				//	return true;
			}
			return false;
		}
	}

	public class DirectoryNotInProjectPathException : Exception
	{
		public DirectoryNotInProjectPathException()
		{
		}

		public DirectoryNotInProjectPathException(string description)
			: base(description)
		{
		}
	}
}