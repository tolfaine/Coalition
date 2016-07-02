using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	public enum 	EnumDir{
		NORD,
		EST,
		SUD,
		OUEST
	}

	public bool open;
	private Map	mapScript;
	public EnumDir typeDir;
	public int  room;
	private Animator 		animator;

	void Open(int currentRoom){
		if (currentRoom == room) {
			animator.SetBool ("open", true);
			animator.SetBool ("close", false);
			open = true;
				}
	}

	void Close(int currentRoom){
		//Debug.Log ("Demande de fermeture!");
		if (currentRoom == room) {
			animator.SetBool ("open", false);
			animator.SetBool ("close", true);
			open = false;
		}
	}
	void Awake()
	{
		mapScript = FindSceneObjectsOfType(typeof (Map))[0] as Map;
		animator = GetComponent<Animator>();
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		//Debug.Log ("collider!");
		if (open) {
			Debug.Log ("open");
			if (collision.gameObject.tag == "Player") {
				Debug.Log ("Player!");
				mapScript.newDir = typeDir;
					mapScript.ChangeRooms ();
			}
		}
	}
}
