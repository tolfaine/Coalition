using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {

	public Vector2 room;
	public bool activated = false;
	void OnTriggerEnter2D (Collider2D collision)
	{
		if (activated == true) {
		} else {
			if (collision.tag == "Player") {
				activated = true;
				Debug.Log ("Le joueur veut changer de niveau !");

				Map.level++;
				Debug.Log ("level MAPP = " + Map.level);
				if (Map.level < 2) {
					Player player = collision.GetComponent<Player> ();
					Gun playerGun = collision.GetComponentInChildren<Gun> ();
					Health playerHealth = collision.GetComponent<Health> ();
					
					Map.playerCurrentDamage = playerGun.damage;
					Map.playerCurrentHealth = playerHealth.health;
					Map.playerCurrentMaxHealth = playerHealth.maxHealth;
					Map.playerCurrentShotDelay = playerGun.shootDelay;
					Map.playerCurrentSpeed = player.speed;
					Map.playerCurrentProjectileSpeed = playerGun.projectileSpeed;	

					Debug.Log ("Load level 2!");

					Application.LoadLevel (1);
					GameObject.FindGameObjectWithTag("MapBuilder").GetComponent<Map>().BuildDungeon();
				} else {
					Destroy(GameObject.FindGameObjectWithTag("MapBuilder"));
					Map.level = 0;
					Application.LoadLevel (0);
				}
			}
		}
	}
}
