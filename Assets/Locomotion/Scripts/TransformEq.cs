using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEq : MonoBehaviour {

    public GameObject go;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

    }
}
