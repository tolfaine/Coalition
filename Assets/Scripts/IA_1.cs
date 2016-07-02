using UnityEngine;
using System.Collections;

public class IA_1 : MonoBehaviour {

	private Transform player;
	private Vector3	PlayerDirection;
	private float Xdif;
	private float Ydif;
	public float speed;
	public float rotationDamping;
	public float 			shootDelay = 0.5f;

	private float			timer;
	private bool			canShoot;
	public int 				damage;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Timer ();
		player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
		lookAtPlayer ();
		chase ();
	}

	void lookAtPlayer ()
	{
		Xdif = player.position.x - transform.position.x;
		Ydif = player.position.y - transform.position.y;

		PlayerDirection = new Vector3 (Xdif, Ydif, 0);
		//transform.LookAt(player.position);
		//transform.Rotate(new Vector3(0,-90,0),Space.Self);
	}

	void chase ()
	{
		transform.Translate ((PlayerDirection.x*4) * speed * Time.deltaTime, (PlayerDirection.y*4) * speed * Time.deltaTime, 0);
		//transform.Translate(new Vector3(speed* Time.deltaTime,0,0) );
	}

	void OnTriggerEnter2D (Collider2D collision){
		//Debug.Log ("Touché");
		if (collision.gameObject.tag == "Player") {
			//Debug.Log ("Miam un joueur");
			if (canShoot) 
			{
				//Debug.Log ("Je peux tirerrr");
				collision.gameObject.BroadcastMessage ("TakeDamage",damage,SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void Timer ()
	{
		timer += Time.deltaTime;
		canShoot = false;
		if (timer > shootDelay)
		{
			canShoot = true;
		}
	}
}
