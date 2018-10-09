//
// CameraControl.cs
//

using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    private float radius = 0.7f;
    private float angle = Mathf.PI / 2;
    private float height = 0.7f;

    private float angleStep = Mathf.PI / 180;
    private float moveStep = 0.01f;

    private Vector3 pointerOffsetL;
    private Vector3 pointerOffsetR;

    void Awake()
    {
        radius = transform.position.z;

        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);

        pointerOffsetL = GameObject.Find("HapticPointerL").transform.position;
        pointerOffsetR = GameObject.Find("HapticPointerR").transform.position;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal == -1)
        {
            angle -= angleStep;
            UpdateCameraPan();
        }
        else if (horizontal == 1)
        {
            angle += angleStep;
            UpdateCameraPan();
        }

        float vertical = Input.GetAxis("Vertical");

        if (vertical == -1)
        {
            height -= moveStep;
            UpdateCameraTilt();
        }
        else if (vertical == 1)
        {
            height += moveStep;
            UpdateCameraTilt();
        }

        if (Input.GetButton("Reset"))
        {
            ClearTransform();
        }
    }

    void Start()
    {

    }
    void UpdateCameraPan()
    {
        Vector3 pos = transform.position;

        pos.x = radius * Mathf.Cos(angle);
        pos.z = radius * Mathf.Sin(angle);

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(Vector3.zero - pos);

        Quaternion rot = Quaternion.Inverse(Quaternion.AngleAxis((angle - Mathf.PI / 2) * Mathf.Rad2Deg, Vector3.up));

        HapticPointer hp = GetHapticPointer("HapticPointerL");

        if (hp)
        {
            hp.PositionOffset = rot * pointerOffsetL;
            hp.RotationOffset = rot;
        }

        hp = GetHapticPointer("HapticPointerR");

        if (hp)
        {
            hp.PositionOffset = rot * pointerOffsetR;
            hp.RotationOffset = rot;
        }
    }

    void UpdateCameraTilt()
    {
        Vector3 pos = transform.position;

        pos.y = height;

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(Vector3.zero - pos);
    }

    HapticPointer GetHapticPointer(string name)
    {
        GameObject obj = GameObject.Find(name);

        if (!obj) return null;

        return obj.GetComponent<HapticPointer>();
    }

    void ClearTransform()
    {
        Vector3 pos;

        pos.x = 0;
        pos.y = 0.7f;
        pos.z = -0.7f;

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(Vector3.zero - pos);

        angle = Mathf.PI / 2;
        height = 0.7f;

        HapticPointer hp = GetHapticPointer("HapticPointerL");

        if (hp)
        {
            hp.ReleaseObject();
            hp.PositionOffset = pointerOffsetL;
            hp.RotationOffset = Quaternion.identity;
        }

        hp = GetHapticPointer("HapticPointerR");

        if (hp)
        {
            hp.ReleaseObject();
            hp.PositionOffset = pointerOffsetR;
            hp.RotationOffset = Quaternion.identity;
        }
    }

} // end of class CameraControl.

// end of file.
