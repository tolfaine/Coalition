using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public bool isEnemy;
	public int health;
	public int maxHealth;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(int damageCount)
	{
		health -= damageCount;
		Debug.Log ("hp =" + health);
		if (health <= 0) {
			if(isEnemy){
				if (this.transform.parent != null)
				{
					
					Destroy (this.transform.parent.gameObject);
				}
				else
				{
					Destroy (this.gameObject);
				}
			}
		}

	}

	void OnDestroy ()
	{
		Map.EnemyDie();
	}
/*
	public IEnumerator Damage(int damage){
		Debug.Log ("Taking damage");

		hp -= damage;
		Debug.Log ("hp =" + hp);
		if (hp <= 0)
		{
			Debug.Log ("bloublou");
			bool b = isEnemy;
			
			if (this.transform.parent != null)
			{
				Debug.Log ("destroy parent");
				Destroy(gameObject);
				Destroy (this.transform.parent.gameObject);
				
			}
			else{
				Debug.Log ("Avant wait");
				Destroy(gameObject);
			}
			Debug.Log ("Avant wait");
			yield return new WaitForSeconds (3f);
			Debug.Log ("Checking doooooors");
			Map.CheckDoors ();
			
		}
	}*/
	
	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// Is this a shot?
		Projectile projectile = otherCollider.gameObject.GetComponent<Projectile>();
		if (projectile != null)
		{
			// Avoid friendly fire
			if (projectile.isEnemy == isEnemy)
			{
				TakeDamage(projectile.damage);
				
				// Destroy the shot
				Destroy(projectile.gameObject); // Remember to always target the game object, otherwise you will just remove the script
			}
		}
	}

}
