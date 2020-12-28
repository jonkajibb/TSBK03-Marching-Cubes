using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{
    Camera camera;
    public float editRadius;
    public WorldGenerator world;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        world = GameObject.Find("Mesh Parent").GetComponent<WorldGenerator>();


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            var ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit , 50f))
            {
                Debug.Log("Jonkajibb");
                world.EditTerrain(hit.point, editRadius, 1.0f);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            var ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50f))
            {
                Debug.Log("Jonkajibb2");
                world.EditTerrain(hit.point, editRadius, -1.0f);
            }
        }
    }
}
