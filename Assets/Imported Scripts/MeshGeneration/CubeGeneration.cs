using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGeneration : MonoBehaviour
{
    private Vector3[] _vertices; //Array of vertices 


	// Use this for initialization
	void Start ()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -5);
        _vertices = new Vector3[8];
        StartCoroutine(Generate());
	}

    private IEnumerator Generate()
    {
        MeshFilter _meshFilter = GetComponent<MeshFilter>();
        Mesh _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        //Define positions of vertices

        _vertices[0].Set(-0.5f, -0.5f, -0.5f); //front bottom left
        _vertices[1].Set(-0.5f, 0.5f, -0.5f); //front top left
        _vertices[2].Set(0.5f, 0.5f, -0.5f); //front top right
        _vertices[3].Set(0.5f, -0.5f, -0.5f); //front bottom right
        _vertices[4].Set(-0.5f, -0.5f, 0.5f); //back bottom left
        _vertices[5].Set(-0.5f, 0.5f, 0.5f); //back top left
        _vertices[6].Set(0.5f, -0.5f, 0.5f); //back bottom right
        _vertices[7].Set(0.5f, 0.5f, 0.5f); //back top right

        _mesh.vertices = _vertices; //pass the vertices to the mesh

        //Define the triangles 
        int[] triangles = new int[36]
        {
            //front face
            0,1,3,
            1,2,3,

            //back face
            6,5,4,
            6,7,5,

            //left face
            4,5,0,
            5,1,0,

            //right face
            2,7,6,
            2,6,3,

            //top face
            1,5,2,
            5,7,2,

            //bottom face
            6,4,0,
            3,6,0
        };

        //pass the triangles to the mesh
        _mesh.triangles = triangles;
        //no normals because couldn't get them to work properly because it is not smooth
        yield return null;
    }



    // Update is called once per frame
    void Update ()
    {
        Quaternion temp = transform.rotation;
        temp.y += 45 * Time.deltaTime;
        transform.rotation = temp;
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
