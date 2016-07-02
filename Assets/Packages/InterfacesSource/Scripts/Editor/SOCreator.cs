using UnityEditor;
using UnityEngine;

namespace interfaces
{
	[CustomEditor(typeof(MonoScript))]
	public class SOCreator : Editor
	{
		public override void OnInspectorGUI()
		{
			var ms = target as MonoScript;
			var type = ms.GetClass();
			if(type != null && type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsSubclassOf(typeof(Editor)))
			{
				if(GUILayout.Button("CREATE INSTANCE"))
				{
					var asset = CreateInstance(type);
					var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + type.Name + ".asset");
					AssetDatabase.CreateAsset(asset, path);
					EditorGUIUtility.PingObject(asset);
				}
			}
			else
				DrawDefaultInspector();
		}

		public static T SOCreateInstance<T>() where T : ScriptableObject
		{
			var type = typeof(T);

			var asset = CreateInstance(type);

			AssetDatabase.CreateFolder("Assets", "Resources");
			var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + type.Name + ".asset");
			AssetDatabase.CreateAsset(asset, path);
			return asset as T;
		}
	}
}