using UnityEngine;
using System.Collections;

public class RoomBuilderScript : MonoBehaviour 
{
	private Transform[] 	roomObjLocs;
	public 	GameObject 		roomEnemyType;
	public 	GameObject 		roomRockType;
	public 	GameObject 		endGame;
	private GameObject	newObj;
	public int level ;
	
	public 	bool		isBossRoom;
	public	bool		isBossDead;
	private bool		isEnemysSpawned;
	private bool		isTriggerSpawned = false;

	public  RoomComponentList	roomComponentList;

	
	public void RoomLayout()
	{
		roomComponentList = GameObject.Find ("MapBuilder").GetComponent<RoomComponentList> ();

		roomObjLocs = this.gameObject.GetComponentsInChildren <Transform>() as Transform[];
		
		// if the current room has stuff in it then check what that stuff is
		if (roomObjLocs != null)
		{
			foreach (Transform objLoc in roomObjLocs)
			{
				// if its rock the spawn rocks 
				if (objLoc.gameObject.name == "Rock")
				{
					Object g = roomComponentList.GetRocks(level);

					if(g == null){
						Debug.Log("Nullllllllllll");
					}
					Instantiate (roomComponentList.GetRocks(level),objLoc.position, objLoc.rotation);
				}
				if (objLoc.gameObject.name == "PickUp")
				{
					Instantiate (roomComponentList.GetCoffre(),objLoc.position, objLoc.rotation);
				}
			}
		}
	}
	
	public void SpawnEnemys ()
	{
		roomObjLocs = this.gameObject.GetComponentsInChildren <Transform>() as Transform[];
		
		// if the current room has stuff in it then check what that stuff is
		if (roomObjLocs != null)
		{
			if (!isEnemysSpawned)
			{
				if(isBossRoom){
					roomEnemyType = roomComponentList.GetBoss(level) as GameObject ;
				}
				else{
					roomEnemyType = roomComponentList.GetEnemy(level) as GameObject;
				}

				foreach (Transform objLoc in roomObjLocs)
				{
					// if its an emeny then spawn an enemy
					if (objLoc.gameObject.name == "Enemy")
					{
						newObj = Instantiate (roomEnemyType,objLoc.position, objLoc.rotation) as GameObject;
						newObj.transform.parent = objLoc;
					}
					// if its an boss then spawn an boss
					if (objLoc.gameObject.name == "Boss")
					{
						roomEnemyType = roomComponentList.GetBoss(level) as GameObject;
						newObj = Instantiate (roomEnemyType,objLoc.position, objLoc.rotation) as GameObject;
						newObj.transform.parent = objLoc;
					}
				}
				isEnemysSpawned = true;
			}
		}
	}
	
	public int CheckForEnemys ()
	{
		roomObjLocs = this.gameObject.GetComponentsInChildren <Transform>() as Transform[];
		// check howmay enemys are left in the room
		int enemys = 0;
		int boss = 0;

		if (roomObjLocs != null)
		{

			foreach (Transform objLoc in roomObjLocs)
			{
				if (objLoc.gameObject.tag == "Enemy")
				{
					Debug.Log("Enemy" + objLoc.gameObject.name);
					enemys ++;
				}
				if (objLoc.gameObject.tag == "Boss")
				{
					Debug.Log("Boss" + objLoc.gameObject.name);
					enemys ++;
					boss++;
				}
			}

		}
		if (isBossRoom) {
			Debug.Log("C'est la salle du boss");
			if(boss == 0){
				Debug.Log("Ya plus de bosss");
				Instantiate(endGame, this.transform.position, this.transform.rotation);
			}
		}


		Debug.Log("Nb enemy =" + (enemys));
		return enemys ;
	}
}
