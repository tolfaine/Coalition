using UnityEngine;
using System.Collections;

public class Coffre : MonoBehaviour {
	public RoomComponentList script;

	// Use this for initialization
	void Start () {
		script = (GameObject.Find("MapBuilder") as GameObject).GetComponent<RoomComponentList>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		Debug.Log ("LOOT Colision");
		if (col.collider.tag == "Player") {
			Instantiate( script.GetPickUp(), this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
}
