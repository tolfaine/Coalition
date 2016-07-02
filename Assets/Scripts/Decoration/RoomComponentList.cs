using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Level
{
	public List<Object> type;
}

public class RoomComponentList : MonoBehaviour
{
	public  List<Level> floorsAll;
	public  List<Level> wallsAll;
	public  List<Level> doorsAll;
	public  List<Level> typesRoomAll;

	public  List<Level> EnemyAll;
	public  List<Level> RockAll;
	public  List<Level> PickUpAll;
	public  List<Level> BossAll;

	public 	GameObject 		endGame;

	public  Object coffre;
	public  Object floorDefault;
	public  Object wallDefault;
	public  Object doorDefault;
	public  Object TypeDefault;
	
	public  Object GetFloor(Vector4 room, int level)
	{
		int rand ; 
		Object o;

       Debug.Log ("level" +level);

		o = floorsAll[level].type[(int)room.y-1];
		if(o == null )
		{
			return floorDefault;
		}
		else{
			return o;
		} 
	}
	
	public  Object GetWall(Vector4 room, int level)
	{
		int rand ; 
		Object o;

		o = wallsAll[level].type[(int)room.y-1];
		if(o == null )
		{
			return wallDefault;
		}
		else{
			return o;
		}
	}
	
	public  Object GetDoor(Vector4 room, int level)
	{
		int rand ; 
		Object o;

		//Debug.Log ("type" +(int)(room.y - 1));
		o = doorsAll[level].type[(int)room.y-1];
		if(o == null )
		{
			return doorDefault;
		}
		else{
			return o;
		}
	}
	
	public  Object  GetType(Vector4 room, int level)
	{
		int rand ; 
		GameObject o;

		if(room.y == 2 ){
			rand = Random.Range(0,typesRoomAll[level].type.Count-2);
			//Debug.Log ("level =" + level + " rand =" + rand);
			o = typesRoomAll[level].type[rand] as GameObject;
		}
		else if(room.y == 3){
			//Debug.Log ("level =" + level); 
			//Debug.Log (" count =" + (typesRoomAll[level].type.Count-3));
			o = typesRoomAll[level].type[typesRoomAll[level].type.Count-1] as GameObject;
			RoomBuilderScript r = o.GetComponent<RoomBuilderScript>();
			r.endGame = GetEndGame();
		}
		else{
			o = typesRoomAll[level].type[typesRoomAll[level].type.Count-2] as GameObject;
			RoomBuilderScript r = o.GetComponent<RoomBuilderScript>();
			r.endGame = GetEndGame();
		}
		if(o == null )
		{
			return TypeDefault;
		}
		else{

			return o;
		}
	}

	public  Object GetRocks(int level)
	{
		Object o;
		int rand = Random.Range(0,RockAll[level].type.Count);
		Debug.Log ("level =" + level + " rand =" + rand);
		o = RockAll[level].type[rand];
		if(o == null )
		{
			return doorDefault;
		}
		else{
			return o;
		}
	}

	public  Object GetPickUp()
	{
		Object o;
		int rand = Random.Range(0,PickUpAll[0].type.Count);
		o = PickUpAll[0].type[rand];
		if(o == null )
		{
			return doorDefault;
		}
		else{
			return o;
		}
	}

	public  Object GetEnemy(int level)
	{
		Object o;
		int rand = Random.Range(0,EnemyAll[level].type.Count);
		o =  EnemyAll[level].type[rand];
		if(o == null )
		{
			return doorDefault;
		}
		else{
			return o;
		}
	}

	public  Object GetBoss(int level)
	{
		int rand ; 
		Object o;

		o = BossAll[level].type[0];
		if(o == null )
		{
			return doorDefault;
		}
		else{
			return o;
		}
	}

	public GameObject GetEndGame()
	{
		int rand ; 
		GameObject o;
		
		return o = endGame;

	}

	public GameObject GetCoffre()
	{
		
		return coffre as GameObject;
		
	}
	

}