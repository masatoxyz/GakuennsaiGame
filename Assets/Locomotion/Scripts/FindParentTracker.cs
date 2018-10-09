using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindParentTracker : MonoBehaviour {

    public GameObject tracker;
    public HapticPointer hpL, hpR;
    private bool parentChanged = false;
	
	void Update () {
        if((hpL.calibratedOk || hpR.calibratedOk) && !parentChanged)
        {
            parentChanged = true;
            transform.parent = tracker.transform;
        }
    }
}
