using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereGeneration : MonoBehaviour
{
    //Generates 1 less horizontal loop than it should, could not solve

    private int noOfVerticalLoops = 4;
    private int noOfHorizontalLoops = 8;
    private float sphereRadius = 1.0f;

    private int noOfVertices;
    private int noOfTriangles;
    private Vector3[] _vertices; //Array of vertices 
    //private Vector3[] _normals; //Array of normals
    private int[] _triangles;

    private GameObject sphereRadiusText;
    private GameObject verticalLoopsText;
    private GameObject horizontalLoopsText;

    private bool started = false;
    // Use this for initialization
    void Start()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -5);
        Render();
    }

    public void Render()
    {
        if (started)//weird addition so that it doesn't try and find menu objects before they are added
        {
            sphereRadiusText = GameObject.Find("SphereRadius");
            horizontalLoopsText = GameObject.Find("HorizontalLoops");
            verticalLoopsText = GameObject.Find("VerticalLoops");

            float fResult;
            int iResult;
            if (float.TryParse(sphereRadiusText.GetComponent<Text>().text, out fResult))
            {
                sphereRadius = fResult;
            }
            if (int.TryParse(horizontalLoopsText.GetComponent<Text>().text, out iResult))
            {
                noOfHorizontalLoops = iResult;
            }
            if (int.TryParse(verticalLoopsText.GetComponent<Text>().text, out iResult))
            {
                noOfVerticalLoops = iResult;
            }
        }
        else
        {
            started = true;
        }
        
        noOfVertices = noOfVerticalLoops * noOfHorizontalLoops + 2;
        noOfTriangles = (noOfVerticalLoops * (noOfHorizontalLoops - 1) * 2 + ((noOfVerticalLoops * 2))) * 3; //(VerticalLoops * HorizontalLoops - 1) * 2 + VerticalLoops * 2 all multiplied by 3
        //VerticalLoops *the number of verticalLoops of squares multiply by 2 to get the triangles + the top and bottom has a triangle per loop then multiply by 3 because triangles
        _vertices = new Vector3[noOfVertices];
        //_normals = new Vector3[noOfVertices];
        _triangles = new int[noOfTriangles];
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        MeshFilter _meshFilter = GetComponent<MeshFilter>();
        Mesh _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        //Define positions of vertices

        //Top and bottom vertices will always be the same, define as 0 and the last vertex in the list
        _vertices[0].Set(0, 1, 0);
        _vertices[noOfVertices-1].Set(0, -1, 0);
        //_vertices[1].Set(0, -1, 0); set to bottom
        //Define vertices around the base of the cone
        //Find the angle between the vertical loops so they are evenly spread
        float verticalLoopsAngleIncrement = 2 * Mathf.PI / noOfVerticalLoops;
        //Find the angle between the horizontal loops so they are evenly spread. Only use 1Pi as each loop is a semi circle
        float horizontalLoopsAngleIncrement = Mathf.PI / (noOfHorizontalLoops+1);
        //assigning currentAngle here as adding x each loop is more efficent than multiplying x and y
        float verticalLoopsCurrentAngle = 0;
        //Start at the 2nd position at the first is always the same
        float horizontalLoopsCurrentAngle = horizontalLoopsAngleIncrement;

        
        //We can get a direction vector to the next vertex using the angle it is at with sin and cos
        //this will be the same as the cordinates as it is starting from 0,0,0
        //I and J are counters for the array position j starts at 1 as the first vertex has already been defined
        //I counts how many horizontal loops of vertices have been made 
        for (int i = 0; i < noOfHorizontalLoops; i++, horizontalLoopsCurrentAngle+=horizontalLoopsAngleIncrement)
        {
            float yDir = Mathf.Cos(horizontalLoopsCurrentAngle);

            for (int j = 1; j < noOfVerticalLoops+1 ; j++, verticalLoopsCurrentAngle += verticalLoopsAngleIncrement) 
            {
                float sinHorizontalLoopsAngle = Mathf.Sin(horizontalLoopsCurrentAngle);
                _vertices[j+i*noOfVerticalLoops].Set(sinHorizontalLoopsAngle*Mathf.Sin(verticalLoopsCurrentAngle), yDir, sinHorizontalLoopsAngle*Mathf.Cos(verticalLoopsCurrentAngle));
            }
        }

        //set these temporarily otherwise normals cannot be set
        _mesh.vertices = _vertices;

        //As it is a sphere the normals are the same as the direction vector from the center as the center is at 0,0,0 and are calculated for a sphere of radius 1 the direction vector is the same as their cordinates
        //tl:dr vertex position = vertex normal 
        _mesh.normals = _vertices; //pass the normals to the mesh

        //Multiply the vertices matrix by the sphere radius
        if (sphereRadius != 1)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] *= sphereRadius;
            }
        }

        _mesh.vertices = _vertices;

        //Define the triangles 
        //Top and bottom sets are different to the rest
        //J is for the vertex position on the loop
        int trianglesArrayPosition = 0;
        for (int j=1; j < noOfVerticalLoops; trianglesArrayPosition += 3,  j++)
        {
            //first for each is the top vertex
            _triangles[trianglesArrayPosition] = 0;
            _triangles[trianglesArrayPosition + 1] = j;
            _triangles[trianglesArrayPosition + 2] = j+1;
        }
        //do the last triangle as it does not fit the for loop
        _triangles[trianglesArrayPosition] = 0;
        _triangles[trianglesArrayPosition + 1] = noOfVerticalLoops;
        _triangles[trianglesArrayPosition + 2] = 1;
        trianglesArrayPosition += 3;

        //loop to define the middles row of triangles
        for (int i = 0; i < noOfHorizontalLoops - 1; i++)
        {
            //number of horizontal loops gone through * the number of vertices on each horizontal loop
            //pre calculation
            int iTimesNoOfVerticalLoops = i * noOfVerticalLoops;
            int iAdd1TimesNoOfVerticalLoops = (i + 1) * noOfVerticalLoops;

            for (int j = 0; j < noOfVerticalLoops - 1; j++, trianglesArrayPosition += 6)
            {
                //add 1 to the end of all to account for the top vertex
                _triangles[trianglesArrayPosition] = j + iTimesNoOfVerticalLoops + 1;
                _triangles[trianglesArrayPosition + 1] = j + iAdd1TimesNoOfVerticalLoops + 1;
                _triangles[trianglesArrayPosition + 2] = j + 1 + iTimesNoOfVerticalLoops + 1;

                _triangles[trianglesArrayPosition + 3] = j + 1 + iTimesNoOfVerticalLoops + 1;
                _triangles[trianglesArrayPosition + 4] = j + iAdd1TimesNoOfVerticalLoops + 1;
                _triangles[trianglesArrayPosition + 5] = j + 1 + iAdd1TimesNoOfVerticalLoops + 1;
            }
            //add what doesn't fit the loop
            //j = num of vertical loops-1, j+1 = 0
            _triangles[trianglesArrayPosition] = noOfVerticalLoops + iTimesNoOfVerticalLoops;
            _triangles[trianglesArrayPosition + 1] = noOfVerticalLoops + iAdd1TimesNoOfVerticalLoops;
            _triangles[trianglesArrayPosition + 2] = iTimesNoOfVerticalLoops + 1;

            _triangles[trianglesArrayPosition + 3] = iTimesNoOfVerticalLoops + 1;
            _triangles[trianglesArrayPosition + 4] = noOfVerticalLoops + iAdd1TimesNoOfVerticalLoops;
            _triangles[trianglesArrayPosition + 5] = iAdd1TimesNoOfVerticalLoops + 1;
            trianglesArrayPosition += 6;
        }

        //make the bottom loop the same way as the top loop
        for (int j = 1; j < noOfVerticalLoops; trianglesArrayPosition += 3, j++)
        {
            //first for each is the top vertex
            _triangles[trianglesArrayPosition] = noOfVertices - 1;
            _triangles[trianglesArrayPosition + 1] = j + 1 + (noOfHorizontalLoops - 1) * noOfVerticalLoops;
            _triangles[trianglesArrayPosition + 2] = j + (noOfHorizontalLoops - 1) * noOfVerticalLoops;
        }
        //do the last triangle as it does not fit the for loop
        _triangles[trianglesArrayPosition] = noOfVertices - 1;
        _triangles[trianglesArrayPosition + 1] = 1 + (noOfHorizontalLoops - 1) * noOfVerticalLoops;
        _triangles[trianglesArrayPosition + 2] = noOfVerticalLoops + (noOfHorizontalLoops - 1) * noOfVerticalLoops;

        //pass the triangles to the mesh
        _mesh.triangles = _triangles;
        yield return null;
    }



    // Update is called once per frame
    void Update()
    {
        Quaternion temp = transform.rotation;
        temp.y += 45 * Time.deltaTime;
        transform.rotation = temp;
    }

    private void OnDrawGizmos()
    {
        if (_vertices != null)
        {
            Gizmos.color = Color.black;
            foreach (Vector3 vec3 in _vertices)
            {
                Gizmos.DrawSphere(vec3, 0.1f);
            }
        }
    }

}

