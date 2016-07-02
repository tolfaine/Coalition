#define DEBUG

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

///Applications/Unity/Unity.app/Contents//Frameworks/Mono/lib/mono/

namespace interfaces
{
	public class DLLMaker
	{
#if UNITY_EDITOR_WIN

		private static readonly string unityDLLDirectoryPath = Path.Combine(EditorApplication.applicationContentsPath,
			"Managed" + Path.DirectorySeparatorChar);//.Replace(" ","\\ ");

#else
	private static readonly string unityDLLDirectoryPath = Path.Combine(EditorApplication.applicationContentsPath,"Frameworks/Managed/");
#endif

		private static readonly string unityDLLPath = unityDLLDirectoryPath + "UnityEngine.dll";
		private static readonly string unityEditorDLLPath = unityDLLDirectoryPath + "UnityEditor.dll";

		public string dllRootOutputFolder
		{
			get
			{
				return Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "..") + Path.DirectorySeparatorChar + dllFolder + Path.DirectorySeparatorChar;
			}
		}

		// Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		public bool optimize;
		public List<string> blackList;
		public string dllFolder = "Dlls";

		public List<string> sourcePaths; //ie "Assets/Plugins/TouchKit/",

		public string buildTargetName { get; set; }

		public List<string> dllDependencies { get; set; }

		public bool isEditor { get; set; }

		//TODO: pull in the define helepr from the initila verison of this script... which has specific things like reflection_support
		//public List<string> defines = new List<string> { "REFLECTION_SUPPORT", "DYNAMIC_FONT", "UNITY_4_3" };
#if UNITY_5 
		public List<string> defines = new List<string> { "UNITY_5_0", "UNITY_5" }; //"UNITY_4_6",
#else
		public List<string> defines = new List<string> { "UNITY_4_6" };
#endif
		private readonly UnityDirectoryAdapter directory = new UnityDirectoryAdapter();

		public void cleanAllDLLs()
		{
			directory.deleteAllFilesInDirectory(dllFolder);
		}

		public void cleanAllDLLMatchingName(string name)
		{
			//string finalPath = finalDllOutputPath();
			string finalPathRoot = finalDLLOutputRootPath();
			directory.createDirectory(finalPathRoot);
			directory.deleteFileInDirectoryIfItExists(finalPathRoot, name);
		}

		public string finalDLLOutputRootPath()
		{
			return Path.GetDirectoryName(finalDllOutputPath());
		}

		public string finalDllOutputPath()
		{
			return Path.Combine(dllFolder, buildTargetName);
		}

		private void AddDllDependenciesToReferencedAssemblies(CompilerParameters compileParams)
		{
			if(dllDependencies != null)
			{
				foreach(string dllDependency in dllDependencies)
				{
					if(string.IsNullOrEmpty(dllDependency))
						continue;
					string dllDepPath = dllDependency;
					Debug.Log("Dependency " + dllDepPath + " while building name :" + finalDllOutputPath());
					if(!File.Exists(dllDepPath))
						Debug.LogError("Does not think that dll with path:" + dllDepPath + " exists");
					compileParams.ReferencedAssemblies.Add(dllDepPath);
				}
			}
		}

		private CompilerParameters getStandardCompilerParameters()
		{
			var compileParams = new CompilerParameters();
			compileParams.OutputAssembly = finalDllOutputPath();
			compileParams.CompilerOptions = "";
			if(optimize)
				compileParams.CompilerOptions += " /optimize";
			if(!File.Exists(unityDLLPath) || !File.Exists(unityEditorDLLPath))
				throw new Exception("asink: Error unity path dll path does not exist");

			compileParams.ReferencedAssemblies.Add(unityDLLPath);
			defines.RemoveAll((string matchToRemove) => { return string.IsNullOrEmpty(matchToRemove); });
			//make sure there are no empty entries, as it causes the compiler to have a warning

			if(isEditor)
			{
				defines.Add("UNITY_EDITOR");
				compileParams.ReferencedAssemblies.Add(unityEditorDLLPath);
				Debug.Log("Adding compile flag:" + unityEditorDLLPath);
			}
			if(defines.Count > 1)
				compileParams.CompilerOptions += " /define:" + string.Join(";", defines.ToArray());

			return compileParams;
		}

		public class NoFilesInDLLException : Exception
		{
		}

		public string createDLL()
		{
			if(blackList == null)
				blackList = new List<string>();

			var allSource = new List<string>();

			foreach(string buildFolder in sourcePaths)
			{
#if DEBUG
				Debug.Log("Looking at source path:" + buildFolder);
#endif
				allSource.AddRange(getAllSourceFromCSFilesInPathRecursive(buildFolder));
				var dllsInSource = getAllDllsInPathRecursive(buildFolder);
				if(dllDependencies == null) dllDependencies = new List<string>();
				foreach(var dll in dllsInSource)
				{
					if(!dllDependencies.Contains(dll))
						dllDependencies.Add(dll);
				}
			}

			CompilerParameters compileParams = getStandardCompilerParameters();
			AddDllDependenciesToReferencedAssemblies(compileParams);

			if(allSource.Count == 0)
			{
				throw new NoFilesInDLLException();
				//Debug.LogWarning("Specified build does not have any source files:" + buildTargetName);
				//return false;
			}

			Debug.Log("DEFINE: " + string.Join(",", defines.ToArray()));

			string directoryNameOfDLL = Path.GetDirectoryName(finalDllOutputPath());
			if(!Directory.Exists(directoryNameOfDLL))
				Directory.CreateDirectory(directoryNameOfDLL);

#if DEBUG
			var sb2 = new StringBuilder();
			allSource.ForEach((string toadd) => { sb2.Append(toadd + ","); });
			Debug.Log(sb2);
			Debug.Log("dll dependencies:" + string.Join(",", dllDependencies.ToArray()));
			//compileParams.ReferencedAssemblies.Clear();
			Debug.Log("referenced assemblies:" + string.Join(",", compileParams.ReferencedAssemblies.Cast<string>().ToArray()));
#endif

			var codeProvider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.0" } });

			CompilerResults compilerResults = codeProvider.CompileAssemblyFromFile(compileParams, allSource.ToArray());
			//bool hadRealError = false;
			if(compilerResults.Errors.Count > 0)
			{
				var sb = new StringBuilder();
				var warningSb = new StringBuilder();

				foreach(CompilerError error in compilerResults.Errors)
				{
					if(!error.IsWarning)
					{
						//hadRealError = true;
						sb.Append(error);
						Debug.LogError(error.ToString());
					}
					else
					{
						warningSb.Append(error);
						Debug.LogWarning("WARNING:" + error);
					}
				}
				if(sb.Length > 0)
					throw new Exception(sb.ToString());
			}

			Debug.Log("Compiled :" + finalDllOutputPath());
			//EditorUtility.DisplayDialog("DLL compile", buildTargetName + " should now be on your desktop. ", "OK");
			return finalDllOutputPath();
		}

		public List<string> getAllSourceFromCSFilesInPathRecursive(string pathIn)
		{
			string path = directory.cleanPath(pathIn);
			List<string> files = directory.getAllMatchingFilesRecursive(path, blackList, "*.cs");
			return files;
		}

		public List<string> getAllDllsInPathRecursive(string pathIn)
		{
			string path = directory.cleanPath(pathIn);
			List<string> dlls = directory.getAllMatchingFilesRecursive(path, blackList, "*.dll");
			//Debug.Log("ALL DLLS:" + dlls.JsonSerialize());
			return dlls;
		}
	}
}