using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    private Camera myCamera;


    void Awake() {
        myCamera = GetComponent<Camera>();
    }

	void Update () {
        //myCamera.orthographicSize = transform.parent.localScale.x * 5;
        //myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize + Input.mouseScrollDelta.y, 1, 100);
    }
}
