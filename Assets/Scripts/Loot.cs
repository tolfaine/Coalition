using UnityEngine;
using System.Collections;

public class Loot : MonoBehaviour {

	public  RoomComponentList	roomComponentList;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		roomComponentList = GameObject.Find ("MapBuilder").GetComponent<RoomComponentList> ();
	}

	void OnDestroy ()
	{
		int rand = Random.Range(0,5);
		Debug.Log ("LOOT = " + rand);
		if (rand == 0) {
			Instantiate (roomComponentList.GetPickUp() , this.transform.position, this.transform.rotation);
		}
	}
}
