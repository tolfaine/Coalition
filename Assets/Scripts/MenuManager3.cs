using UnityEngine;
using System.Collections;

public class MenuManager3 : MonoBehaviour {

	public GameObject CurrentMenu;
	public bool isPause = false;
	public Canvas pauseCanvas ;
	
	public void Start(){
		CurrentMenu.GetComponent<menu>().IsOpen = false;
		
	}
	
	void Update(){
		
		if( Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log ("pause");
			isPause = !isPause;
			if(isPause){
				ShowMenu(CurrentMenu);
				
				Time.timeScale = 0;
			}
			else{
				HideMenu(CurrentMenu);
				Time.timeScale = 1;
				
			}
		}
	}
	public void ShowMenu(GameObject menu){
		CurrentMenu.GetComponent<menu>().IsOpen = false;
		menu.GetComponent<menu>().IsOpen = true;
	}﻿
		
	public void HideMenu(GameObject menu){
		menu.GetComponent<menu>().IsOpen = false;
	}﻿

}
