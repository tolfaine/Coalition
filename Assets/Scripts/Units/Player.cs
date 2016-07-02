using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// loacal stats
	public float			speed ;
	public float			maxSpeed ;
	public bool facingRight = false;
	
	// external scripts
	public Health			playerHealth;
	public Gun				playerGun;
	private Animator 		animator;


	void Start ()
	{
		playerGun = this.GetComponentInChildren<Gun>();
		playerHealth = this.GetComponent<Health>();
		animator = GetComponent<Animator>();
		playerGun.animator = animator;
	//Map map = GameObject.FindGameObjectWithTag("MapBuilder").GetComponent<Map>();


		SetStats ();
	}
	
	public void SetStats ()
	{
		CapStats();
	}
	
	void CapStats ()
	{
		if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
	}

	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		if (Mathf.Abs (h) > 0.2 || Mathf.Abs (v) > 0.2) {
			if (Mathf.Abs (h) < Mathf.Abs (v)) {
				if(playerGun.notShooting()){
					if( v > 0){
						animator.SetBool ("Walk_Dos", true);
						animator.SetBool ("Walk_Face", false);
						animator.SetBool ("Walk_Profile", false);
					}
					else{
						animator.SetBool ("Walk_Face", true);
						animator.SetBool ("Walk_Dos", false);
						animator.SetBool ("Walk_Profile", false);
					}
				}
				else{

				}

			} 
			else {
				animator.SetBool ("Walk_Profile", true);
				animator.SetBool ("Walk_Dos", false);
				animator.SetBool ("Walk_Face", false);
			}
	} 
		else {
			animator.SetBool ("Walk_Face", false);
			animator.SetBool ("Walk_Profile", false);
			animator.SetBool ("Walk_Dos", false);

		}

		rigidbody2D.velocity = new Vector2(h * speed, v * speed);
		if(h < 0 && !facingRight)
			Flip();
		else if(h > 0 && facingRight)
			Flip();

		if (Input.GetKey (KeyCode.LeftArrow) && !facingRight ) {
			Flip();
		}

		if (Input.GetKey (KeyCode.RightArrow) && facingRight ) {
			Flip();
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
