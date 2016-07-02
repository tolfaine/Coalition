using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public int			damage = 100;
	public bool				isEnemy;
	public GameObject			explosion;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, 2);
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D collision){
		Debug.Log ("Touché Projectile");

		if(!isEnemy){
			if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") {
				GameObject g= Instantiate(explosion, this.transform.position, this.transform.rotation) as GameObject;
				Destroy (g, 1);
				Debug.Log ("Miam un enemy");
				collision.gameObject.BroadcastMessage ("TakeDamage",damage,SendMessageOptions.DontRequireReceiver);
				Destroy(gameObject);
			}
		}
	}
}
