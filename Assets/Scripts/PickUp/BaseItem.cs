using UnityEngine;
using System.Collections;

public class BaseItem : MonoBehaviour{

	public enum 	ItemTypes{
		UPGRADE,
		POTION,
	}

	public enum 	PickUpType
	{
		plusFrequenceArme,
		plusRafaleArme,
		plusVitesseProjectile,
		plusDegatProjectile,
		plusZoneProjectile,
		plusPerforationProjectile, 
		plusFrostProjectile, 
		plusVieArmure, 
		plusReductionDegatArmure, 
		plusVitesseDeplaArmure,
		plusAll
	}

	public string itemName ;
	public string itemDescription;
	public int itemId;
	public ItemTypes itemType;
	public PickUpType pickUpType;
	public  Map script;

	public string ItemName{
		get{ return itemName;}
		set{ itemName = value;}
	}

	public string ItemDescription{
		get{ return itemDescription;}
		set{ itemDescription = value;}
	}
	public int ItemId{
		get{ return itemId;}
		set{ itemId = value;}
	}
	public ItemTypes ItemType{
		get{ return itemType;}
		set{ itemType = value;}
	}

	void Start ()
	{
		script = (GameObject.Find("MapBuilder") as GameObject).GetComponent<Map>();
	}


	void OnCollisionEnter2D (Collision2D col)
	{
		Debug.Log ("LOOT Colision");
		if (col.collider.tag == "Player")
		{
			Player player = col.gameObject.GetComponent<Player>();

			switch (pickUpType)
			{
			case PickUpType.plusFrequenceArme:
				script.pickupInfo = "Frequence d'arme augmentée";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponentInChildren<Gun>().shootDelay -= 0.2f;
				}
				break;
			case PickUpType.plusRafaleArme:
				script.pickupInfo = "Rafale d'arme augmentée";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponentInChildren<Gun>().rafale += 1;
				}
				break;
			case PickUpType.plusVitesseProjectile:
				script.pickupInfo = "Vitesse Projectile augmetée!";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponentInChildren<Gun>().projectileSpeed += 0.5f;
				}
				break;
			case PickUpType.plusDegatProjectile:
				script.pickupInfo = "Degat de zone augmenté";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponentInChildren<Gun>().damage+= 50;
				}
				break;
			case PickUpType.plusZoneProjectile:
				script.pickupInfo = "Zone augmentéee";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					
				}
				break;

			case PickUpType.plusFrostProjectile :
				script.pickupInfo = "Frost augmenté" ;
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					
				}
				break;
			case PickUpType.plusVieArmure  :
				script.pickupInfo = "Vie augmentéee";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponent<Health>().maxHealth+= 50;
					col.gameObject.GetComponent<Health>().health+= 50;

				}
				break;
			case PickUpType.plusReductionDegatArmure  :
				script.pickupInfo = "Reduction degat augmentéee" ;
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					
				}
				break;
			case PickUpType.plusVitesseDeplaArmure :
				script.pickupInfo = "Vitesse de deplacement augmentéee";
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					col.gameObject.GetComponent<Player>().speed+= 0.5f;
				}
				break;
			case PickUpType.plusAll :
				script.pickupInfo = "Tout augmentéee" ;
				script.SendMessage ("PickUpInfo");
				if(itemType == ItemTypes.POTION){
				}
				else{
					
				}
				break;
			}
			Destroy (this.gameObject);
		}

	}


}
