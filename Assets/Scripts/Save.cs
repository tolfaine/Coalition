using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class Save {
	public static int						level ;
	public static bool 						beginGame;
	public  bool 						mapBuilt;


	public  float		playerCurrentHealth;		// create an empty int to store the players current health
	public  float		playerCurrentMaxHealth;		// create an empty int to store the players current health
	public  float		playerCurrentSpeed;			// create an empty int to store the players current health
	public  float		playerCurrentDamage;		// create an empty int to store the players current health
	public float		playerCurrentShotDelay;		// create an empty int to store the players current health
	public float		playerCurrentProjectileSpeed;		// create an empty int to store the players current health


	private int 							dungeonX;
	private int 							dungeonY;
	public  Vector4[,] 				map;


	public int 								roomTotal;
	public int 								roomNum;
	public  Vector2[]					roomList;
	public  RoomBuilderScript[] 		roomScripts;
	private int 							roomToPlaceNum;
	public List<Vector2> 					roomToPlaceList;
	private int 							roomCurrent;
	private Vector2 						roomCurrentPosList;

	public static int[] 					dir;
	public List<int> 						dirPossible;
	private List<int>						randDir;
	private int								numDoorMax;
	private int								numDoorMin;
	private int 							randNumDoor;

	public  RoomComponentList				roomComponentList;

	public string 							pickupInfo = " ";		// an empty string to store info about the pickup;
	public bool 							printPickupInfo;		// if true then print pickup info to screen

	public Door.EnumDir						newDir;

	public  UnityEngine.Object					Player;
	public static GameObject				playerInst;

	public static int 						tailleRoomX = 4;
	public static int 						tailleRoomY = 3;


	public  int						playerCurrentRoom;
	public  Vector2					playerRoomPos;
	public Vector2					playerNewRoomPos;
	public  Vector4					playerRoom;

	public Vector3 					playerPos;
	public  Vector3					playerNewPos;

	public Vector3					camPos;
	public Vector3					camNewPos;
	public  float						camSpeed = 2;

	private float			miniMapX = Screen.width / 22;
	private float			miniMapY = Screen.height / 30;

	public static bool 						isChanging;

	public static  Health bossHealth;
	private static  GameObject[] 	allDoors;	

	public  bool 	loaded;	


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toSave(Map map){
		level =Map.level ;
		beginGame =Map.beginGame;
		mapBuilt =map.mapBuilt ;
		
		
		playerCurrentHealth =Map.playerCurrentHealth ;		// create an empty int to store the players current health
		playerCurrentMaxHealth =Map.playerCurrentMaxHealth ;		// create an empty int to store the players current health
		playerCurrentSpeed =Map.playerCurrentSpeed ;			// create an empty int to store the players current health
		playerCurrentDamage =Map.playerCurrentDamage ;		// create an empty int to store the players current health
		playerCurrentShotDelay =Map.playerCurrentShotDelay ;		// create an empty int to store the players current health
		playerCurrentProjectileSpeed =Map.playerCurrentProjectileSpeed;		// create an empty int to store the players current health
		
		
		dungeonX = map.dungeonX ;
		dungeonY = map.dungeonY;
		this.map =Map.map;
		
		roomTotal =map.roomTotal;
		roomNum =map.roomNum ;
		roomList =Map.roomList;
		roomScripts =Map.roomScripts;
		roomToPlaceNum =map.roomToPlaceNum;
		roomToPlaceList =map.roomToPlaceList;
		roomCurrent =map.roomCurrent;
		roomCurrentPosList =map.roomCurrentPosList;

		
		roomComponentList =map.roomComponentList;
		
		tailleRoomX = 4;
		tailleRoomY = 3;
		
		
		playerCurrentRoom =Map.playerCurrentRoom;
		playerRoomPos =Map.playerRoomPos;
		playerNewRoomPos =Map.playerNewRoomPos;
		playerRoom =Map.playerRoom ;
		
		playerPos =Map.playerPos;
		playerNewPos =Map.playerNewPos;
		
		camPos =Map.camPos;
		camNewPos =Map.camNewPos;
		camSpeed = 2;
		

		miniMapX = Screen.width / 22;
		miniMapY = Screen.height / 30;
		
		isChanging =Map.isChanging;
		
		bossHealth =Map.bossHealth;
		allDoors =Map.allDoors;	
		
		loaded =map.loaded ;	
	}

	public void toLoad(Map map){
		Map.level = level  ;
		Map.beginGame = beginGame;
		map.mapBuilt = mapBuilt ;
		
		
		Map.playerCurrentHealth =  playerCurrentHealth ;		// create an empty int to store the players current health
		Map.playerCurrentMaxHealth = playerCurrentMaxHealth ;		// create an empty int to store the players current health
		Map.playerCurrentSpeed = playerCurrentSpeed ;			// create an empty int to store the players current health
		Map.playerCurrentDamage = playerCurrentDamage ;		// create an empty int to store the players current health
		Map.playerCurrentShotDelay = playerCurrentShotDelay ;		// create an empty int to store the players current health
		Map.playerCurrentProjectileSpeed = playerCurrentProjectileSpeed;		// create an empty int to store the players current health
		
		
		map.dungeonX = dungeonX ;
		map.dungeonY = dungeonY;
		Map.map = this.map;
		
		map.roomTotal = roomTotal;
		map.roomNum = roomNum ;
		Map.roomList = roomList;
		Map.roomScripts = roomScripts;
		map.roomToPlaceNum = roomToPlaceNum;
		map.roomToPlaceList = roomToPlaceList;
		map.roomCurrent = roomCurrent;
		map.roomCurrentPosList = roomCurrentPosList;
		
		
		map.roomComponentList = roomComponentList;
		
		
		Map.playerCurrentRoom = playerCurrentRoom;
		Map.playerRoomPos = playerRoomPos;
		Map.playerNewRoomPos = playerNewRoomPos;
		Map.playerRoom = playerRoom ;
		
		Map.playerPos = playerPos;
		Map.playerNewPos = playerNewPos;
		
		Map.camPos = camPos ;
		Map.camNewPos = camNewPos;
		
		
		Map.isChanging = isChanging;
		
		Map.bossHealth = bossHealth;
		Map.allDoors = allDoors;	
		
		map.loaded = loaded ;	

	}
}
