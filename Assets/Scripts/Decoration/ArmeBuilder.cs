using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmeBuilder : MonoBehaviour {

	public   List<GameObject>  guns;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject getGun(int i){
		return guns [i];
	}
}
