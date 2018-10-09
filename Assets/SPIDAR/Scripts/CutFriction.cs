using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutFriction : MonoBehaviour {

    public PhysicMaterial onFriction, offFriction;
    public Material holdMat, releaseMat, fallenMat;
    public bool holding = false;
    bool falling = false;
    JengaRules jRules;
    //GameObject cameraObj;
    //Vector3 camRot, camRotOld;

    private void Start()
    {
        jRules = GameObject.Find("JengaMain").GetComponent<JengaRules>();
        //cameraObj = GameObject.Find("CameraRig");
    }

    void Update () {
		if (holding) {
            GetComponent<Renderer>().material = holdMat;
            GetComponent<BoxCollider>().material = offFriction;
        } else if (falling) {
            GetComponent<Renderer>().material = fallenMat;
            GetComponent<BoxCollider>().material = onFriction;
        }
        else
        {
            GetComponent<Renderer>().material = releaseMat;
            GetComponent<BoxCollider>().material = onFriction;
        }

        //camRot = cameraObj.transform.eulerAngles;

        //if (camRot != camRotOld)
        //{
        //    //カメラが動いているとき
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //    GetComponent<Renderer>().material = holdMat;
        //} else
        //{
        //    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //}

        //camRotOld = camRot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Plane")
        {
            falling = true;
            jRules.fallenPieceCount++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Plane")
        {
            falling = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Plane")
        {
            falling = false;
            jRules.fallenPieceCount--;
        }
    }
}
