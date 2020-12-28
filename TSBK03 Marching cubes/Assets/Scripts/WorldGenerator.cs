using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class WorldGenerator : MonoBehaviour
{
 	Dictionary<Vector3Int, Chunk> chunkDict = new Dictionary<Vector3Int, Chunk>();

	private Vector3[] vertices;
	private int[] triangles;

	int chunkSize = 31;
	int numVoxels;
	int numPointsPerAxis;
	int numPoints;

	const int threadGroupSize = 8;

	[Header("Chunk settings")]
	public int chunks_xz = 10;
	public int chunks_y = 10;

	[Header("Noise settings")]
	//public int _config = -1;
	public float Frequency = 1.0f;
	public float Amplitude = 1.0f;
	public int Octaves = 3;

	[Header("Shader files")]
	public ComputeShader densityShader;
	public ComputeShader marchShader;

	private ComputeBuffer pointsBuffer; // Carries noise data
	private ComputeBuffer triangleBuffer;
	private ComputeBuffer triCountBuffer;
	private ComputeBuffer pointsBufferMarch;

	struct Triangle
    {
		public Vector3 vertex0;
		public Vector3 vertex1;
		public Vector3 vertex2;
    }

	private void Start()
	{

		GenerateChunks();
		UpdateChunks();
	}
	
	private void GenerateNoise(Vector3Int chunkPos)
	{
		float[] densityArray;

		numPointsPerAxis = chunkSize + 1;
		numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
		numVoxels = chunkSize * chunkSize * chunkSize;

		pointsBuffer = new ComputeBuffer(numPoints, sizeof(float));

		densityShader.SetBuffer(0, "densityData", pointsBuffer);
		densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
		densityShader.SetInt("Octaves", Octaves);
		densityShader.SetFloat("Amplitude", Amplitude);
		densityShader.SetFloat("Frequency", Frequency);
		densityShader.SetInt("chunkPosX", chunkPos.x);
		densityShader.SetInt("chunkPosY", chunkPos.y);
		densityShader.SetInt("chunkPosZ", chunkPos.z);

		densityShader.Dispatch(0, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize, numPointsPerAxis / threadGroupSize);

		densityArray = new float[numPoints];
		pointsBuffer.GetData(densityArray);
		chunkDict[chunkPos].densityArray = densityArray;
		pointsBuffer.Release();

	}
	private void MarchCubes(float[] densityArray)
	{
	
		pointsBufferMarch = new ComputeBuffer(numPoints, sizeof(float));
		pointsBufferMarch.SetData(densityArray);

		// Number of triangles per voxel is 5 max -> total is numVoxels*5
		triangleBuffer = new ComputeBuffer(numVoxels * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
		triCountBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);


		triangleBuffer.SetCounterValue(0);
		marchShader.SetBuffer(0, "densityData", pointsBufferMarch);
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
		//Debug.Log("Vertex count: " + numTris);

		vertices = new Vector3[numTris * 3];
		Triangle[] tris = new Triangle[numTris];

		triangleBuffer.GetData(tris, 0, 0, numTris);

		triangles = new int[numTris * 3];

		for (int i = 0; i < numTris; i++)
		{
			triangles[i * 3 + 0] = i * 3 + 0;
			triangles[i * 3 + 1] = i * 3 + 1;
			triangles[i * 3 + 2] = i * 3 + 2;
			vertices[i * 3 + 0] = tris[i].vertex0;
			vertices[i * 3 + 1] = tris[i].vertex1;
			vertices[i * 3 + 2] = tris[i].vertex2;

		}

		pointsBufferMarch.Release();
		triangleBuffer.Release();
		triCountBuffer.Release();
	}

	void OnValidate()
	{
		UpdateChunks();
	}

	Mesh BuildMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		return mesh;
	}

	void GenerateChunks()
	{
		for (int x = 0; x < chunks_xz; x++)
		{
			for (int y = 0; y < chunks_y; y++)
			{
				for (int z = 0; z < chunks_xz; z++)
				{
					Vector3Int chunkPos = new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize);
					chunkDict.Add(chunkPos, new Chunk(chunkPos));
					chunkDict[chunkPos].chunkObject.transform.SetParent(transform);
					chunkDict[chunkPos].densityArray = new float[numPoints];
					chunkDict[chunkPos].meshRenderer.material = GetComponent<Renderer>().material;
				}
			}
		}
	}
	void UpdateChunks()
	{
		foreach (KeyValuePair<Vector3Int, Chunk> i in chunkDict)
		{
			GenerateNoise(i.Key);
			MarchCubes(i.Value.densityArray);
			Mesh mesh = BuildMesh();
			i.Value.meshFilter.mesh = mesh;
			i.Value.meshCollider.sharedMesh = i.Value.meshFilter.sharedMesh;
		}
	}
	void UpdateChunk(Vector3Int chunkPos)
	{
		MarchCubes(chunkDict[chunkPos].densityArray);
		Mesh mesh = BuildMesh();
		chunkDict[chunkPos].meshFilter.mesh = mesh;
		chunkDict[chunkPos].meshCollider.sharedMesh = chunkDict[chunkPos].meshFilter.sharedMesh;
	}
	public void EditTerrain(Vector3 pointHit, float radius, float newDensityVal)
	{
		
		Vector3Int chunkPos = new Vector3Int((int)(pointHit.x/chunkSize)*chunkSize, (int)(pointHit.y / chunkSize) * chunkSize, (int)(pointHit.z / chunkSize)*chunkSize);
		int rad = Mathf.FloorToInt(radius);
		Vector3Int pointHitInt = new Vector3Int(Mathf.CeilToInt(pointHit.x), Mathf.CeilToInt(pointHit.y), Mathf.CeilToInt(pointHit.z));

		for (int x = -rad; x <= rad; x++)
		{
			for (int y = -rad; y <= rad; y++)
			{
				for (int z = -rad; z <= rad; z++)
				{
					
					if (chunkDict.TryGetValue(chunkPos, out Chunk chunk))
					{
						Vector3Int worldCoord = pointHitInt;
						worldCoord = worldCoord - new Vector3Int(chunkPos.x+x, chunkPos.y + y, chunkPos.z + z);
						int index = indexFromCoord(worldCoord.x, worldCoord.y, worldCoord.z, numPointsPerAxis);

						if(newDensityVal > 0)
							chunkDict[chunkPos].densityArray[index] = Vector3Int.Distance(worldCoord, pointHitInt);
						else
							chunkDict[chunkPos].densityArray[index] = -Vector3Int.Distance(worldCoord, pointHitInt);


					}
				}
			}

		}
		UpdateChunk(chunkPos);
	}

	int indexFromCoord(int x, int y, int z, int w)
	{
		return x + w * (y + w * z);
		//return z * w * w + y * w + x;
	}
}
