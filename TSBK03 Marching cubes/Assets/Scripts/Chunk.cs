using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	public GameObject chunkObject;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	public MeshFilter meshFilter;
	public Vector3Int chunkPosition;
	//public Material material;
	const int size = 100000;
	public float[] densityArray;

	Material triplanarMat;

	public Chunk(Vector3Int position)
	{
		chunkObject = new GameObject();
		chunkObject.name = string.Format("Chunk {0}, {1}, {2}", position.x, position.y, position.z);
		chunkPosition = position;
		chunkObject.transform.position = chunkPosition;

		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshCollider = chunkObject.AddComponent<MeshCollider>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();

		meshRenderer.material = new Material(Shader.Find("Diffuse"));
		//meshRenderer.material = material;

		densityArray = new float[size];
	}
}
