using UnityEngine;
using System.Collections;

public class fullscreen : MonoBehaviour {


	public void full() {
		if (!Screen.fullScreen) {
			Screen.fullScreen = true;
			Screen.SetResolution(Screen.width, Screen.height, true, 60);
				} else {
			Screen.fullScreen = false;
				}
	
	}
}
