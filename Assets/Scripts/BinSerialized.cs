using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BinSerialized : MonoBehaviour {

	private static string filePath;


	public static void Save( object obj){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/Save.sv");
		bf.Serialize (file, obj);
		file.Close();
	}

	public static Save Load() {
		if(File.Exists(Application.persistentDataPath + "/Save.sv")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			Save saving = (Save)bf.Deserialize(file);
			file.Close();
			return saving ;
		}
		return null;
	}

}
