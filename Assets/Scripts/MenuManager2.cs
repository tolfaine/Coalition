﻿using UnityEngine;
using System.Collections;

public class MenuManager2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			GetComponent<Renderer> ().material.color = Color.red;
		}
	}
}
