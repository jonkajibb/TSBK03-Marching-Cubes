using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	GameObject chunkObject;
	MeshRenderer meshRenderer;
	MeshCollider meshCollider;
	public MeshFilter meshFilter;
	Vector3Int chunkPosition;

	public Chunk(Vector3Int _position)
	{
		chunkObject = new GameObject();
		chunkPosition = _position;
		chunkObject.transform.position = chunkPosition;

		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshCollider = chunkObject.AddComponent<MeshCollider>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Diffuse"));
	}
}
