using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace interfaces
{
	//TODO: delete all directories in one pass instead of multiple -- this would entail testing all deted files up to the project dir
	public class EmptyFolderKiller : AssetPostprocessor
	{
		//TODO: enable this after a certain period fo tiem in the editor, periodically checking,
		//and ignore the plugins folder entirely for now?
		//use a menu option for that, as it is a dangerous move
		public static bool enabled = false;

		[MenuItem("asink/util/turn folder killer on")]
		public static void FolderKillerEnabled()
		{
			enabled = true;
		}
		[MenuItem("asink/util/folder killer off")]
		public static void FolderKillerDisabled()
		{
			enabled = false;
		}
		private static readonly ILogger Debug = new NullDebugLog();
		private static readonly List<string> _toIgnore = new List<string> { ".DS_STORE", ".meta" };
		private static readonly TimeSpan AmountToWaitBeforeAutoDeletingEmptyDirectory = new TimeSpan(0, 0, 30, 0);
		private static readonly string FILES_TO_DELETE_PLAYERPREFS = "FILES_TO_DELETE_PLAYERPREFS";

		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
			string[] movedFromPath)
		{
			if(!enabled) return;

			var ToDelete = new List<string>();

			ToDelete.AddRange(checkEmptyFolders(importedAssets));
			var parentsOfDeletedObjects = new List<string>();
			foreach(string deletedAsset in deletedAssets)
			{
				parentsOfDeletedObjects.Add(Directory.GetParent(Path.GetFullPath(deletedAsset)).FullName);
			}
			ToDelete.AddRange(getFilesToDelete(parentsOfDeletedObjects));

			ToDelete.AddRange(getFilesToDelete(movedAssets));

			ToDelete.RemoveAll((string fileOrDir) => !isDirectory(fileOrDir));

			foreach(string directoryToDelete in ToDelete)
			{
				Debug.Log("DELETING empty dir:" + directoryToDelete);
				try
				{
					Directory.Delete(directoryToDelete, true);
				}
				catch(Exception e)
				{
					Debug.LogWarning("Could not delete directory:" + e);
				}
			}
		}

		private static List<string> checkEmptyFolders(string[] importedAssets)
		{
			var deleteImmediately = new List<string>();
			List<FolderTemporarilyWhitelisted> toCheckIfIShouldDelete = getTemporarilyWhitelistedFiles();
			var checkAgainLater = new List<FolderTemporarilyWhitelisted>();

			foreach(string justImported in importedAssets)
			{
				if(!isDirectory(justImported))
					continue;

				checkAgainLater.Add(new FolderTemporarilyWhitelisted
				{
					fullPath = justImported,
					ticksSerializedAt = DateTime.Now.Ticks
				});
			}
			var directoryAdapter = new UnityDirectoryAdapter();
			foreach(FolderTemporarilyWhitelisted folderTemporarilyWhitelisted in toCheckIfIShouldDelete)
			{
				DateTime timeToCutOff = (new DateTime(folderTemporarilyWhitelisted.ticksSerializedAt) +
										AmountToWaitBeforeAutoDeletingEmptyDirectory);
				string path = directoryAdapter.assetPathToFullPath(folderTemporarilyWhitelisted.fullPath);
				Debug.Log(string.Format("folder whitelisted:{2} DateTime.now:{0} vs timeToCutOff:{1}", DateTime.Now, timeToCutOff,
					folderTemporarilyWhitelisted.fullPath));
				if(DateTime.Now > timeToCutOff)
				{
					if(isEmptyDirectory(path))
						deleteImmediately.Add(path);
				}
				else
				{
					Debug.Log("Check again later for folder:" + folderTemporarilyWhitelisted);
					checkAgainLater.Add(folderTemporarilyWhitelisted);
				}
			}

			SaveTemporarilyWhitelistedFiles(checkAgainLater);

			Debug.Log("Folders to delete immediately:" + deleteImmediately.Join(","));
			return deleteImmediately;
		}

		private static List<FolderTemporarilyWhitelisted> getTemporarilyWhitelistedFiles()
		{
			var listToDelete = new List<FolderTemporarilyWhitelisted>();
			string serializedStringOfWhatToDelete = EditorPrefs.GetString(FILES_TO_DELETE_PLAYERPREFS, "");

			if(string.IsNullOrEmpty(serializedStringOfWhatToDelete)) return listToDelete;

			try
			{
				return
					JsonReader.Deserialize<ContainerForWhitelistedFolders>(serializedStringOfWhatToDelete).listOfWhitelistedFolders;
			}
			catch(Exception e)
			{
				Debug.LogError("Error desierializing what to delete:" + serializedStringOfWhatToDelete + " exception::" + e);
			}
			return new List<FolderTemporarilyWhitelisted>();
		}

		private static void SaveTemporarilyWhitelistedFiles(List<FolderTemporarilyWhitelisted> toSave)
		{
			if(toSave == null || toSave.Count == 0)
				EditorPrefs.SetString(FILES_TO_DELETE_PLAYERPREFS, "");
			else
			{
				string stringToSave = JsonWriter.Serialize(new ContainerForWhitelistedFolders { listOfWhitelistedFolders = toSave });
				EditorPrefs.SetString(FILES_TO_DELETE_PLAYERPREFS, stringToSave);
			}
		}

		private static bool isEmptyDirectory(string path)
		{
			if(!Directory.Exists(path))
				return false;

			var allSubFiles = new List<string>(Directory.GetFiles(path));
			//clean up list -- remove junk files like .DS_STORE
			allSubFiles.RemoveAll((string toRemove) => { return _toIgnore.Contains(Path.GetFileName(toRemove)); });
			//remove all directories, as those aren't "real" files
			allSubFiles.RemoveAll((string toRemove) => { return isDirectory(toRemove); });

			return allSubFiles.Count == 0;
		}

		private static List<string> getFilesToDelete(IEnumerable<string> paths)
		{
			var toDelete = new List<string>();

			foreach(string path in paths)
			{
				if(!File.Exists(path) && !Directory.Exists(path))
					continue;
				if(isEmptyDirectory(path))
					toDelete.Add(path);
			}

			return toDelete;
		}

		private static bool isFile(string path)
		{
			return (File.GetAttributes(path) & FileAttributes.Normal) == FileAttributes.Normal;
		}

		private static bool isDirectory(string path)
		{
			return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
		}

		//save when we last saw files and delete them after some reasonable timeout

		//[MenuItem("asink/ManualTest")]
		public static void manualTest()
		{
			AssetDatabase.CreateFolder("Assets", "Foobar");
			var adapter = new UnityDirectoryAdapter();
			string directoryLocation = adapter.assetPathToFullPath("Assets/Foobar");
			var things = new ContainerForWhitelistedFolders
			{
				listOfWhitelistedFolders =
					new List<FolderTemporarilyWhitelisted>
					{
						new FolderTemporarilyWhitelisted
						{
							fullPath = "Assets/Foobar",
							//ticksSerializedAt = DateTime.Now.Subtract(new TimeSpan(0,0,10)).Ticks,
							//ticksSerializedAt = DateTime.Now.Subtract(new TimeSpan(0,0,4,30)).Ticks,
							ticksSerializedAt = DateTime.Now.Ticks
						}
					}
			};
			SaveTemporarilyWhitelistedFiles(things.listOfWhitelistedFolders);

			List<string> toDelete = checkEmptyFolders(new[] { directoryLocation });
			Debug.Log("To delete:" + toDelete.Join(","));
		}

		[Serializable]
		public class ContainerForWhitelistedFolders
		{
			public List<FolderTemporarilyWhitelisted> listOfWhitelistedFolders;
		}

		[Serializable]
		public class FolderTemporarilyWhitelisted
		{
			public string fullPath;

			//public DateTime timeSerializedAt;
			public long ticksSerializedAt;
		}
	}
}