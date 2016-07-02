using UnityEngine;
using System.Collections;

public class NouvellePartie : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void newGame() {

		Map map = GameObject.FindGameObjectWithTag("MapBuilder").GetComponent<Map>();
		if(map == null)
			Debug.Log ("MAP NULLL");
		map.loaded = false;
		map.mapBuilt= false;
		Map.level = 0;
		Application.LoadLevel (1);
	}
}
