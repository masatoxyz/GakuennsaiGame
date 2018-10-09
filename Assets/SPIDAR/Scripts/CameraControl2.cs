using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl2 : MonoBehaviour {

    float mouseX, mouseY;

    void Update ()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        //transform.Rotate(transform.InverseTransformDirection(new Vector3(0.0f, mouseX * 5.0f, 0.0f)));
        transform.Rotate(new Vector3(-mouseY * 5.0f, 0.0f, 0.0f));
    }
}
