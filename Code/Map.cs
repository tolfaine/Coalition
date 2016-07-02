public class Dungeon : MonoBehaviour
{
	public int					level;
	private int					mobsMax;
	private int 				mobsMin;
	
	private int 				dungeonX;
	private int 				dungeonY;
	
	private int 				roomTotal;
	
	private int 				roomNum;
	public static Vector2[]	 roomList;
	
	private int 				roomToPlaceNum;
	public static Vector2[] 	roomToPlaceList;
	
	private int 				roomCurrent;
	private Vector2 			roomCurrentPosList;
	
	public static int[] 		dir;
	public ArrayList 			dirPossible;
	private ArrayList 			randDir;
	
	private int					numDoorMax;
	private int 				randNumDoor;
	
	public RoomComponentList	roomComponentList;
	public EnemyList			enemyList;
	public ItemList				itemList;
	public ObjectList			objectList;
	
	public RoomBuilder			roomBuilder;
	public 

	
	public static Vector4[,] map;				
	
	
	public void BuildDungeon()
	{
		Init();
		Rooms();
	}
	public void Init(){
	
		mobsMin = level;
		mobsMax = level + 5;
		
		dungeonX = 20;
		dungeonY = 20;
		
		roomTotal = 10;
		
		roomNum = 1;
		roomList = new Vector2[roomTotal];
		
		roomToPlaceNum = 0;
		roomToPlaceList = new Vector2[roomTotal];
		
		roomCurrent = 1;
		roomCurrentPosList = new Vector2();
		
		dir = new int[]{0,1,2,3};
		dirPossible = new ArrayList();
		randDir = new ArrayList();
		
		dungeon = new Vector4[dungeonX,dungeonY];
		
	}
	
	public void Rooms(){
		PlaceRooms();
		BuildRooms();
	}
	
	public void PlaceRooms(){
		PlaceStartRoom();
		PlaceNormalRooms();
		PlaceSpecialRooms();	
	}
	
	public void PlaceStartRoom(){
		if(roomNum != 1 ){
			Debug.LogError ("Wesh ya un blem la et je sais pas faire un assert en c# hein");
		}
		
		int midX = dungeonX/2;
		int midY = dungeonY/2;
		
		roomList[roomNum] = new Vector2( midX , midY);
		map[midX,midY].x = roomNum;
		map[midX,midY].y = roomNum;
		map[midX,midY].z = 0;
		map[midX,midY].w = 0;
		
		setDoor();
		randDirections();
		addRoomsToPlace();
	
		roomNum ++;
	}
	
	
	public void PlaceNormalRooms(){
		if(roomNum == 1 ){
			Debug.LogError ("Wesh ya un blem la et je sais pas faire un assert en c# hein");
		}

		while(roomNum <= roomTotal - 1){

			getNewRoom();
			addRoomList();
			setDoors();
			randDirections();
			addRoomsToPlace();
			roomNum ++;
		}
	}
	
	public void addRoomList(){
		roomList[roomNum] = new Vector2( roomCurrentPosList.x ,roomCurrentPosList.y );
		map[roomCurrentPosList.x,roomCurrentPosList.y].x = roomNum;
		map[roomCurrentPosList.x,roomCurrentPosList.y].y = 2;
		map[roomCurrentPosList.x,roomCurrentPosList.y].z = Random.Range(;
		map[roomCurrentPosList.x,roomCurrentPosList.y].w = 0;
	}
	
	public void getNewRoom(){
			roomCurrent = Ramdom.Range(0,roomToPlaceNum);
			roomCurrentPosList = roomToPlaceList[roomCurrent];
			roomToPlaceList.remove(roomCurrent);
			roomToPlaceNum --;
	}
	
	public void addRoomsToPlace(){
		foreach (int dir in randDir){
			if(dir == 0){
				roomToPlaceList.add(new Vector2 (roomCurrentPosList.x,roomCurrentPosList.y+1)
			}
			if(dir == 1){
				roomToPlaceList.add(new Vector2 (roomCurrentPosList.x+1,roomCurrentPosList.y)
			}
			if(dir == 2){
				roomToPlaceList.add(new Vector2 (roomCurrentPosList.x,roomCurrentPosList.y-1)
			}
			if(dir == 3){
				roomToPlaceList.add(new Vector2 (roomCurrentPosList.x-1,roomCurrentPosList.y)
			}
		}
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

		if(numDoorMax > dirPossible.TrimToSize()){
			numDoorMax = dirPossible.TrimToSize();
		}
	}
	
	public void setDoorMin(){
		
		numDoorMin = 0;
		
		if(roomToPlaceNum == 0){
			numDoorMin = 1;
		}
	}
	
	public void checkNearbyRooms(){
		dirPossible.AddRange(dir);
		
		if(map[roomCurrentPosList.x +1,roomCurrentPosList.y] != 0){
			dirPossible.remove(1);
		}
		
		if(map[roomCurrentPosList.x - 1,roomCurrentPosList.y] != 0){
			dirPossible.remove(3);
		}
		
		if(map[roomCurrentPosList.x,roomCurrentPosList.y+1] != 0){
			dirPossible.remove(0);
		}
		if(map[roomCurrentPosList.x,roomCurrentPosList.y-1] != 0){
			dirPossible.remove(2);
		}
	}
	
	public void randDirections(){
	
		randNumDoor = Ramdom.Range(numDoorMin,numDoorMax+1);
		
		while(randNumDoor >= 0){
			randDir[randNumDoor] = Random.Range(0,dirPossible.TrimToSize());
			dirPossible.remove(randDir[randNumDoor]);
			randNumDoor --;
		}
	}
	
	public void PlaceSpecialRooms(){
	
	}
	
	public void BuildRooms(){
	
		foreach (Vector2 room in roomList){
			
			
		
		}
	
	}
	
	
}