using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGeneration : MonoBehaviour
{
    private int noOfRows = 60, noOfColumns = 60; //stores the number of rows and columns in the grid
    private float maxHeight = 5.0f;
    private float jaggedness = 0.25f; //taken as a number 0-1 defines how likely steep slopes are, scales with height

    private Vector3[] _vertices; //Array of vertices 
    private Color[] _colours; //Array of vertex colours

    private GameObject widthText;
    private GameObject lengthText;
    private GameObject maxHeightText;
    private GameObject jaggednessText;

    private bool started = false;
    // Use this for initialization
    void Start ()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(noOfRows*0.5f, noOfColumns*0.25f, -60);
        Render();
	}

    public void Render()
    {
        if (started)//weird addition so that it doesn't try and find menu objects before they are added
        {
            widthText = GameObject.Find("Width");
            lengthText = GameObject.Find("Breadth");
            maxHeightText = GameObject.Find("MaxHeight");
            jaggednessText = GameObject.Find("Jaggedness");

            float fResult;
            int iResult;
            if (int.TryParse(widthText.GetComponent<Text>().text, out iResult))
            {
                noOfRows = iResult;
            }
            if (int.TryParse(lengthText.GetComponent<Text>().text, out iResult))
            {
                noOfColumns = iResult;
            }
            if (float.TryParse(maxHeightText.GetComponent<Text>().text, out fResult))
            {
                maxHeight = fResult;
            }
            if (float.TryParse(jaggednessText.GetComponent<Text>().text, out fResult))
            {
                jaggedness = fResult;
            }
        }
        else
        {
            started = true;
        }

        int iTotalVertices = noOfRows * noOfColumns;
        _vertices = new Vector3[iTotalVertices];
        _colours = new Color[iTotalVertices];
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        MeshFilter _meshFilter = GetComponent<MeshFilter>();
        Mesh _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        //loop to define positions of vertices
        /*int x = 0, z = 0;
        float minimumHeight = maxHeight * -0.5f, maximumHeight = maxHeight * 0.5f;
        for(int i = 0; i<_vertices.Length; i++)
        {
            float y = Random.Range(minimumHeight, maximumHeight);
            _vertices[i].Set(x, y, z);
            _normals[i].Set(0f, 0f, -1f);
            //0-0.25 = water
            //0.25-0.5 = grass
            //0.5-0.75 = stone
            //0.75-1 = snow
            if (y < -0.25f * maxHeight)
                _colours[i] = new Color(0, 0, 1, 255);
            else if (y < 0)
                _colours[i] = new Color(0, 1, 0, 255);
            else if (y < 0.25f * maxHeight)
                _colours[i] = new Color(0.5f, 0.5f, 0.5f, 1);
            else
                _colours[i] = new Color(1, 1, 1, 1);
            
            ++x;
            if (x % (columns+1)==0)
            {
                x = 0;
                ++z;
            }
        }*/

        ///<summary>
        ///first vertex made independently
        ///if row or column 1 base height on just the adjacent vertex
        ///else take the 2 adjacent vertices and base off those +/- an amount

        for (int currentRow = 0; currentRow < noOfRows; currentRow++)
        {
            for (int currentColumn = 0; currentColumn < noOfColumns; currentColumn++)
            {
                //find neighboor's heights if they have them
                float neighboorHeight1;
                if (currentRow == 0)
                    neighboorHeight1 = -1;
                else
                    neighboorHeight1 = _vertices[vertexArrayPosition(currentRow - 1, currentColumn)].y;
                float neighboorHeight2;
                if (currentColumn == 0)
                    neighboorHeight2 = -1;
                else
                    neighboorHeight2 = _vertices[vertexArrayPosition(currentRow, currentColumn-1)].y;

                //randomise a height based off the neighboors found
                float vertexHeight;
                if (neighboorHeight1 == -1 && neighboorHeight2 == -1)
                {
                    //no neighboors 
                    vertexHeight = Random.Range(0.0f, maxHeight);
                }
                else
                {
                    float averageNeighboorHeight;
                    if(neighboorHeight1 == -1 || neighboorHeight2 == -1)
                    {
                        //only 1 neighboor add together and add 1 to correct for the -1
                        averageNeighboorHeight = neighboorHeight1 + neighboorHeight2 + 1;
                    }
                    else
                    {
                        //2 neighboors
                        averageNeighboorHeight = (neighboorHeight1 + neighboorHeight2) * 0.5f;
                    }
                    vertexHeight = Random.Range(averageNeighboorHeight - jaggedness * maxHeight, averageNeighboorHeight + jaggedness * maxHeight);
                    if (vertexHeight < 0)
                        vertexHeight = 0;
                    else if (vertexHeight > maxHeight)
                        vertexHeight = maxHeight;
                }
                _vertices[vertexArrayPosition(currentRow, currentColumn)].Set(currentRow, vertexHeight, currentColumn);
                if (vertexHeight < 0.25f * maxHeight)
                    _colours[vertexArrayPosition(currentRow, currentColumn)] = new Color(0, 0, 1, 1);
                else if (vertexHeight < 0.5f * maxHeight)
                    _colours[vertexArrayPosition(currentRow, currentColumn)] = new Color(0, 1, 0, 1);
                else if (vertexHeight < 0.75f * maxHeight)
                    _colours[vertexArrayPosition(currentRow, currentColumn)] = new Color(0.5f, 0.5f, 0.5f, 1);
                else
                    _colours[vertexArrayPosition(currentRow, currentColumn)] = new Color(1, 1, 1, 1);

            }
        }

        _mesh.vertices = _vertices; //pass the vertices to the mesh
        _mesh.colors = _colours; //pass the colours to the mesh
        _mesh.RecalculateNormals();

        //loop to define the triangles 
        //this is the most efficient method but cannot get it to work
        int numIndices = ((noOfColumns - 1) * (noOfRows - 1)) * 6;
        int[] triangles = new int[numIndices];
        //ti is the position in the array, vi is the square that is being drawn
        /*for (int ti = 0, vi = 0; vi < (noOfColumns) * (noOfRows-1)-1; vi++, ti+=6)
        {
            triangles[ti] = vi;
            triangles[ti + 1] = vi + noOfColumns;
            triangles[ti + 2] = vi + 1;
            triangles[ti + 3] = vi + 1;
            triangles[ti + 4] = vi + noOfColumns;
            triangles[ti + 5] = vi + noOfColumns + 1;

            //if vi+1 is at the edge of the terrain increase to go to the next row
            if (vi != 0 && ((vi+1) % (noOfColumns-1)) == 0)
            {
                vi++;
            }
        }*/
        int trianglesArrayPosition = 0;
        for (int currentRow = 0; currentRow < noOfRows-1; currentRow++)
        {
            for (int currentColumn = 0; currentColumn < noOfColumns-1; currentColumn++, trianglesArrayPosition+=6)
            {
                triangles[trianglesArrayPosition] = vertexArrayPosition(currentRow, currentColumn);
                triangles[trianglesArrayPosition + 1] = triangles[trianglesArrayPosition + 3] = vertexArrayPosition(currentRow, currentColumn + 1);
                triangles[trianglesArrayPosition + 2] = triangles[trianglesArrayPosition + 5] = vertexArrayPosition(currentRow + 1, currentColumn);
                triangles[trianglesArrayPosition + 4] = vertexArrayPosition(currentRow + 1, currentColumn + 1);
            }
        }

                //pass the triangles to the mesh
                _mesh.triangles = triangles;
        yield return null;
    }

    int vertexArrayPosition(int row, int column)
    {
        return (column * noOfRows + row);
    }

    // Update is called once per frame
    void Update ()
    {
        
	}

    private void OnDrawGizmos()
    {
        if (_vertices!=null)
        {
            Gizmos.color = Color.black;
            foreach(Vector3 vec3 in _vertices)
            {
                Gizmos.DrawSphere(vec3, 0.1f);
            }
        }
    }

}
