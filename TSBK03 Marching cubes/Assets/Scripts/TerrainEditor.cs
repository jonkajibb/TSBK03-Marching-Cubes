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
        //gameobject.transform.Find("ChildName");

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
    /*public Camera cam;
    public WorldGenerator world;

    private void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        cam.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                    world.GetChunkFromVector3(hit.transform.position).world.PlaceTerrain(hit.point);

            }

        }

        if (Input.GetMouseButtonDown(1))
        {

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Terrain")
                    world.GetChunkFromVector3(hit.transform.position).RemoveTerrain(hit.point);

            }

        }

    }*/
}
