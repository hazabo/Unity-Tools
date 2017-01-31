using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeGeneration : MonoBehaviour
{
    private int noOfFaces = 6; //Number of faces on the cone ignoring the bottom of the cone
    private float baseRadius = 1.0f;
    private float coneHeight = 1.5f;

    private int noOfVertices;
    private int noOfTriangles;
    private Vector3[] _vertices; //Array of vertices 
    private Vector3[] _normals; //Array of normals
    private int[] triangles;

    private GameObject noOfFacesText;
    private GameObject baseRadiusText;
    private GameObject coneHeightText;

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
            noOfFacesText = GameObject.Find("Smoothness");
            baseRadiusText = GameObject.Find("BaseRadius");
            coneHeightText = GameObject.Find("ConeHeight");
        
            float fResult;
            int iResult;
            if (int.TryParse(noOfFacesText.GetComponent<Text>().text, out iResult))
            {
                noOfFaces = iResult;
            }
            if (float.TryParse(baseRadiusText.GetComponent<Text>().text, out fResult))
            {
                baseRadius = fResult;
            }
            if (float.TryParse(coneHeightText.GetComponent<Text>().text, out fResult))
            {
                coneHeight = fResult;
            }
        }
        else
        {
            started = true;
        }
        
        noOfVertices = noOfFaces + 3;
        noOfTriangles = noOfFaces * 6;//faces 2 * 3
        _vertices = new Vector3[noOfVertices];
        _normals = new Vector3[noOfVertices];
        triangles = new int[noOfTriangles];
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        MeshFilter _meshFilter = GetComponent<MeshFilter>();
        Mesh _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        //Define positions of vertices
        //Top and bottom vertices will always be the same, define as 0 and 1 respectively
        _vertices[0].Set(0, coneHeight, 0);
        _normals[0].Set(0, 1, 0);
        _vertices[1].Set(0, 0, 0);
        _normals[1].Set(0, -1, 0);
        //Define vertices around the base of the cone
        float angleIncrement = 2*Mathf.PI / noOfFaces;
        //assigning currentAngle as adding x each loop is more efficent than multiplying x and y
        float currentAngle = 0.0f;
        //We can get a direction vector to the next vertice using the angle it is at with sin and cos
        //this will be the same as the cordinates as it is starting from 0,0,0
        for (int i = 2; i < noOfVertices - 1; i++) 
        {
            float xDir = Mathf.Sin(currentAngle);
            float yDir = Mathf.Cos(currentAngle);
            _vertices[i].Set(xDir * baseRadius, 0, yDir * baseRadius);
            _normals[i].Set(xDir, 0, yDir);

            currentAngle += angleIncrement;
        }


        _mesh.vertices = _vertices; //pass the vertices to the mesh
        _mesh.normals = _normals; //pass the normals to the mesh

        //Define the triangles 
        //i defines the position in the array, j is which base point is being used
        for (int i=0, j=2; i < noOfTriangles-6; i += 6, j++)
        {
            //define the triangle from 2 base points to the top
            triangles[i] = 0;
            triangles[i + 1] = j;
            triangles[i + 2] = j+1;

            //define the triangle from 2 base points to the bottom
            triangles[i + 3] = 1;
            triangles[i + 4] = j+1;
            triangles[i + 5] = j;
        }
        //last 2 triangles do not work with the loop
        triangles[noOfTriangles - 6] = 0;
        triangles[noOfTriangles - 5] = noOfVertices-2;
        triangles[noOfTriangles - 4] = 2;

        triangles[noOfTriangles - 3] = 1;
        triangles[noOfTriangles - 2] = 2;
        triangles[noOfTriangles - 1] = noOfVertices-2;


        //pass the triangles to the mesh
        _mesh.triangles = triangles;
        //_mesh.RecalculateNormals();


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
