using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public class DLLConfigVO : ScriptableObject
	{
#pragma warning disable 0649
		public string nameOfDll;
		public List<DLLConfigVO> listOfDependencies;
		public List<string> defines;
		public List<Object> dllObjects;

		public enum BuildType
		{
			Debug,
			Release,
		}

		public enum CompileOrderFlag
		{
			Scripts,
			Editor,
			Plugins,
			PluginEditor,
		}

		//some better way to handle the
		//public RuntimePlatform platformToBuildAgainst;

		public bool EditorBuild;

		[SerializeField]
		private Object BaseDirectory;

		public BuildType buildType;

		[ContextMenu("build dll")]
		private void buildDLL()
		{
			//build the dll, without linking this code directly to editor-only code paths by using reflection
			//idea thanks to 2014 schell talk
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				//var tempClass = assembly.GetType("BuildDLLConfigVOMenuOptions",false,true);//.Where(x => x.Namespace.ToUpper().Contains("MACRO"));
				IEnumerable<Type> tempClasses = assembly.GetTypes().Where(x => x.Name.ToUpper().Contains("BuildDLLConfigVOMenuOptions".ToUpper()));
				foreach(var tempClass in tempClasses)
				{
					tempClass.GetMethod("BuildVO", BindingFlags.Public | BindingFlags.Static, null,
						new[] { typeof(DLLConfigVO) }, new ParameterModifier[] { }).Invoke(null, new object[] { this });

					return;
				}
			}
			Debug.LogError("Didn't find method");
		}

		public string getDllOutputName(SubBuild build)
		{
			string directory = build.flags.ToString() + Path.DirectorySeparatorChar;
			if(build.flags == CompileOrderFlag.PluginEditor)
			{
				directory = CompileOrderFlag.Plugins.ToString() + Path.DirectorySeparatorChar;
				directory += "Plugins" + Path.DirectorySeparatorChar;
			}
			return directory + nameOfDll + "_" + build.flags + ".dll";
		}

		public Object getBaseDirectory()
		{
			return BaseDirectory;
		}

		public SubBuild pluginEditorBuild = new SubBuild { flags = CompileOrderFlag.PluginEditor };
		public SubBuild pluginBuild = new SubBuild { flags = CompileOrderFlag.Plugins };
		public SubBuild scriptsBuild = new SubBuild { flags = CompileOrderFlag.Scripts };
		public SubBuild editorBuild = new SubBuild { flags = CompileOrderFlag.Editor };

		public SubBuild getBuildFromType(CompileOrderFlag flag)
		{
			if(flag == CompileOrderFlag.Editor)
				return editorBuild;
			if(flag == CompileOrderFlag.Plugins)
				return pluginBuild;
			if(flag == CompileOrderFlag.Scripts)
				return scriptsBuild;
			if(flag == CompileOrderFlag.PluginEditor)
				return pluginEditorBuild;
			return null;
		}

		[Serializable]
		public class SubBuild
		{
			[SerializeField]
			private List<Object> DirectoriesToSearch;

			[SerializeField]
			private List<Object> blackListDirectories;

			[SerializeField]
			private List<Object> dllsToCopy;

			public bool enabled = true;
			public CompileOrderFlag flags;

			public List<Object> getUnityObjectDirectoriesToSearch()
			{
				return DirectoriesToSearch;
			}

			public List<Object> getUnityObjectBlacklistedDirectories()
			{
				return blackListDirectories;
			}

			public List<Object> getUnityObjectDllsToCopy()
			{
				return dllsToCopy;
			}
		}

		public List<DLLConfigVO> getAllDependencies()
		{
			var knownDependencies = new List<DLLConfigVO>();

			foreach(DLLConfigVO config in listOfDependencies)
			{
				if(knownDependencies.Contains(config))
					continue;

				knownDependencies.Add(config);
				getAllNewDependencies(knownDependencies);
			}
			return knownDependencies;
		}

		public void getAllNewDependencies(List<DLLConfigVO> knownDependencies)
		{
			foreach(DLLConfigVO config in listOfDependencies)
			{
				if(knownDependencies.Contains(config))
					continue;

				knownDependencies.Add(config);
				getAllNewDependencies(knownDependencies);
			}
		}

#pragma warning restore 0649
	}
}