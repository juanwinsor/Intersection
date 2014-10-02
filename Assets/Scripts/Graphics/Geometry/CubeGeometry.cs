using UnityEngine;
using System.Collections;

public class CubeGeometry : MonoBehaviour {
	
	public Material cubeMaterial;
	
	private float size = 1;
	
	void Awake()
	{
		Mesh mesh = new Mesh();
		mesh.name = "cube";
		mesh.subMeshCount = 6;
		
		Vector3 ul_front = new Vector3(-size / 2, size / 2, size / 2);
		Vector3 ur_front = new Vector3(size / 2, size / 2, size / 2);
		Vector3 lr_front = new Vector3(size / 2, -size / 2, size / 2);
		Vector3 ll_front = new Vector3(-size / 2, -size / 2, size / 2);
		
		Vector3 ul_rear = new Vector3(-size / 2, size / 2, -size / 2);
		Vector3 ur_rear = new Vector3(size / 2, size / 2, -size / 2);
		Vector3 lr_rear = new Vector3(size / 2, -size / 2, -size / 2);
		Vector3 ll_rear = new Vector3(-size / 2, -size / 2, -size / 2);
			
		//each triangle needs unique verts for proper lighting
		Vector3[] verts = new Vector3[24] {
			//front face verts
			ul_front, //upper left front	0
			ur_front, //upper right front	1
			lr_front, //lower right front	2
			ll_front, //lower left front	3
			//rear face verts
			ul_rear, //upper left rear		4
			ur_rear, //upper right rear		5
			lr_rear, //lower right rear		6
			ll_rear, //lower left rear		7
			//left face verts
			ul_rear, //		8
			ul_front, //	9
			ll_front, //	10
			ll_rear, //		11
			//right face verts
			ur_front, //	12
			ur_rear, //		13
			lr_rear, //		14
			lr_front, //	15
			//top face verts
			ul_front, //	16
			ur_front, //	17
			ur_rear, //		18
			ul_rear, //		19
			//bottom face verts
			ll_front, //	20
			lr_front, //	21
			lr_rear, //		22
			ll_rear //		23
		};
		
		mesh.vertices = verts;
		
		//set uv coordinates
		mesh.uv = new Vector2[24]{
			//front face
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
			new Vector2(0,1),
			//rear face
			new Vector2(1,0),
			new Vector2(0,0),
			new Vector2(0,1),
			new Vector2(1,1),
			//left face
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
			new Vector2(0,1),
			//right face
			new Vector2(1,0),
			new Vector2(0,0),
			new Vector2(0,1),
			new Vector2(1,1),
			//top face
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
			new Vector2(0,1),
			//bottom face
			new Vector2(1,0),
			new Vector2(0,0),
			new Vector2(0,1),
			new Vector2(1,1)
		};
		
		//create each face of the cube as its own submesh, this will allow a material per face
		
		//front face submesh
		mesh.SetTriangles(new int[]{			
			1,0,3,
			1,3,2,
		}, 0);		
		//rear face submesh
		mesh.SetTriangles(new int[]{			
			5,7,4,
			5,6,7
		}, 1);
		//left face submesh
		mesh.SetTriangles(new int[]{			
			9,8,11,
			9,11,10
		}, 2);
		//right face submesh
		mesh.SetTriangles(new int[]{			
			12,15,13,
			15,14,13
		}, 3);
		//top face submesh
		mesh.SetTriangles(new int[]{			
			16,17,19,
			17,18,19
		}, 4);		
		//bottom face submesh
		mesh.SetTriangles(new int[]{			
			20,23,22,
			20,22,21
		}, 5);
		
		//compute normals
		mesh.RecalculateNormals();
		
		//calculate tangents
		calculateMeshTangents(mesh);
		
		mesh.Optimize();
		
		//set the new mesh into the meshfilter
		MeshFilter meshFilter = (MeshFilter)this.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		
		//setup materials for cube
		Material[] mats = new Material[6];
		for(int i = 0; i < 6; i++)
		{
			//set the material for all sides
			mats[i] = (Material)Instantiate(cubeMaterial);

			//this.GetComponent<MeshRenderer>().renderer.materials[i].color = new Color(Random.value, Random.value, Random.value, 1);
		}		
		this.GetComponent<MeshRenderer>().renderer.materials = mats;
		
	}
	
	
	
	//http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
	public static void calculateMeshTangents(Mesh mesh)
	{
	    //speed up math by copying the mesh arrays
	    int[] triangles = mesh.triangles;
	    Vector3[] vertices = mesh.vertices;
	    Vector2[] uv = mesh.uv;
	    Vector3[] normals = mesh.normals;
	 
	    //variable definitions
	    int triangleCount = triangles.Length;
	    int vertexCount = vertices.Length;
	 
	    Vector3[] tan1 = new Vector3[vertexCount];
	    Vector3[] tan2 = new Vector3[vertexCount];
	 
	    Vector4[] tangents = new Vector4[vertexCount];
	 
	    for (long a = 0; a < triangleCount; a += 3)
	    {
	        long i1 = triangles[a + 0];
	        long i2 = triangles[a + 1];
	        long i3 = triangles[a + 2];
	 
	        Vector3 v1 = vertices[i1];
	        Vector3 v2 = vertices[i2];
	        Vector3 v3 = vertices[i3];
	 
	        Vector2 w1 = uv[i1];
	        Vector2 w2 = uv[i2];
	        Vector2 w3 = uv[i3];
	 
	        float x1 = v2.x - v1.x;
	        float x2 = v3.x - v1.x;
	        float y1 = v2.y - v1.y;
	        float y2 = v3.y - v1.y;
	        float z1 = v2.z - v1.z;
	        float z2 = v3.z - v1.z;
	 
	        float s1 = w2.x - w1.x;
	        float s2 = w3.x - w1.x;
	        float t1 = w2.y - w1.y;
	        float t2 = w3.y - w1.y;
	 
	        float r = 1.0f / (s1 * t2 - s2 * t1);
	 
	        Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
	        Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	 
	        tan1[i1] += sdir;
	        tan1[i2] += sdir;
	        tan1[i3] += sdir;
	 
	        tan2[i1] += tdir;
	        tan2[i2] += tdir;
	        tan2[i3] += tdir;
	    }
	 
	 
	    for (long a = 0; a < vertexCount; ++a)
	    {
	        Vector3 n = normals[a];
	        Vector3 t = tan1[a];
	 
	        //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
	        //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
	        Vector3.OrthoNormalize(ref n, ref t);
	        tangents[a].x = t.x;
	        tangents[a].y = t.y;
	        tangents[a].z = t.z;
	 
	        tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
	    }
	 
	    mesh.tangents = tangents;
	}
}
