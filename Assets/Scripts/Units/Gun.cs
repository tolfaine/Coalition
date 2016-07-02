using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	// shoot stats
	public int				damage ;
	public int			maxDamage ;
	public float 			shootDelay ;
	public float			maxShootDelay ;
	public float			projectileSpeed ;
	public int 				rafale ;

	private float			timer;
	private float			timerRafale;
	public float 			rafaleDelay ;
	public bool				canRafale;
	private bool			canShoot;

	public GameObject			projectile;
	public GameObject			unProjectile;
	public Transform 			projectileSpawn;
	public GameObject			explosion;

	public Animator 		animator;
	
	void CapStats ()
	{
		if (damage > maxDamage)
		{
			damage = maxDamage;
		}
		if (shootDelay < maxShootDelay)
		{
			shootDelay = maxShootDelay;
		}
	}
	// Use this for initialization
	void Start () {
		projectileSpawn = GameObject.FindGameObjectsWithTag("Player")[0].transform;
		shootDelay = 1f;
		rafaleDelay = 0.2f;
	}
	
	// Update is called once per frame
	void Update () {
		GetInputs();
		Timer ();
		TimerRafale ();
		UpdateAnimator ();
		CapStats();
	}

	void TimerRafale ()
	{	
		timerRafale += Time.deltaTime;
		canRafale = false;
		if (timerRafale > rafaleDelay)
		{
			canRafale= true;
		}
	}
	
	void UpdateAnimator(){

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
	
	void GetInputs ()
	{
		//-------------------
		// SHOOTING
		//-------------------
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Debug.Log("PEW");
			// shoot a projectile to the up
			//this.transform.rotation = Quaternion.Euler (0,270,0);
			animator.SetBool ("Walk_Dos", true);
			animator.SetBool ("Walk_Face", false);
			animator.SetBool ("Walk_Profile", false);

			Shoot(new Vector2(0 , projectileSpeed));
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			// shoot a projectile to the down
			//this.transform.rotation = Quaternion.Euler (0,90,0);
			animator.SetBool ("Walk_Dos", false);
			animator.SetBool ("Walk_Face", true);
			animator.SetBool ("Walk_Profile", false);
			Shoot(new Vector2(0 , -projectileSpeed));
			
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			animator.SetBool ("Walk_Dos", false);
			animator.SetBool ("Walk_Face", false);
			animator.SetBool ("Walk_Profile", true);
			// shoot a projectile to the left
			
			Shoot(new Vector2(-projectileSpeed,0));
			
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			// shoot a projectile to the right
			//this.transform.rotation = Quaternion.Euler (0,0,0);
			animator.SetBool ("Walk_Dos", false);
			animator.SetBool ("Walk_Face", false);
			animator.SetBool ("Walk_Profile", true);

			Shoot(new Vector2(projectileSpeed, 0));
			
		}
	}

	public bool notShooting(){
		if(!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow)){
			return true;
		}
		else{
			return false;
		}
	}


	void Shoot(Vector2 v)
	{
		if (canShoot) 
		{
			timer = 0;

			int r = rafale;

			while ( r > 0){
				if(canRafale){


					timerRafale = 0;
					unProjectile = Instantiate(projectile,projectileSpawn.position,this.transform.rotation) as GameObject;
					unProjectile.rigidbody2D.velocity = v;

					Quaternion theRotation = transform.localRotation;

					if(v.x > 0 ){
						theRotation= Quaternion.Euler (0,0, 0);
					}
					if(v.x < 0 ){
						theRotation= Quaternion.Euler (0,0,180);
					}
					if(v.y < 0 ){
						theRotation= Quaternion.Euler (0,0,270);
						
					}
					if(v.y > 0 ){
						theRotation= Quaternion.Euler (0,0,90);
					}
					
					unProjectile.transform.rotation =  theRotation;
					Debug.Log("Rafale =" + r);
					unProjectile.GetComponent<Projectile>().damage = damage;
					unProjectile.GetComponent<Projectile>().isEnemy = false;
					unProjectile.GetComponent<Projectile>().explosion = explosion;
					r--;
					canRafale = false;
				}
			}
			canShoot = false;
		}

	}
	
}
