using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerBodyをtarget(VRCamera)に常に追従させる処理
/// </summary>

public class PlayerBodyTracer : MonoBehaviour {

    public GameObject target;

	void Update () {
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
	}
}
