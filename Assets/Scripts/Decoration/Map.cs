using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
	public static int						level ;
	public static bool 						beginGame;
	public  bool 						mapBuilt;


	public static float		playerCurrentHealth;		// create an empty int to store the players current health
	public static float		playerCurrentMaxHealth;		// create an empty int to store the players current health
	public static float		playerCurrentSpeed;			// create an empty int to store the players current health
	public static float		playerCurrentDamage;		// create an empty int to store the players current health
	public static float		playerCurrentShotDelay;		// create an empty int to store the players current health
	public static float		playerCurrentProjectileSpeed;		// create an empty int to store the players current health


	public int 							dungeonX;
	public int 							dungeonY;
	public static Vector4[,] 				map;

	public Texture2D		currentTex;
	public Texture2D		seenTex;
	public Texture2D		visitedTex;
	public Texture2D		secretTex;
	public Texture2D		bossTex;
	public Texture2D		backgroundImage;
	public Texture2D		treasureTex;
	public Texture2D		heart;

	public int 								roomTotal;
	public int 								roomNum;
	public static Vector2[]					roomList;
	public static RoomBuilderScript[] 		roomScripts;
	public int 							roomToPlaceNum;
	public List<Vector2> 					roomToPlaceList;
	public int 							roomCurrent;
	public Vector2 						roomCurrentPosList;

	public static int[] 					dir;
	public List<int> 						dirPossible;
	public List<int>						randDir;
	public int								numDoorMax;
	public int								numDoorMin;
	public int 							randNumDoor;

	public  RoomComponentList				roomComponentList;

	public string 							pickupInfo = " ";		// an empty string to store info about the pickup;
	public bool 							printPickupInfo;		// if true then print pickup info to screen

	public Door.EnumDir						newDir;

	public  UnityEngine.Object					Player;
	public static GameObject				playerInst;

	public static int 						tailleRoomX = 4;
	public static int 						tailleRoomY = 3;


	public static int						playerCurrentRoom;
	public static Vector2					playerRoomPos;
	public static Vector2					playerNewRoomPos;
	public static Vector4					playerRoom;

	public static Vector3 					playerPos;
	public static Vector3					playerNewPos;

	public static Vector3					camPos;
	public static Vector3					camNewPos;
	public static float						camSpeed = 2;

	public GUISkin			gameGUI;
	public float			miniMapX = Screen.width / 22;
	public float			miniMapY = Screen.height / 30;

	public static bool 						isChanging;

	public static  Health bossHealth;
	public static  GameObject[] 	allDoors;	

	public  bool 	loaded;	

	
	void OnLevelWasLoaded (int lvl)
	{
		if (lvl == 1)
		{
			beginGame = true;
			BuildDungeon();
		}
		
	}
	void Start () {
		DontDestroyOnLoad (this.gameObject);
		beginGame = true;
		if (mapBuilt) {
			BuildDungeon();
		}
	}
	

	public void BuildDungeon()
	{
		if (!loaded) {
			Init ();
		}
		Rooms();
		UpdateMiniMap ();
	}
	public void Init(){
		dungeonX = 30;
		dungeonY = 30;
		roomTotal = 13;
		roomNum = 0;
		roomList = new Vector2[roomTotal+1];
		roomScripts = new RoomBuilderScript[roomTotal + 1];
		roomToPlaceNum = 0;
		roomToPlaceList = new List<Vector2>();
		roomCurrent = 1;
		roomCurrentPosList = new Vector2();
		dir = new int[]{0,1,2,3};
		dirPossible = new List<int>();
		randDir = new List<int>();
		map = new Vector4[dungeonX,dungeonY];
	}
	
	public  void Rooms(){
		if (!loaded) {
						PlaceRooms ();
				}
		BuildRooms();
		mapBuilt = true;
	}
	
	public void PlaceRooms(){
		PlaceStartRoom();
		PlaceNormalRooms();
		PlaceSpecialRooms ();
	}
	
	public void PlaceStartRoom(){
		if(roomNum != 0 ){
			Debug.LogError ("Wesh ya un blem la et je sais pas faire un assert en c# hein");
		}
		int midX = dungeonX/2;
		int midY = dungeonY/2;
		roomCurrentPosList = new Vector2( midX , midY);
		//roomList[roomNum] = roomCurrentPosList;
		roomList[roomNum] = roomCurrentPosList;
		addRoomList(1);
		/*map[midX,midY].x = roomCurrentPosList.x;
		map[midX,midY].y = 0;
		map[midX,midY].z = 0;
		map[midX,midY].w = 0;*/
		setDoors();
		randDirections();
		addRoomsToPlace();
	}

	
	public void PlaceNormalRooms(){
		if(roomNum == 0 ){
			Debug.LogError ("Wesh ya un blem la et je sais pas faire un assert en c# hein");
		}
		while(roomNum <= roomTotal-3){
			getNewRoom();
			addRoomList(2);
			setDoors();
			randDirections();
			addRoomsToPlace();
		}
		Debug.Log ("nbRoom = " + roomNum + "room to place = " + roomToPlaceNum);
	}
	
	public void addRoomList(int type){
		roomList[roomNum] = new Vector2( roomCurrentPosList.x ,roomCurrentPosList.y );
		Debug.Log("SET = " + " [" + roomCurrentPosList.x +";" +roomCurrentPosList.y + "]" );
		map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y].x = roomNum;
		map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y].y = type;
		map [(int)roomCurrentPosList.x, (int)roomCurrentPosList.y].z = 0;
		map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y].w = 0;
		roomNum ++;
	}
	
	public void getNewRoom(){
		//Debug.Log ("nb room to place = " + roomToPlaceNum);
		roomCurrent = (int)UnityEngine.Random.Range(0,roomToPlaceNum);
//		Debug.Log ("NbRoom = " + roomNum + "nbroom" + roomNum);
		roomCurrentPosList = roomToPlaceList[roomCurrent]; 
		roomToPlaceList.RemoveAt(roomCurrent);
		roomToPlaceNum --;
	}
	
	public void addRoomsToPlace(){
		foreach (int dir in randDir){
			if(dir == 0  && map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y+1].y <= 0 && !inRoomToPlace((int)roomCurrentPosList.x , (int)roomCurrentPosList.y+1)){
				roomToPlaceList.Add(new Vector2 (roomCurrentPosList.x,roomCurrentPosList.y+1));
				roomToPlaceNum ++;
				//Debug.Log("ADD = " +roomNum + " :" + " [" + roomCurrentPosList.x +";" +(roomCurrentPosList.y+1) + "]" );
			}

			if(dir == 1 && map[(int)roomCurrentPosList.x +1,(int)roomCurrentPosList.y].y <= 0 & !inRoomToPlace((int)roomCurrentPosList.x+1 , (int)roomCurrentPosList.y)){
				roomToPlaceList.Add(new Vector2 (roomCurrentPosList.x+1,roomCurrentPosList.y));
				roomToPlaceNum ++;
				//Debug.Log("ADD = " +roomNum + " :" + " [" + (roomCurrentPosList.x+1) +";" +(roomCurrentPosList.y) + "]" );
			}

			if(dir == 3 && map[(int)roomCurrentPosList.x - 1,(int)roomCurrentPosList.y].y <= 0 & !inRoomToPlace((int)roomCurrentPosList.x-1 , (int)roomCurrentPosList.y)){
				roomToPlaceList.Add(new Vector2 (roomCurrentPosList.x-1,roomCurrentPosList.y));
				roomToPlaceNum ++;
				//Debug.Log("ADD = " +roomNum + " :" + " [" + (roomCurrentPosList.x-1) +";" +(roomCurrentPosList.y) + "]" );
			}

			try{
				if(map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y-1].y <= 0){
				}
			}catch (Exception e){
				if(dir == 2 && map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y-1].x  <= 0 & !inRoomToPlace((int)roomCurrentPosList.x , (int)roomCurrentPosList.y-1)){
					roomToPlaceList.Add(new Vector2 (roomCurrentPosList.x,roomCurrentPosList.y-1));
					roomToPlaceNum ++;
					//Debug.Log("ADD = " +roomNum + " :" + " [" + roomCurrentPosList.x +";" +(roomCurrentPosList.y-1) + "]" );
				}
			}
		}
	}

	public bool inRoomToPlace(int x , int y){

		foreach (Vector2 room in roomToPlaceList) {
			if(room.x == x && room.y ==y){
				return true;
			}
		}
		return false;
	}
	
	public void setDoors(){
		checkNearbyRooms();
		setDoorMax();
		setDoorMin();
	}
	public void setDoorMax(){
		numDoorMax = roomTotal - roomNum - roomToPlaceNum ;
		if(numDoorMax > 3 ){
			numDoorMax = 3;
		}
		if(numDoorMax > dirPossible.Count ){
			numDoorMax = dirPossible.Count;
		}
		if(numDoorMax < 0){
			numDoorMax = 0;
		}
		if(roomNum == roomTotal-1 || roomNum == roomTotal-2){
			numDoorMax = 3;
		}
	}
	
	public void setDoorMin(){
		numDoorMin = 0;
		if(roomToPlaceNum == 0){
			numDoorMin = 1;
		}

		if(roomNum == 1){
			numDoorMin = 3;
		}

		if (roomToPlaceNum == 0 && roomNum > roomTotal) {
			numDoorMin = 1;
		}

		if(roomNum == roomTotal-1 || roomNum == roomTotal-2){
			numDoorMin = 2;
		}
	}
	
	public void checkNearbyRooms(){
		dirPossible = new List<int>();
		dirPossible.AddRange(dir);
		try{
			if(map[(int)roomCurrentPosList.x +1,(int)roomCurrentPosList.y].x != 0){
				dirPossible.Remove(1);
				//Debug.Log(1);
			}
		}catch (Exception e){

		}
		try{
			if(map[(int)roomCurrentPosList.x - 1,(int)roomCurrentPosList.y].x != 0){
				dirPossible.Remove(3);
				//Debug.Log(3);
			}
		}catch (Exception e){
			
		}
		try{
			if(map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y+1].x != 0){
				dirPossible.Remove(0);
				//Debug.Log(0);
			}
		}catch (Exception e){
			
		}
		try{
			if(map[(int)roomCurrentPosList.x,(int)roomCurrentPosList.y-1].x != 0){
				dirPossible.Remove(2);
				//Debug.Log(2);
			}
		}catch (Exception e){
			
		}
	}
	
	public void randDirections(){
		randDir = new List<int> ();
		if (numDoorMax != 0) {
			randNumDoor = UnityEngine.Random.Range (numDoorMin, numDoorMax+1 ); 

			//Debug.Log("RAND numDoorMax DOOR" + randNumDoor );
			while (randNumDoor >= 0 && dirPossible.Count>0) {
				int i = UnityEngine.Random.Range (0, dirPossible.Count);
				//Debug.Log("i=" + i + "dirPossible.Coun=" + dirPossible.Count + "room to place= " + roomToPlaceNum);
				int dir = dirPossible[i];
				//Debug.Log("dir=" + dir);
				randDir.Add (dir);
				dirPossible.RemoveAt(i);
				randNumDoor --;
			}
		}
	}
	
	public void PlaceSpecialRooms(){
		getNewRoom();
		addRoomList(3);


		int rand = UnityEngine.Random.Range(0,5);
		Debug.Log ("SECRET = " + rand);
		if (rand == 0) {
			getNewRoom();
			addRoomList(4);
		}
	}
	
	public void BuildRooms(){
		//GameObject 			floorInst;
		GameObject 			wallInst;
		GameObject 			doorInst;
		//GameObject 			doorInst;
		Vector4 			roomMap;
		Vector3 			roomPos; // A initialiser
		Vector3				placement;

		foreach (Vector2 room in roomList) {
				roomMap = map [(int)room.x, (int)room.y];
				roomPos = new Vector3 (room.x, 0, room.y);
			placement = new Vector3 (room.x*tailleRoomX, room.y*tailleRoomY, 0);
			
				if (room.y >= 1){
		//Floor
				//Debug.Log ("[" + room.x + ";" + room.y + "]");
				Instantiate (roomComponentList.GetFloor (roomMap, level), placement, this.transform.rotation);

				//wall haut
				wallInst = Instantiate (roomComponentList.GetWall (roomMap, level), placement, this.transform.rotation) as GameObject;
		/*
				//wall droit
				wallInst = Instantiate (roomComponentList.GetWall (roomMap, level), placement, this.transform.rotation) as GameObject;
				wallInst.transform.Rotate (0, 0, -90);
		
				//wall bas
				wallInst = Instantiate (roomComponentList.GetWall (roomMap, level), placement, this.transform.rotation) as GameObject;
				wallInst.transform.localScale = new Vector3 (wallInst.transform.localScale.x, -wallInst.transform.localScale.y, wallInst.transform.localScale.z);
		
				//wall gauche
				wallInst = Instantiate (roomComponentList.GetWall (roomMap, level), placement, this.transform.rotation)as GameObject;
				wallInst.transform.Rotate (0, 0, 90);
				wallInst.transform.localScale = new Vector3 (-wallInst.transform.localScale.x, wallInst.transform.localScale.y, wallInst.transform.localScale.z);
*/

				if(roomMap.y == 2 || roomMap.y == 3 || roomMap.y == 4){
					GameObject gb = Instantiate (roomComponentList.GetType(roomMap,level),placement, this.transform.rotation) as GameObject;
					RoomBuilderScript rbs = gb.GetComponent<RoomBuilderScript>() as RoomBuilderScript;
					rbs.level = level;
					if(roomMap.y == 3){
						rbs.isBossRoom = true;
					}
					roomScripts[(int)roomMap.x] = rbs;
					roomScripts[(int)roomMap.x].RoomLayout();
				}
				//DOORS

				try {
					if (map [(int)room.x, (int)room.y + 1].y >= 1) {
								//Debug.Log ("Door ! ");
						doorInst = Instantiate (roomComponentList.GetDoor (map [(int)room.x, (int)room.y + 1], level), placement, this.transform.rotation) as GameObject;

						Door door = doorInst.GetComponent<Door>();
						door.typeDir = Door.EnumDir.NORD;
						door.room = (int)roomMap.x;
						}
				} catch (Exception e) {
				}

					if (map [(int)room.x + 1, (int)room.y].y >= 1) {
						//Debug.Log ("Door ! ");
						//Debug.Log ("DOOR : [" +(int)(room.x + 1) + ";" + (int)(room.y) + "]  type:" + map [(int)room.x , (int)room.y + 1 ].y );
						placement = new Vector3 (room.x*tailleRoomX+0.5f, room.y*tailleRoomY, 0);
						doorInst = Instantiate (roomComponentList.GetDoor (map [(int)room.x + 1, (int)room.y], level), placement, this.transform.rotation) as GameObject;
						doorInst.transform.Rotate (0, 0, -90);

						Door door = doorInst.GetComponent<Door>();
					  	door.typeDir = Door.EnumDir.EST;
						door.room = (int)roomMap.x;

					                              }

				try {
					if (map [(int)room.x, (int)room.y - 1].y >= 1) {
								//Debug.Log ("Door ! ");
						placement = new Vector3 (room.x*tailleRoomX, room.y*tailleRoomY, 0);
						doorInst = Instantiate (roomComponentList.GetDoor (map [(int)room.x, (int)room.y - 1], level), placement, this.transform.rotation)as GameObject;
						doorInst.transform.localScale = new Vector3 (doorInst.transform.localScale.x, -doorInst.transform.localScale.y, doorInst.transform.localScale.z);

						Door door = doorInst.GetComponent<Door>();
						door.typeDir = Door.EnumDir.SUD;
						door.room = (int)roomMap.x;
						}
				} catch (Exception e) {
				}
				try {
						if (map [(int)room.x - 1, (int)room.y].y >= 1) {
								//Debug.Log ("Door ! ");
						placement = new Vector3 (room.x*tailleRoomX - 0.5f, room.y*tailleRoomY, 0);
						doorInst = Instantiate (roomComponentList.GetDoor (map [(int)room.x - 1, (int)room.y], level), placement, this.transform.rotation)as GameObject;
						doorInst.transform.Rotate (0, 0, 90);
						doorInst.transform.localScale = new Vector3 (-doorInst.transform.localScale.x, doorInst.transform.localScale.y, doorInst.transform.localScale.z);

						Door door = doorInst.GetComponent<Door>();
						door.typeDir = Door.EnumDir.OUEST;
						door.room = (int)roomMap.x;
						}
				} catch (Exception e) {
				}
	
			}


		}
		allDoors = GameObject.FindGameObjectsWithTag("Door");
		SpawnPlayer();
	}
	void SpawnPlayer ()	
	{
		if (beginGame)
		{
			playerCurrentRoom = 0;
			beginGame = false;
		}
		playerRoomPos = roomList[playerCurrentRoom];
		playerInst = Instantiate (Player, new Vector3 (playerRoomPos.x * tailleRoomX ,playerRoomPos.y * tailleRoomY ,0), transform.rotation) as GameObject;

		if (playerCurrentHealth == 0) {
			ResetStat ();

		} 
		else {
			SetStat ();
		}


		Camera.mainCamera.transform.position = new Vector3 (playerRoomPos.x * tailleRoomX ,playerRoomPos.y * tailleRoomY + 0.5f,-3f);
		CheckDoors ();
	}

	void SetStat(){
		Health health = playerInst.GetComponent<Health>();
		Gun gun = playerInst.GetComponentInChildren<Gun>();
		Player Player = playerInst.GetComponent<Player>();

		health.health = (int)playerCurrentHealth;		
		health.maxHealth = (int)playerCurrentMaxHealth;		
		Player.speed= playerCurrentSpeed;			
		gun.damage =	(int)playerCurrentDamage;		
		gun.shootDelay =	playerCurrentShotDelay;		
		gun.projectileSpeed = playerCurrentProjectileSpeed;
	}

	void ResetStat(){
		Health health = playerInst.GetComponent<Health>();
		Gun gun = playerInst.GetComponentInChildren<Gun>();
		Player Player = playerInst.GetComponent<Player>();
		
		health.health = 200;
		health.maxHealth = 200;	
		Player.speed = 1; 	
		gun.damage = 50;
		gun.shootDelay = 0.5f;
		gun.projectileSpeed = 1;
	}

	public void PickUpInfo ()
	{
		StartCoroutine (TelegraphPickup ());
	}
	
	public IEnumerator TelegraphPickup ()
	{
		printPickupInfo = true;
		yield return new WaitForSeconds (3f);
		printPickupInfo = false;
		yield return null;
	}


	public  void ChangeRooms ()
	{
		if (!isChanging)
		{
			playerRoomPos = roomList[playerCurrentRoom];
			playerPos = playerInst.transform.position;
			
			if (newDir == Door.EnumDir.NORD)
			{
				Debug.Log ("Nord");
				playerNewRoomPos = new Vector2(playerRoomPos.x , playerRoomPos.y +1);
				playerCurrentRoom = (int) map[(int)playerNewRoomPos.x,(int)playerNewRoomPos.y].x;
				playerNewPos = new Vector3 (playerPos.x,playerPos.y + 1f,0); 
			}
			else if (newDir == Door.EnumDir.SUD)
			{
				Debug.Log ("sud");
				playerNewRoomPos = new Vector2(playerRoomPos.x , playerRoomPos.y -1);
				playerCurrentRoom = (int) map[(int)playerNewRoomPos.x,(int)playerNewRoomPos.y].x;
				playerNewPos = new Vector3 (playerPos.x,playerPos.y - 1f,0); 
			}
			else if (newDir == Door.EnumDir.EST)
			{
				Debug.Log ("est");
				playerNewRoomPos = new Vector2(playerRoomPos.x +1 , playerRoomPos.y );
				playerCurrentRoom = (int) map[(int)playerNewRoomPos.x,(int)playerNewRoomPos.y].x;
				playerNewPos = new Vector3 (playerPos.x  + 1f,playerPos.y,0); 
			}
			else if (newDir == Door.EnumDir.OUEST)
			{
				Debug.Log ("ouest");
				playerNewRoomPos = new Vector2(playerRoomPos.x -1, playerRoomPos.y );
				playerCurrentRoom = (int) map[(int)playerNewRoomPos.x,(int)playerNewRoomPos.y].x;
				playerNewPos = new Vector3 (playerPos.x - 1f,playerPos.y,0 ); 
			}

			playerRoomPos = playerNewRoomPos;

			playerInst.transform.position = playerNewPos;
			camPos = Camera.mainCamera.transform.position;
			camNewPos = new Vector3 (playerNewRoomPos.x * tailleRoomX,playerNewRoomPos.y * tailleRoomY+ 0.5f ,-3f);

			Debug.Log ("Coroutinne");
			MoveCamPos();
		}
		UpdateMiniMap ();
	}

	public static void MoveCamPos()
	{
		Debug.Log ("Moving cam");
		float i = 0;
		Vector3 currentPlayerPos = playerInst.transform.position;

		isChanging = true;

		Camera.mainCamera.transform.position = camNewPos;

		isChanging = false;
		CheckDoors ();
		Debug.Log ("End Moving cam");
	}

	public static void EnemyDie()
	{
		CheckDoors ();
	}

	

	public static void CheckDoors ()
	{	
		Debug.Log ("Ya encore des enemy");
		if (roomScripts[playerCurrentRoom] != null)
		{
			if (roomScripts[playerCurrentRoom].CheckForEnemys() != 0)
			{
				Debug.Log ("Ya encore des enemy");
				// spawn the enemies
				roomScripts[playerCurrentRoom].SpawnEnemys();
				// if this is the boss room then store the bosses health
				if (roomScripts[playerCurrentRoom].isBossRoom && !roomScripts[playerCurrentRoom].isBossDead)
				{
					bossHealth = roomScripts[playerCurrentRoom].gameObject.GetComponentInChildren<Health>();
				}
				// close the doors
				foreach (GameObject door in allDoors)
				{
					if (door != null)
					{
						//Debug.Log ("Ferme toi !");
						door.BroadcastMessage("Close",map[(int)playerRoomPos.x , (int)playerRoomPos.y].x,SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				foreach (GameObject door in allDoors)
				{
					if (door != null)
					{
						door.BroadcastMessage("Open",map[(int)playerRoomPos.x , (int)playerRoomPos.y].x,SendMessageOptions.DontRequireReceiver);
					}
				}
			}
					
		}
		else
		{
			foreach (GameObject door in allDoors)
			{
				if (door != null)
				{
					door.BroadcastMessage("Open",map[(int)playerRoomPos.x , (int)playerRoomPos.y].x,SendMessageOptions.DontRequireReceiver);
				}
			}
		}

	}


	void OnGUI ()
	{
		
		
		if (mapBuilt)
		{
			Vector4 mapRoom;
			int x = 0;
			int y = 0;
			
			GUI.skin = gameGUI;
			GUI.DrawTexture (new Rect (0,0,Screen.width ,Screen.height/5  + Screen.height/20),backgroundImage);
			//GUI.DrawTexture (new Rect (0-Screen.height/20,0,Screen.height/5 ,Screen.width),backgroundImage);
			//GUI.DrawTexture (new Rect (Screen.height,0,Screen.height/5,Screen.width),backgroundImage);
			
			
			if (printPickupInfo)
			{
				GUI.Box (new Rect (Screen.width / 2 - 100, Screen.height/2 + 300, 200,40), pickupInfo);
			}

			while (y < dungeonY)
			{
				x = 0;
				while (x < dungeonY)
				{
					mapRoom = map[x,y];
					if (mapRoom.y > 0)
					{
						if (mapRoom.x == playerCurrentRoom)
						{

							if (mapRoom.y == 3)
							{
								// draw the bosses health bar
								if (bossHealth != null && !roomScripts[playerCurrentRoom].isBossDead)
								{
									GUI.DrawTexture (new Rect (x*30, -y*30+22*30,miniMapX  * 4,miniMapY), seenTex, ScaleMode.StretchToFill);
									GUI.DrawTexture (new Rect (x*30, -y*30+22*30,(miniMapX  * 3) / (bossHealth.maxHealth / bossHealth.health),miniMapY), visitedTex, ScaleMode.StretchToFill);
								}
							}

							// make the player location flash
							GUI.DrawTexture (new Rect ( x*30, -y*30+19*30,miniMapY-(miniMapY/5),miniMapY-(miniMapY/5)), currentTex);
						}
						else
						{
							if (mapRoom.y == 3 && mapRoom.w > 0)
							{
								GUI.DrawTexture (new Rect ( x*30, -y*30 +19*30,miniMapY-(miniMapY/5),miniMapY-(miniMapY/5)), bossTex);
							}
				
							else if (mapRoom.y == 4 && mapRoom.w > 0)
							{
								GUI.DrawTexture (new Rect ( x*30, -y*30 +19*30,miniMapY-(miniMapY/5),miniMapY-(miniMapY/5)), treasureTex);
							}
							else
							{
								if (mapRoom.w == 1)
								{
									GUI.DrawTexture (new Rect ( x*30, -y*30 +19*30,miniMapY-(miniMapY/5),miniMapY-(miniMapY/5)), seenTex);
								}
								if (mapRoom.w == 2)
								{
									GUI.DrawTexture (new Rect ( x*30, -y*30 +19*30,miniMapY-(miniMapY/5),miniMapY-(miniMapY/5)), visitedTex);
								}
							}
						}	
					}
					x++;
				}
				y++;
			}		
		}
	}	

	void  UpdateMiniMap ()
	{
		// update the minimap so it knows that the player has visited this room

		map [(int)playerRoomPos.x, (int)playerRoomPos.y].w = 2;
		// update the manimap with which rooms the player has seen
		if (map[(int)playerRoomPos.x+1,(int)playerRoomPos.y].x != 0)
		{
			if (map[(int)playerRoomPos.x+1,(int)playerRoomPos.y].w < 1)
			{
				map[(int)playerRoomPos.x+1,(int)playerRoomPos.y].w = 1;
			}
		}
		if (map[(int)playerRoomPos.x-1,(int)playerRoomPos.y].x != 0)
		{
			if (map[(int)playerRoomPos.x-1,(int)playerRoomPos.y].w < 1)
			{
				map[(int)playerRoomPos.x-1,(int)playerRoomPos.y].w = 1;
			}
		}
		if (map[(int)playerRoomPos.x,(int)playerRoomPos.y+1].x != 0)
		{
			if (map[(int)playerRoomPos.x,(int)playerRoomPos.y+1].w < 1)
			{
				map[(int)playerRoomPos.x,(int)playerRoomPos.y+1].w = 1;
			}
		}
		if (map[(int)playerRoomPos.x,(int)playerRoomPos.y-1].x != 0)
		{
			if (map[(int)playerRoomPos.x,(int)playerRoomPos.y-1].w < 1)
			{
				map[(int)playerRoomPos.x,(int)playerRoomPos.y-1].w = 1;
			}
		}	
		
	}


	
	
}