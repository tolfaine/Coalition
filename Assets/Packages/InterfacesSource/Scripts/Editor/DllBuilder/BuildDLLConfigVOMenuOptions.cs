using System.IO;
using UnityEditor;
using UnityEngine;

namespace interfaces
{
	public class BuildDLLConfigVOMenuOptions : Editor
	{
		[MenuItem("asink/dll/build currently selected VO")]
		public static void BuildVO()
		{
			var dll = Selection.activeObject as DLLConfigVO;
			BuildVO(dll);

			//makeDLLConfig(dll);
		}

		public static void BuildVO(DLLConfigVO dll)
		{
			if(dll == null)
			{
				Debug.Log("Selected object is not dll config");
				return;
			}
			var maker = new CreateDLLConfigVO { config = dll };
			maker.build();
		}
		//this seems to only work when I open up the project, then run the shell script. very odd
		[MenuItem("asink/dll/build all VOs")]
		public static void BuildAllDLLconfigs()
		{
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
			//var loader = new ResourceLoader();
			//var configs = loader.GetAllAssetsInFolder<DLLConfigVO>("DllConfigVO/");
			var configs = Resources.LoadAll<DLLConfigVO>("");
			Debug.Log("Found this many configs:" + configs.Length);
			foreach(var config in configs)
			{
				Debug.Log("building config:" + config.name + " dll name:"+ config.nameOfDll);
				Debug.Log("info :"+ config.JsonSerialize());
				BuildVO(config);
			}
			Debug.Log("Done with initial import");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.SaveAssets();
		}

		//TODO: start using this to make the output directory
		//needed for creating a new project that uses the dlls
		public static void copyAllDllSFromDirectoryWithSuffix(string directorySource, string directoryDestination,
			string suffix)
		{
			foreach(var path in Directory.GetFiles(directorySource, "*" + suffix + ".dll", SearchOption.AllDirectories))
			{
				var filename = Path.GetFileName(path);
				File.Copy(path, directoryDestination + Path.DirectorySeparatorChar + filename);
			}
		}
	}
}