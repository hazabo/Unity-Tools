using UnityEngine;
using System.Collections;


public class TinyKeepMazeGeneration_Demo_Mode : MonoBehaviour {

    struct connection
    {
        public int start;
        public int end;
        public Vector2 distance;
        public int set; //this is to show which other connections it is connected to this is used to stop loops
    }
    public GameObject roomPrefab;
    public int maxDungeonWidth;
    public int maxDungeonHeight;
    public int maxRoomWidth;
    public int maxRoomHeight;
    public int initialRooms;
    public int minRoomSize;
    private GameObject[] dungeonRooms;
    private GameObject[] bigRooms;
    private connection[] minSpanTree;
    private GameObject[] paths;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Initialize());
	}

    IEnumerator Initialize()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        WaitForSeconds pause = new WaitForSeconds(1.0f);

        //code has been made messy to allow use of waitforseconds to make it nice to watch see other script 
        //Spawn boxes of random sizes in random positions
        //moved loop here so can call waitforseconds
        dungeonRooms = new GameObject[initialRooms];
        for (int i = 0; i < initialRooms; i++)
        {
            GenerateRoom(i);
            yield return new WaitForSeconds(0.01f);
        }
        //Turn is kinematic off for all rooms so they will resolve their collisions
        ApplyPhysics();
        Time.timeScale = 3;
        //wait for the rooms to finish moving
        do
        {
            yield return delay;
            //movement towards end is very slow so if we scale speed with time passed it makes the end faster without making the start harder to watch
            Time.timeScale += Time.deltaTime;
        } while (!SimulationFinished());
        
        Time.timeScale = 1;
        //change the rooms back to kinematic so they stop moving, change to istrigger for later
        DisablePhysics();
        //Add all the rooms over a certain size to a new array
        InitializeFindBigRooms();
        //FindBigRooms();
        for (int i = 0, j = 0; i < initialRooms; i++)
        {
            float width = dungeonRooms[i].transform.localScale.x;
            float height = dungeonRooms[i].transform.localScale.y;
            if (width * height > minRoomSize)
            {
                bigRooms[j] = dungeonRooms[i];
                bigRooms[j].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                j++;
                yield return delay;
            }
        }
        //Find a minimum spanning tree for all the big rooms
        FindMinSpanTree();
        yield return pause;
        //Draw the min span tree so I can see if it works properly
        //moved loop here so can call waitforseconds
        paths = new GameObject[minSpanTree.Length * 2];
        for (int i = 0; i < minSpanTree.Length; i++)
        {
            AddPaths(i);
            yield return delay;
        }
        yield return pause;
        //wait for a physics update
        ApplyPhysics();
        //wait for update so collisions are registered
        yield return new WaitForFixedUpdate();

        //Make any box that collides with a path part of the dungeon
        //FindCollisions();
        for (int i = 0; i < dungeonRooms.Length; i++)
        {
            if (dungeonRooms[i].GetComponent<SpriteRenderer>().color != new Color(255, 0, 0)) //if the room is not a big room 
            {
                for (int j = 0; j < paths.Length; j++)
                {
                    if (paths[j] != null)
                    {
                        if (dungeonRooms[i].GetComponent<Collider2D>().IsTouching(paths[j].GetComponent<Collider2D>()))
                        {
                            dungeonRooms[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                            yield return delay;
                        }
                    }
                }
            }
        }
        yield return pause;
        DisablePhysics();
        HideInactiveRooms();
    }

    bool SimulationFinished()
    {
        bool notFoundActive = true;

        for (int i = 0; i<dungeonRooms.Length;i++)
        {
            if (!dungeonRooms[i].GetComponent<Rigidbody2D>().IsSleeping())
            {
                notFoundActive = false;
                break;
            }
        }


        return notFoundActive;
    }

    IEnumerator wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }


    void GenerateRoom(int i)
    {
        Vector3 randomPos = new Vector3(Random.Range(maxDungeonWidth * -0.5f, maxDungeonWidth * 0.5f), Random.Range(maxDungeonHeight * -0.5f, maxDungeonHeight * 0.5f), 0.0f);
        dungeonRooms[i] = (GameObject)Instantiate(roomPrefab, randomPos, Quaternion.identity);
        //Use square root so that there are more small rooms 
        dungeonRooms[i].transform.localScale = new Vector3(Mathf.Round(Mathf.Pow(Random.Range(1.0f, Mathf.Sqrt(maxRoomWidth)), 2)), Mathf.Round(Mathf.Pow(Random.Range(1.0f, Mathf.Sqrt(maxRoomHeight)), 2)), 1);
    }
    
    void ApplyPhysics()
    {
        for (int i=0;i<initialRooms;i++)
        {
            dungeonRooms[i].GetComponent<Rigidbody2D>().isKinematic = false;
        }
        if (paths != null)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] != null)
                {
                    paths[i].GetComponent<Rigidbody2D>().isKinematic = false;
                }
            }
        }
    }

    void DisablePhysics()
    {
        for (int i = 0; i < initialRooms; i++)
        {
            dungeonRooms[i].GetComponent<Rigidbody2D>().isKinematic = true;
        }
        if (paths != null)
        {

            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] != null)
                {
                    paths[i].GetComponent<Rigidbody2D>().isKinematic = true;
                }
            }
        }
    }

    void InitializeFindBigRooms()
    {
        //Find the number of big rooms generated 
        int bigRoomsFound = 0;
        for (int i = 0; i < initialRooms; i++)
        {
            float width = dungeonRooms[i].transform.localScale.x;
            float height = dungeonRooms[i].transform.localScale.y;
            if (width * height > minRoomSize)
            {
                bigRoomsFound++;
            }
        }
        //Initialize arrays based on the number of big rooms
        bigRooms = new GameObject[bigRoomsFound];
        minSpanTree = new connection[bigRoomsFound - 1];
        //Initialize the start and end for each connection to -1 as 0 is a possible connection
        for (int i = 0; i < minSpanTree.Length; i++)
        {
            minSpanTree[i].start = -1;
            minSpanTree[i].end = -1;
        }
    }
	
    void FindMinSpanTree()
    {
        //Option 1 make a table of all the connections from each point to each other point then run prims or kruskals on that
        //Easier but less efficient
        //Option 2 delaunay triangulation then run prims or kruskals
        //Hard but significantly more efficient
        //See euclidean minimum spanning trees

        //Option 2 is hard lets do option 1 
        int x = bigRooms.Length;
        connection[] possibleEdges = new connection[(int)(((x-1) * (x)) * 0.5f)];//will always result in an int so it is safe to cast to an int
        int arrayPosition = 0;
        for (int i = 0; i < x; i++) 
        {
            for (int j = i+1; j < x; j++) 
            {
                possibleEdges[arrayPosition].start = i;
                possibleEdges[arrayPosition].end = j;
                possibleEdges[arrayPosition].distance = bigRooms[j].transform.position - bigRooms[i].transform.position;
                arrayPosition++;
            }
        }
        //Bubble sort
        for (int i = (possibleEdges.Length - 1); i >= 0; i--)
        {
            for (int j = 1; j <= i; j++)
            {
                if (possibleEdges[j - 1].distance.sqrMagnitude > possibleEdges[j].distance.sqrMagnitude)
                {
                    connection temp = possibleEdges[j - 1];
                    possibleEdges[j - 1] = possibleEdges[j];
                    possibleEdges[j] = temp;
                }
            }
        }
        int connectionsFound = 0; //position in min span tree array
        int connectionsChecked = 0; //position in possible edges array
        int numOfSets = 0;
        while (connectionsFound<minSpanTree.Length)
        {
            int willMakeLoop1 = -1;
            int willMakeLoop2 = -2; //initialized to a different number so 1!=2 unless they are both changed to the same
            //if they are the same a loop will be made
            for(int i=0; i<connectionsFound;i++)
            {
                //this does not work, will not join 2 sets of paths
                if ((possibleEdges[connectionsChecked].start == minSpanTree[i].start || possibleEdges[connectionsChecked].start == minSpanTree[i].end) && willMakeLoop1 == -1)
                {
                    willMakeLoop1 = minSpanTree[i].set;
                }

                if ((possibleEdges[connectionsChecked].end == minSpanTree[i].start || possibleEdges[connectionsChecked].end == minSpanTree[i].end) && willMakeLoop2 == -2)
                {
                    willMakeLoop2 = minSpanTree[i].set;
                }
            }
            if (willMakeLoop1 != willMakeLoop2)
            {
                if (willMakeLoop1 != -1)
                {
                    if (willMakeLoop2 != -2)
                    {
                        //both ends are part of a set
                        minSpanTree[connectionsFound] = possibleEdges[connectionsChecked];
                        minSpanTree[connectionsFound].set = willMakeLoop1;
                        for (int i = 0; i < connectionsFound; i++)
                        {
                            if (minSpanTree[i].set == willMakeLoop2)
                            {
                                minSpanTree[i].set = willMakeLoop1;
                            }
                        }
                        //for each connection in minspantree if connection set = willmakeloop2 connection set = willmakeloop1
                    }
                    else
                    {
                        //start is part of a set
                        //set = willmakeloop1
                        minSpanTree[connectionsFound] = possibleEdges[connectionsChecked];
                        minSpanTree[connectionsFound].set = willMakeLoop1;
                    }
                }
                else if (willMakeLoop2 != -2)
                {
                    //end is part of a set
                    //set = willmakeloop2
                    minSpanTree[connectionsFound] = possibleEdges[connectionsChecked];
                    minSpanTree[connectionsFound].set = willMakeLoop2;
                }
                else
                {
                    //makes a new set
                    minSpanTree[connectionsFound] = possibleEdges[connectionsChecked];
                    minSpanTree[connectionsFound].set = numOfSets;
                    numOfSets++;
                }
                connectionsFound++;
            }
            connectionsChecked++;
        }
    
        //Attempting delaunay triangulation
        //Sort the positions by x position

    }

    void AddPaths(int i)
    {
        Vector2 pathDistance = minSpanTree[i].distance;
        GameObject startRoom = bigRooms[minSpanTree[i].start];
        GameObject endRoom = bigRooms[minSpanTree[i].end];
        //if abs(pathdistance.y) < endroom.halfheight xPath width is distance - both rooms half width no y path
        //else if abs(pathdistance.x) < endroom.halfwidth yPath Height is distance - both rooms half height no x path
        //else xPath width is distance - start room half width and yPath height is distance - end room half height

        //float xDistance = pathDistance.x + startRoom.transform.localScale.x * (pathDistance.x > 0 ? (-0.5f) : (0.5f));
        //float yDistance = pathDistance.y + endRoom.transform.localScale.y * (pathDistance.y < 0 ? (0.5f) : (-0.5f));

        if (Mathf.Abs(pathDistance.y) < endRoom.transform.localScale.y * 0.5f)
        {
            //xPath width is distance - both rooms half width, no y path
            float xPathWidth = Mathf.Abs(pathDistance.x) - (startRoom.transform.localScale.x + endRoom.transform.localScale.x) * 0.5f;
            Vector2 xPathPos = new Vector2(startRoom.transform.position.x + (startRoom.transform.localScale.x + xPathWidth) * (pathDistance.x > 0 ? (0.5f) : (-0.5f)),
                                            startRoom.transform.position.y); //if pathdistance.x > 0 xPos = startroompos + startroom half width + half xPathWidth else xPos = startroompos - startroom halfwidth - half xPathWidth
            paths[i * 2] = (GameObject)Instantiate(roomPrefab, xPathPos, Quaternion.identity);
            paths[i * 2].transform.localScale = new Vector3(xPathWidth, 1, 1);
            paths[i * 2].GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            paths[i * 2].GetComponent<Collider2D>().isTrigger = true;
            paths[i * 2].GetComponent<SpriteRenderer>().sortingLayerName = "Paths";
        }
        else if (Mathf.Abs(pathDistance.x) < endRoom.transform.localScale.x * 0.5f)
        {
            //yPath Height is distance - both rooms half height, no x path
            float yPathHeight = Mathf.Abs(pathDistance.y) - (startRoom.transform.localScale.y + endRoom.transform.localScale.y) * 0.5f;
            Vector2 yPathPos = new Vector2(startRoom.transform.position.x,
                                           startRoom.transform.position.y + (startRoom.transform.localScale.y + yPathHeight) * (pathDistance.y > 0 ? (0.5f) : (-0.5f)));
            paths[i * 2] = (GameObject)Instantiate(roomPrefab, yPathPos, Quaternion.identity);
            paths[i * 2].transform.localScale = new Vector3(1, yPathHeight, 1);
            paths[i * 2].GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            paths[i * 2].GetComponent<Collider2D>().isTrigger = true;
            paths[i * 2].GetComponent<SpriteRenderer>().sortingLayerName = "Paths";
        }
        else
        {
            //xPath width is distance - start room half width
            //yPath height is distance - end room half height
            float xPathWidth = Mathf.Abs(pathDistance.x) - startRoom.transform.localScale.x * 0.5f + 0.5f;
            Vector2 xPathPos = new Vector2(startRoom.transform.position.x + (startRoom.transform.localScale.x + xPathWidth) * (pathDistance.x > 0 ? (0.5f) : (-0.5f)),
                                           startRoom.transform.position.y);
            paths[i * 2] = (GameObject)Instantiate(roomPrefab, xPathPos, Quaternion.identity);
            paths[i * 2].transform.localScale = new Vector3(xPathWidth, 1, 1);
            paths[i * 2].GetComponent<SpriteRenderer>().color = new Color(0, 255, 0); paths[i * 2].GetComponent<Collider2D>().isTrigger = false;
            paths[i * 2].GetComponent<Collider2D>().isTrigger = true;
            paths[i * 2].GetComponent<SpriteRenderer>().sortingLayerName = "Paths";

            float yPathHeight = Mathf.Abs(pathDistance.y) - endRoom.transform.localScale.y * 0.5f - 0.5f;
            Vector2 yPathPos = new Vector2(endRoom.transform.position.x,
                                           startRoom.transform.position.y + yPathHeight * (pathDistance.y > 0 ? (0.5f) : (-0.5f)) + (pathDistance.y > 0 ? (0.5f) : (-0.5f)));
            paths[i * 2 + 1] = (GameObject)Instantiate(roomPrefab, yPathPos, Quaternion.identity);
            paths[i * 2 + 1].transform.localScale = new Vector3(1, yPathHeight, 1);
            paths[i * 2 + 1].GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            paths[i * 2 + 1].GetComponent<Collider2D>().isTrigger = true;
            paths[i * 2 + 1].GetComponent<SpriteRenderer>().sortingLayerName = "Paths";
        }
    }

    void FindCollisions()
    {
        for (int i = 0; i < dungeonRooms.Length; i++)
        {
            if (dungeonRooms[i].GetComponent<SpriteRenderer>().color != new Color(255, 0, 0)) //if the room is not a big room 
            {
                for (int j = 0; j < paths.Length; j++)
                {
                    if (paths[j] != null)
                    {
                        if (dungeonRooms[i].GetComponent<Collider2D>().IsTouching(paths[j].GetComponent<Collider2D>()))
                        {
                            dungeonRooms[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                        }
                    }
                }
            }
        }
    }

    void HideInactiveRooms()
    {
        for (int i=0;i<dungeonRooms.Length;i++)
        {
            //white != 255,255,255 because rounding reasons 
            //Color roomColor = new Color(255, 255, 255);
            //float green = dungeonRooms[i].GetComponent<SpriteRenderer>().color.g;
            //float blue = dungeonRooms[i].GetComponent<SpriteRenderer>().color.b;
            //float red = dungeonRooms[i].GetComponent<SpriteRenderer>().color.r;
            if (Mathf.Round(dungeonRooms[i].GetComponent<SpriteRenderer>().color.b) == 1 && Mathf.Round(dungeonRooms[i].GetComponent<SpriteRenderer>().color.g) == 1 && Mathf.Round(dungeonRooms[i].GetComponent<SpriteRenderer>().color.r) == 1)
            {
                dungeonRooms[i].SetActive(false);
            }
        }
    }

    void Update ()
    {
	    
	}
}