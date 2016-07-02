using UnityEngine;
using System.Collections;

public class IA_2 : MonoBehaviour {

	private Transform player;
	private Vector3	PlayerDirection;
	private float Xdif;
	private float Ydif;
	public float speed;
	public float rotationDamping;
	public float 			shootDelay = 1f;
	private Animator 		animator;
	
	private float			timer;
	private bool			canShoot;
	public int 				damage;
	public bool facingRight = false;

	public bool canChase = true;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
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
		if (canChase) {
						if (Xdif > 0.1 || Ydif > 0.1) {
								if (Mathf.Abs (Xdif) < Mathf.Abs (Ydif)) {

										if (Ydif > 0) {
												animator.SetBool ("Walk_Dos", true);
												animator.SetBool ("Walk_Face", false);
												animator.SetBool ("Walk_Profile", false);
												animator.SetBool ("Attack", false);
										} else {
												animator.SetBool ("Walk_Face", true);
												animator.SetBool ("Walk_Dos", false);
												animator.SetBool ("Walk_Profile", false);
												animator.SetBool ("Attack", false);
										}

								} else {
										animator.SetBool ("Walk_Profile", true);
										animator.SetBool ("Walk_Dos", false);
										animator.SetBool ("Walk_Face", false);
										animator.SetBool ("Attack", false);
								}
						}

						if ((transform.position.x - player.position.x) > 0 && !facingRight) {
								Flip ();
						}
		
						if ((transform.position.x - player.position.x) < 0 && facingRight) {
								Flip ();
						}


						transform.Translate ((PlayerDirection.x * 4) * speed * Time.deltaTime, (PlayerDirection.y * 4) * speed * Time.deltaTime, 0);
						//transform.Translate(new Vector3(speed* Time.deltaTime,0,0) );
				}
	}
	
	void OnTriggerEnter2D (Collider2D collision){
		//Debug.Log ("Touché");
		if (collision.gameObject.tag == "Player") {
			//Debug.Log ("Miam un joueur");
			if (canShoot) 
			{
				//Debug.Log ("Je peux tirerrr");
				animator.SetBool ("Attack", true);
				collision.gameObject.BroadcastMessage ("TakeDamage",damage,SendMessageOptions.DontRequireReceiver);
				timer = 0;
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

	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
