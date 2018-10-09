//
// JengaMain.cs
//

using UnityEngine;
using UnityEngine.SceneManagement;

//
public class JengaMain : MonoBehaviour
{
    private float mass;
    private float drag;
    private float angularDrag;

    private GUIStyle style = new GUIStyle();
    private GUIStyleState state = new GUIStyleState();

    private bool showInformation = true;
    private bool DebugMode = true;

    //
    void Awake()
    {
        Application.targetFrameRate = 60;

        state.textColor = Color.black;
        style.normal = state;
        style.fontSize = 16;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Jenga");
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            showInformation = !showInformation;
        }
    }

    void OnGUI()
    {
        if (!showInformation) return;

        //GUILayout.Label("Information (i),  " + "Restart (r),  " + "Quit (q)", style);
        //GUILayout.Label("Hold (space),  " + "Calibration (c),  " + "Clutch (v)", style);
        //GUILayout.Label("Select Haptic Pointer (m)", style);
        //GUILayout.Label("Unity deltaTime: " + (Time.deltaTime * 1000).ToString("f3") + "[ms], fixedDeltaTime: "
        //                + (Time.fixedDeltaTime * 1000).ToString("f3") + "[ms]", style);

        //GUILayout.Label("", style);
        GameObject obj = GameObject.Find("HapticPointer");

        HapticPointer hp = null;
        int[] encoderCount = new int[8];

        if (obj) hp = obj.GetComponent<HapticPointer>();

        if (!obj || !hp)
        {
            //GUILayout.Label(selectedPointer.ToString() + " is null", style);
            return;
        }

        GUILayout.Label(hp.ToString(), style);

        GUILayout.Label("Device serial number: " + hp.SerialNumber, style);
        GUILayout.Label("Device Delta Time: " + (hp.DeviceDeltaTime * 1000).ToString("f3") + "[ms]", style);
        GUILayout.Label("Device Type: " + hp.DeviceType, style);
        GUILayout.Label("Haptics: " + hp.Haptics + ", Gravity: " + hp.Gravity + ", Cascade Control: " + hp.CascadeControl, style);

        if (!DebugMode) return;

        GUILayout.Label("GPIO: " + hp.GpioValue, style);

        GUILayout.Label("", style);

        hp.GetEncoderCount(ref encoderCount);

        GUILayout.Label("encder counts", style);

        for (int i = 0; i < 8; ++i)
        {
            GUILayout.Label("motor " + (i + 1) + ": " + encoderCount[i], style);
        }
        GUILayout.Label("", style);

        if (hp.HoldingObject)
        {
            HoldState hs = hp.HoldingObject.GetComponent<HoldState>();
            GUILayout.Label(hp.HoldingObject.ToString(), style);
            GUILayout.Label(hs.ToString(), style);
            GUILayout.Label("Collision state: " + hs.Collision, style);
            GUILayout.Label("mass: " + hp.HoldingObject.mass, style);
            GUILayout.Label("center of mass: " + hp.HoldingObject.centerOfMass.ToString("f4"), style);
            GUILayout.Label("inertiaTensor: " + hp.HoldingObject.inertiaTensor.ToString("f4"), style);
            GUILayout.Label("inertiaTensorRotation: " + hp.HoldingObject.inertiaTensorRotation.ToString("f4"), style);
            GUILayout.Label("", style);
        }
    }

} // end of class JengaMain.

// end of file.
