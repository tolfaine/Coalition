using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class CreateDLLConfigVO
	{
		public DLLConfigVO config;
		private UnityDirectoryAdapter directory = new UnityDirectoryAdapter();

		public void build()
		{
			DLLConfigVO dll = config;
			if(dll == null)
			{
				Debug.Log("Selected object is not dll config");
				return;
			}

			printAllSubDependencies();

			makeDLLConfig(config);
		}

		private void printAllSubDependencies()
		{
			var alldependencies = config.getAllDependencies();

			alldependencies.Sort(new DLLConfigVOEditorHelper.DLLConfigVOComparer());
			StringBuilder sb = new StringBuilder();
			foreach(DLLConfigVO subDep in alldependencies)
			{
				sb.Append(subDep.nameOfDll + ",");
			}
			Debug.Log("All dependencies:" + sb);
		}

		private static int maxCalls = 100;
		private static int currentCalls = 0;

		private void makeDLLConfig(DLLConfigVO configTarget)
		{
			var dllMaker = new DLLMaker();

			foreach(DLLConfigVO subConfig in configTarget.getAllDependencies())
			{
				currentCalls++;
				if(currentCalls > maxCalls)
					throw new Exception("Test shunt to prevent uneeded recursion");
				Debug.Log("Trying to make subconfig:" + subConfig.name);
				makeDLLConfig(subConfig);
			}
			Debug.Log("Starting to build config:" + configTarget.nameOfDll);

			//TODO: handle dll dependencies at the lead builld layer
			string plugin = doSubBuild(configTarget, configTarget.pluginBuild, dllMaker, new List<string>());
			Debug.Log("Done with plugins");
			string scriptsBuild = doSubBuild(configTarget, configTarget.scriptsBuild, dllMaker, new List<string>() { plugin });
			Debug.Log("Done with scripts");
			doSubBuild(configTarget, configTarget.editorBuild, dllMaker, new List<string>() { plugin, scriptsBuild });
			Debug.Log("Done with editor");
		}

		//TODO: other dlls to link against
		private string doSubBuild(DLLConfigVO configToMake, DLLConfigVO.SubBuild subBuild, DLLMaker maker, List<string> subDlls)
		{
			if(!subBuild.enabled)
				return "";

			DLLConfigVOEditorHelper helper = new DLLConfigVOEditorHelper() { config = configToMake };

			string finalDLLName = configToMake.getDllOutputName(subBuild);
			//string basePath = Path.GetDirectoryName(finalDLLName);
			//Debug.LogError(basePath + Path.DirectorySeparatorChar);
			//directory.createDirectory(basePath + Path.DirectorySeparatorChar);

			if(subBuild.enabled == false) return null;

			maker.buildTargetName = finalDLLName;
			maker.blackList = helper.getBlackListedDirectories(subBuild);
			maker.sourcePaths = helper.getSourcePaths(subBuild);
			maker.isEditor = (subBuild.flags == DLLConfigVO.CompileOrderFlag.Editor);

			maker.cleanAllDLLMatchingName(finalDLLName);

			//TODO link dependencies!  // link them recursively with references to other DLLConfigVOs?
			List<string> partialDllPaths = helper.getDLLsToCopy(subBuild);
			for(int i = 0; i < partialDllPaths.Count; i++)
			{
				partialDllPaths[i] = Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "..") + Path.DirectorySeparatorChar + partialDllPaths[i];
			}
			maker.dllDependencies = partialDllPaths;
			foreach(var dllObject in config.dllObjects)
			{
				var dllPath = AssetDatabase.GetAssetPath(dllObject);
				Debug.Log("Adding dll :" + dllPath);
				maker.dllDependencies.Add(dllPath);
			}
			foreach(var subDll in subDlls)
			{
				Debug.Log("Adding sub dll :" + subDll);
				maker.dllDependencies.Add(subDll);
			}
			maker.defines.AddRange(configToMake.defines);

			foreach(DLLConfigVO subconfig in configToMake.getAllDependencies())
			{
				if(subconfig == configToMake)
					continue;
				//Debug.Log("Building this type of build:"+subBuild.flags);

				//for now... just add all flags
				foreach(DLLConfigVO.CompileOrderFlag flag in Enum.GetValues(typeof(DLLConfigVO.CompileOrderFlag)))
				{
					if(subBuild.flags == DLLConfigVO.CompileOrderFlag.Scripts)
					{
						if(flag == DLLConfigVO.CompileOrderFlag.Editor)
							continue;
					}

					DLLConfigVO.SubBuild build = subconfig.getBuildFromType(flag); //todo: most plugins require scripts, etc
					if(build.enabled == false)
						continue;
					//Probably need to include the plugins if it's scripts?
					string dllOutput = subconfig.getDllOutputName(build);
					Debug.LogWarning("Adding sub dependency dll:" + dllOutput);

					maker.dllDependencies.Add(maker.dllRootOutputFolder + dllOutput);
				}
			}
			//maker.dllDependencies.AddRange();
			if(subBuild.flags == DLLConfigVO.CompileOrderFlag.Editor)
			{
				string path = maker.finalDLLOutputRootPath();
				List<string> dlls = directory.getAllMatchingFilesRecursive(path, new List<string>(), "*" + configToMake.nameOfDll + "*.dll");
				foreach(var dll in dlls)
				{
					Debug.Log("test dll:" + dll);
					if(dll.ToLower().Contains("scripts"))
					{
						Debug.LogError("SHOULD BE ADDING:" + dll);
					}
				}
				//string newPath = maker.finalDLLOutputRootPath() + Path.DirectorySeparatorChar
			}

			//right now the creation also copies it to ../dlls
			string newDll = maker.createDLL();

			string dependentDLLDirectoryLocation = configToMake.getDllOutputName(subBuild);// DLLMaker.outputFolder + subBuild.flags;

			directory.createDirectory(dependentDLLDirectoryLocation);
			foreach(string dependentDll in helper.getDLLsToCopy(subBuild))
			{
				string filename = Path.GetFileName(dependentDll);
				string newPath = maker.finalDLLOutputRootPath() + Path.DirectorySeparatorChar + filename;
				//Debug.Log("Trying to copy from:" + filename + " to newpath:(" + newPath + ") dependentdll : " + dependentDll + " final output name:" + maker.finalDLLOutputRootPath());
				if(File.Exists(newPath))
					File.Delete(newPath);

				File.Copy(dependentDll, newPath);
			}
			return newDll;
		}
	}
}