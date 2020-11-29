using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	bool isUpdated = false;
	MeshFilter meshFilter;

	//List<Vector3> vertices = new List<Vector3>();
	//List<int> triangles = new List<int>();
	private float[] densityArray;
	private Vector3[] vertices;
	private int[] triangles;

	public int chunkSize = 16;
	int numVoxels;
	int numPointsPerAxis;
	int numPoints;

	const int threadGroupSize = 8;
	int kernel;

	public int _config = -1;
	public float Frequency = 1.0f;
	public float Amplitude = 1.0f;
	public int Octaves = 3;

	public ComputeShader densityShader;
	public ComputeShader marchShader;

	private ComputeBuffer pointsBuffer; // Carries noise data
	private ComputeBuffer triangleBuffer;
	private ComputeBuffer triCountBuffer;

	struct Triangle
    {
		public Vector3 vertex0;
		public Vector3 vertex1;
		public Vector3 vertex2;
    }

	private void Start()
	{
		//kernel = shader.FindKernel("CSMain");

		// Buffer for the noise values in a chunk

		meshFilter = GetComponent<MeshFilter>();

		Run();
	}

	private void Run()
	{
		numPointsPerAxis = chunkSize + 1; 
		numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
		numVoxels = chunkSize * chunkSize * chunkSize;

		pointsBuffer = new ComputeBuffer(numPoints, sizeof(float));

		densityShader.SetBuffer(0, "densityData", pointsBuffer);
		densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
		densityShader.SetInt("Octaves", Octaves);
		densityShader.SetFloat("Amplitude", Amplitude);
		densityShader.SetFloat("Frequency", Frequency);

		densityShader.Dispatch(0, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize);

		densityArray = new float[numPoints];

		pointsBuffer.GetData(densityArray);
		Debug.Log(densityArray[0]);

		// Number of triangles per voxel is 5 max -> total is numVoxels*5
		triangleBuffer = new ComputeBuffer(numVoxels * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
		triCountBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);


		triangleBuffer.SetCounterValue(0);
		marchShader.SetBuffer(0, "densityData", pointsBuffer);
		marchShader.SetBuffer(0, "triangleBuffer", triangleBuffer);
		marchShader.SetInt("chunkSize", chunkSize);
		marchShader.SetInt("numPointsPerAxis", numPointsPerAxis);

		marchShader.Dispatch(0, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize);
		//marchShader.Dispatch(0, 1, 1, 1);

		int[] triCountArray = { 0, 1, 0, 0 };
		triCountBuffer.SetData(triCountArray);

		ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
		triCountBuffer.GetData(triCountArray);
		int numTris = triCountArray[0];

		//numVerts *= 3;
		Debug.Log("Vertex count: " + numTris);

		vertices = new Vector3[numTris * 3];
		Triangle[] tris = new Triangle[numTris];

		triangleBuffer.GetData(tris, 0, 0, numTris);

		triangles = new int[numTris * 3];

		for (int i = 0; i < numTris; i++)
		{
			//for (int j = 0; j < 3; j++)
			//{
			//	triangles[i * 3 + j] = i * 3 + j;
			//	vertices[i * 3 + 0] = tris[i].vertex0;
			//	vertices[i * 3 + 1] = tris[i].vertex1;
			//	vertices[i * 3 + 2] = tris[i].vertex2;
			//}
			triangles[i * 3 + 0] = i * 3 + 0;
			triangles[i * 3 + 1] = i * 3 + 1;
			triangles[i * 3 + 2] = i * 3 + 2;
			vertices[i * 3 + 0] = tris[i].vertex0;
			vertices[i * 3 + 1] = tris[i].vertex1;
			vertices[i * 3 + 2] = tris[i].vertex2;

		}
		//ClearMesh();
		//generateTerrain();
		//CreateMesh();
		BuildMesh();
		
		pointsBuffer.Release();
		triangleBuffer.Release();
		triCountBuffer.Release();

		isUpdated = false;
	}

	void ClearMesh()
	{
		//vertices.;
		//triangles.Clear();
	}

	void OnValidate()
	{
		Run();
	}

	void BuildMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;

		Debug.Log(mesh.vertexCount);
	}


}
