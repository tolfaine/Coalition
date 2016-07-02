using UnityEngine;
using System.Collections;

public class MenuPrincipal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Quit () {
		Debug.Log ("click");
		Map map = GameObject.FindGameObjectsWithTag("MapBuilder")[0].GetComponent<Map>();
		map.mapBuilt = false;
		/*
		Save saving = new Save ();
		saving.toSave(map);
		BinSerialized.Save (saving);

		Debug.Log ("Saved");*/
		Time.timeScale = 1;
		Destroy(GameObject.FindGameObjectWithTag("MapBuilder"));
		Application.LoadLevel (0);
	}
	
}
