using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public int Chunks = 10;
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    void Start()
    {
        Generate();
    }

    void Generate(){
        for(int x = 0; x < Chunks; x++)
        { 
          for(int y = 0; y < Chunks; y++)
                {
            for(int z = 0; z < Chunks; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x*GameData.chunkSize, y*GameData.chunkSize, z*GameData.chunkSize);
                chunks.Add(chunkPos, new Chunk(chunkPos));
            }
            }
        }

    }
}
