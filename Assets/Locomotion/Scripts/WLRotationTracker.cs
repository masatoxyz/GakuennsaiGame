using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLRotationTracker : MonoBehaviour {

    public GameObject Tracker;
    private float currentAngle = 0.0f;
    private float tempAngle = 0.0f;

    Quaternion rotation;
    Vector3 rotationAngles;

    void Update ()
    {
        rotation = this.transform.localRotation;
        rotationAngles = rotation.eulerAngles;

        // Y軸回転
        //rotationAngles.y = rotationAngles.y + Tracker.transform.eulerAngles.y - tempAngle;
        if (Mathf.Abs(currentAngle - tempAngle) >= 15.0f)
        {
            rotationAngles.y = Tracker.transform.localEulerAngles.y;
            currentAngle = Tracker.transform.localEulerAngles.y;
        }

        // オイラー角 → クォータニオンへの変換
        rotation = Quaternion.Euler(rotationAngles);

        // Transform値を設定する
        this.transform.localRotation = rotation;

        tempAngle = Tracker.transform.localEulerAngles.y;

    }
}
