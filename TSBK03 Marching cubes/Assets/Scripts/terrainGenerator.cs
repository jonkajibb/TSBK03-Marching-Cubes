using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainGenerator : MonoBehaviour
{
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();

	float terrainSurface = 0.5f;
	int width = 32;
	int height = 8;
	float[,,] terrainMap;

	Marching marching = GameObject.GetComponent<Marching>();

	private void Start()
	{

		meshFilter = GetComponent<MeshFilter>();
		terrainMap = new float[width + 1, height + 1, width + 1];
		PopulateTerrainMap();
		CreateMeshData();

	}

	void CreateMeshData()
	{

		ClearMeshData();

		// Loop through each "cube" in our terrain.
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				for (int z = 0; z < width; z++)
				{

					// Create an array of floats representing each corner of a cube and get the value from our terrainMap.
					float[] cube = new float[8];
					for (int i = 0; i < 8; i++)
					{
						Vector3Int corner = new Vector3Int(x, y, z) + CornerTable[i];
						cube[i] = terrainMap[corner.x, corner.y, corner.z];
					}

					// Pass the value into our MarchCube function.

					marching.MarchCube(new Vector3(x, y, z), cube);

				}
			}
		}

		BuildMesh();

	}

	void PopulateTerrainMap()
	{

		// The data points for terrain are stored at the corners of our "cubes", so the terrainMap needs to be 1 larger
		// than the width/height of our mesh.
		for (int x = 0; x < width + 1; x++)
		{
			for (int z = 0; z < width + 1; z++)
			{
				for (int y = 0; y < height + 1; y++)
				{

					// Get a terrain height using regular old Perlin noise.
					float thisHeight = (float)height * Mathf.PerlinNoise((float)x / 16f * 1.5f + 0.001f, (float)z / 16f * 1.5f + 0.001f);

					float point = 0;
					// We're only interested when point is within 0.5f of terrain surface. More than 0.5f less and it is just considered
					// solid terrain, more than 0.5f above and it is just air. Within that range, however, we want the exact value.
					if (y <= thisHeight - 0.5f)
						point = 0f;
					else if (y > thisHeight + 0.5f)
						point = 1f;
					else if (y > thisHeight)
						point = (float)y - thisHeight;
					else
						point = thisHeight - (float)y;

					// Set the value of this point in the terrainMap.
					terrainMap[x, y, z] = point;

				}
			}
		}
	}

	void ClearMeshData()
	{

		vertices.Clear();
		triangles.Clear();

	}

	void BuildMesh()
	{

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;

	}

}
