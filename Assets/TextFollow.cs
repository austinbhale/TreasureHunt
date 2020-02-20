using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollow : MonoBehaviour
{
    Camera camera;
    public int forwardOffset = 10;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("TreasureHunter").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set transform forward and rotate to look at target camera.
        transform.position = camera.transform.position + camera.transform.forward * forwardOffset;
        transform.LookAt(camera.transform);
        transform.Rotate(0,180,0);
    }
}