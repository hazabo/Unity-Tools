using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MazeAndPathFinding : MonoBehaviour {
    private int mazeWidth = 10;
    private int mazeHeight = 10;
    private GameObject tilePrefab;

    private int pathStart = 11;
    private int pathEnd = 88;

    //primms variables
    private bool[] tiles; //false for wall true for path
    private List<int> closedList;
    private List<int> frontierSpaces;
    private GameObject[] maze;

    //AStar variables
    private List<int> openSet;
    private List<int> closedSet;
    private node[] mazeNodes;
    private int savedPathStart;
    private int savedPathEnd;

    private GameObject mazeWidthText;
    private GameObject mazeHeightText;
    private GameObject pathStartText;
    private GameObject pathEndText;

    private bool started = false;
    private bool started2 = false;

    private bool pathFound = false;

    private struct node
    {
        public int cameFrom;
        public int gScore; //distance travelled to get to this node
        public int hScore; //distance from this node to the end ignoring walls
        public int fScore; //distance from current node to end node + gScore
    }


    // Use this for initialization
    void Start ()
    {
        
        tilePrefab = Resources.Load<GameObject>("MazeSpace");
        Generate();
        PathFind();
    }

    public void Generate()
    {
        if(maze!=null)
        {
            foreach(GameObject space in maze)
            {
                Destroy(space);
            }
        }
        if (started)//weird addition so that it doesn't try and find menu objects before they are added
        {
            mazeWidthText = GameObject.Find("MazeWidth");
            mazeHeightText = GameObject.Find("MazeHeight");
            pathStartText = GameObject.Find("PathFindingStart");
            pathEndText = GameObject.Find("PathFindingEnd");

            int iResult;
            if (int.TryParse(mazeWidthText.GetComponent<Text>().text, out iResult))
            {
                mazeWidth = iResult;
            }
            if (int.TryParse(mazeHeightText.GetComponent<Text>().text, out iResult))
            {
                mazeHeight = iResult;
            }
        }
        else
        {
            started = true;
        }

        GameObject.Find("Main Camera").transform.position = new Vector3(mazeWidth * 0.5f - 0.5f, mazeHeight * 0.5f - 0.5f, (mazeHeight > mazeWidth ? (mazeHeight * -1) : (mazeWidth * -1)));

        GenerateMaze();
    }

    public void PathFind()
    {
        if (started2)
        {
            int iResult;
            if (int.TryParse(pathStartText.GetComponent<Text>().text, out iResult))
            {
                pathStart = iResult;
            }
            if (int.TryParse(pathEndText.GetComponent<Text>().text, out iResult))
            {
                pathEnd = iResult;
            }
            if (pathFound)
            {
                ResetAStar();
            }
        }
        else
        {
            started2 = true;
        }
        pathFound = true;
        InitialiseAStar();
        AStar(pathStart, pathEnd);
    }

    void GenerateMaze()
    {
        int mazeWidthHeightProduct = mazeWidth * mazeHeight;
        //initialise the lists
        closedList = new List<int>();
        frontierSpaces = new List<int>();
        tiles = new bool[mazeWidthHeightProduct];
        //set all tile to false 
        for (int i = 0; i < tiles.Length; i++)
        {

            tiles[i] = false;
        }
        //edge of maze should be added to the closed list as walls 
        //so exceptions do not have to be added to the loop for when its at the edge
        //bottom
        for (int i = 0; i < mazeWidth; i++)
        {
            closedList.Add(i);
        }
        //top
        for (int i = mazeWidthHeightProduct - 1; i > mazeWidthHeightProduct - mazeWidth; i--)
        {
            closedList.Add(i);
        }
        //left
        for (int i = mazeWidth; i < mazeWidthHeightProduct; i += mazeWidth)
        {
            closedList.Add(i);
        }
        //right
        for (int i = mazeWidth * 2 - 1; i < mazeWidthHeightProduct; i += mazeWidth)
        {
            closedList.Add(i);
        }


        //do the first step outside of the loop to initialise
        tiles[mazeWidth + 1] = true;
        closedList.Add(mazeWidth + 1);
        frontierSpaces.Add(mazeWidth + 2);
        frontierSpaces.Add(mazeWidth * 2 + 1);
        while (frontierSpaces.Count != 0)
        {
            ///summary
            ///pick one of frontier spaces if 2 or more of its 
            ///neighboors are spaces make it a wall else make it a space
            ///then add it the used list and add its neighboors to the frontier spaces if they are not an used or frontier space

            //pick 
            int randomIndex = Random.Range(0, frontierSpaces.Count - 1);
            int currentArrayIndex = frontierSpaces[randomIndex];
            frontierSpaces.RemoveAt(randomIndex);
            Vector2 currentPosition = arrayIndexToPostion(currentArrayIndex);
            //neighboors should not be added to the frontier list if it is a wall
            if (makeWall(currentPosition))
            {
                tiles[currentArrayIndex] = true;
                closedList.Add(currentArrayIndex);

                int nextTileIndex = currentArrayIndex + 1;
                if (!closedList.Contains(nextTileIndex) && !frontierSpaces.Contains(nextTileIndex))
                    frontierSpaces.Add(nextTileIndex);
                nextTileIndex = currentArrayIndex - 1;
                if (!closedList.Contains(nextTileIndex) && !frontierSpaces.Contains(nextTileIndex))
                    frontierSpaces.Add(nextTileIndex);
                nextTileIndex = currentArrayIndex + mazeWidth;
                if (!closedList.Contains(nextTileIndex) && !frontierSpaces.Contains(nextTileIndex))
                    frontierSpaces.Add(nextTileIndex);
                nextTileIndex = currentArrayIndex - mazeWidth;
                if (!closedList.Contains(nextTileIndex) && !frontierSpaces.Contains(nextTileIndex))
                    frontierSpaces.Add(nextTileIndex);
            }
        }
        maze = new GameObject[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            Color tileColour;
            if (tiles[i])
                tileColour = new Color(1, 1, 1, 1);
            else
                tileColour = new Color(0, 0, 0, 1);
            maze[i] = (GameObject)Instantiate(tilePrefab, arrayIndexToPostion(i), Quaternion.identity);
            maze[i].GetComponent<SpriteRenderer>().color = tileColour;
        }
    }

    //this method will make a normal maze
    
    bool makeWall(Vector2 position)
    {
        int spacesFound = 0;
        //need to make sure not to test things that don't exist
        //we can ignore the case where index is 0 
        if (position.x < mazeWidth - 1)
            if (tiles[positionToArrayIndex((int)position.x + 1, (int)position.y)] == true)
                spacesFound++;
        if (position.x > 0)
            if (tiles[positionToArrayIndex((int)position.x - 1, (int)position.y)] == true)
                spacesFound++;
        if (position.y < mazeHeight - 1)
            if (tiles[positionToArrayIndex((int)position.x, (int)position.y + 1)] == true)
                spacesFound++;
        if (position.y > 0)
            if (tiles[positionToArrayIndex((int)position.x, (int)position.y - 1)] == true)
                spacesFound++;
        if (spacesFound<2)
            return true;
        else
            return false;
    }
    
    //this method makes a connected maze but the paths are wide 
    //kinda good for dungeons, makes rooms as well as paths
    bool makeWall2(Vector2 position)
    {
        bool northIsSpace, southIsSpace, eastIsSpace, westIsSpace ;
        if (position.x < mazeWidth - 1)
            eastIsSpace = tiles[positionToArrayIndex((int)position.x + 1, (int)position.y)];
        else
            eastIsSpace = false;
        if (position.x > 0)
            westIsSpace = tiles[positionToArrayIndex((int)position.x - 1, (int)position.y)];
        else
            westIsSpace = false;
        if (position.y < mazeHeight - 1)
            northIsSpace = tiles[positionToArrayIndex((int)position.x, (int)position.y + 1)];
        else
            northIsSpace = false;
        if (position.y > 0)
            southIsSpace = tiles[positionToArrayIndex((int)position.x, (int)position.y - 1)];
        else
            southIsSpace = false;

        if (northIsSpace && southIsSpace || eastIsSpace && westIsSpace)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int positionToArrayIndex(int x, int y)
    {
        return (y * mazeWidth + x);
    }

    Vector2 arrayIndexToPostion(int index)
    {
        int y = Mathf.FloorToInt(index / mazeWidth);
        return new Vector2((index - y * mazeWidth), y);
    }

    // Update is called once per frame
    void Update()
    {
        if (savedPathEnd != pathEnd || savedPathStart != pathStart)
        {
            ResetAStar();
            AStar(pathStart, pathEnd);
        }
    }

    void InitialiseAStar()
    {
        openSet = new List<int>();
        closedSet = new List<int>();
        mazeNodes = new node[mazeWidth * mazeHeight];
        for (int i = 0; i < mazeNodes.Length; i++)
        {
            Vector2 endPosition = arrayIndexToPostion(pathEnd);
            if (tiles[i] == false)
            {
                closedSet.Add(i); //add all walls to the closed list, this means they will be ignored in the path finding
            }
            else
            {
                Vector2 distance = endPosition - arrayIndexToPostion(i);
                mazeNodes[i].hScore = (int)(Mathf.Abs(distance.x) + Mathf.Abs(distance.y));
                mazeNodes[i].gScore = -1;
            }
        }
    }

    void ResetAStar()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if(maze[i].GetComponent<SpriteRenderer>().color == new Color(1,0,0,1))
            {
                maze[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
        InitialiseAStar();
    }

    void AStar(int start, int end) /////if end or start is a wall do nothing and output "path impossible"
    {
        savedPathEnd = pathEnd;
        savedPathStart = pathStart;
        //if the start or end are walls do not path find
        if (tiles[pathEnd] == false || tiles[pathStart] == false)
            return;        
        openSet.Add(start);
        mazeNodes[start].gScore = 0;
        Vector2 distanceStartToEnd = arrayIndexToPostion(start) - arrayIndexToPostion(end) ;
        mazeNodes[start].fScore = (int)(Mathf.Abs(distanceStartToEnd.x) + Mathf.Abs(distanceStartToEnd.y));
        while (openSet.Count>0)
        {
            int current = FindLowestOpenSetFScore();//lowest fscore from mazeNodes
            if (current == end)
                break;
            openSet.Remove(current);
            closedSet.Add(current);
            DoNeighboorStuff(current, current + 1);
            DoNeighboorStuff(current, current - 1);
            DoNeighboorStuff(current, current + mazeWidth);
            DoNeighboorStuff(current, current - mazeWidth);

        }
        ReconstructPath(pathEnd);
    }

    int FindLowestOpenSetFScore()
    {
        int lowestFound = mazeNodes[openSet[0]].fScore;
        int lowestFoundAt = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if(mazeNodes[i].fScore>lowestFound)
            {
                lowestFound = mazeNodes[i].fScore;
                lowestFoundAt = i;
            }
        }
        return lowestFoundAt;
    }

    void DoNeighboorStuff(int current, int neighboor)
    {
        //if neighboor has been added to the closed list its fscore must be lower than or equal to current's
        if(!closedSet.Contains(neighboor))
        {
            if ((mazeNodes[neighboor].gScore > mazeNodes[current].gScore + 1) || (mazeNodes[neighboor].gScore == -1)) //if want to use weighted paths adjust here
            {
                mazeNodes[neighboor].gScore = mazeNodes[current].gScore + 1;
                mazeNodes[neighboor].cameFrom = current;
                mazeNodes[neighboor].fScore = mazeNodes[neighboor].gScore + mazeNodes[neighboor].hScore;
                if (!openSet.Contains(neighboor))
                {
                    openSet.Add(neighboor);
                }

            }
        }
    }

    void ReconstructPath(int endPoint)
    {
        int currentPos = endPoint;
        while(mazeNodes[currentPos].cameFrom!=0)
        {
            maze[currentPos].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            currentPos = mazeNodes[currentPos].cameFrom;
        }
        //the loop will be exited before changing the colour of the start square
        maze[currentPos].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

    void OnDestroy()
    {
        foreach(GameObject space in maze)
        {
            Destroy(space);
        }
    }
}
