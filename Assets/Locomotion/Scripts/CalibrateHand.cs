using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateHand : MonoBehaviour {

    public GameObject handObj;
    public GameObject handPrefab;

    private void Start()
    {
        DestroyHand();
        Invoke("RegenerateHand", 0.1f);
    }

    public void DestroyHand ()
    {
        Destroy(handObj);
    }

    public void RegenerateHand()
    {
        handObj = Instantiate(handPrefab, transform.position, Quaternion.identity);
    }
}
