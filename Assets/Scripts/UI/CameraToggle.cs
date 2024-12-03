using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] Camera cabin;
    [SerializeField] Camera drone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Cam Toggle");
            if (cabin.rect.width > 0.9f)
            {
                Toggle(drone, cabin);
            }
            else
            {
                Toggle(cabin, drone);
            }
        }
    }

    void Toggle(Camera cam1, Camera cam2)
    {
        cam1.rect = new Rect(0, 0, 1, 1);
        cam1.depth = -1;
        cam2.rect = new Rect(0.75f, 0.75f, 0.25f, 0.25f);
        cam2.depth = 0;
    }
}
