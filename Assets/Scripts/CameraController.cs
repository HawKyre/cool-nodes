using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    private float scrollSens = -1f;
    private float maxZoom = 150, minZoom = 3;

    private Vector3 prevMousePos;
    private bool firstFrame = true;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float newOrtSize = cam.orthographicSize + Input.mouseScrollDelta.y * scrollSens;
        cam.orthographicSize = Mathf.Min(Mathf.Max(minZoom, newOrtSize), maxZoom);

        if (Input.GetMouseButton(0))
        {
            if (!firstFrame)
            {
                Vector3 cameraPosDelta = (prevMousePos - Input.mousePosition) * 0.012f;

                cameraPosDelta.z = 0;
                Debug.Log(cameraPosDelta);
                cam.transform.position += cameraPosDelta;
                prevMousePos = Input.mousePosition;
            }
            else
            {
                firstFrame = false;
                prevMousePos = Input.mousePosition;
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            firstFrame = true;
        }
    }
}
