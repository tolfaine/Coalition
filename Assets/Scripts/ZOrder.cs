using UnityEngine;
using System.Collections;

public class ZOrder : MonoBehaviour {

	private SpriteRenderer sprite;
	private Vector3 		position;
	public int order;
	public int 		tailleRoomY = 3;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer>();
		position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		position = transform.position;
		order = (int)(-position.y*100);
		sprite.sortingOrder = order;
	}
}
