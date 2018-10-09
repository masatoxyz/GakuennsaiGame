//
// LocomoMain.cs
//

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
public class LocomoMain : MonoBehaviour
{
    private GUIStyle style = new GUIStyle();
    private GUIStyleState state = new GUIStyleState();

    private bool showInformation = true;
    private bool selected;
    private bool DebugMode = true;

    private string selectedPointer = "HapticPointerL";

    //public GameObject scText;
    //public Toggle textModeToggle;
    public GameObject player;

    public HapticPointer hpL, hpR;

    private int allowMoveCount = 0;

    void Awake()
    {
        Application.targetFrameRate = 60;

        state.textColor = Color.black;
        style.normal = state;
        style.fontSize = 16;
    }

    void Update()
    {
        //if (!textModeToggle.isOn)
        //{

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                showInformation = !showInformation;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                selected = !selected;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedPointer = "HapticPointerL";
                CalibratePointer();
                allowMoveCount++;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedPointer = "HapticPointerR";
                CalibratePointer();
                allowMoveCount++;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                showInformation = false;
                //ScreenCapture.CaptureScreenshot("C:/Users/demo/Downloads/SearchDoorTask_Output/screenshots/sc" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".png");
                //scText.SetActive(true);
                //scText.GetComponent<ScreenshotTextFade>().timer = 0.0f;
            }

        if (allowMoveCount >= 2 || (hpL.calibratedOk && hpR.calibratedOk))
        {
            player.GetComponent<SensorReceiver>().allowMove = true;
        }
        //}
    }

    public void HideInfomation()
    {
        showInformation = false;
    }

    public void ReloadAndCalibrate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DelayCalibrate()
    {
        selectedPointer = "HapticPointerR";
        CalibratePointer();
    }

    void CalibratePointer()
    {
        HapticPointer hp = GameObject.Find(selectedPointer).GetComponent<HapticPointer>();
        hp.Calibrate();
        Invoke("CatchObject", 0.1f);
    }

    void CatchObject()
    {
        HapticPointer hp = GameObject.Find(selectedPointer).GetComponent<HapticPointer>();
        hp.HoldObject();
        hp.ReleaseObject();
        hp.HoldObject();
    }

    void OnGUI()
    {
        if (!showInformation) return;

        GameObject obj;
        if (selected)
        {
            obj = GameObject.Find("HapticPointerL");
        } else
        {
            obj = GameObject.Find("HapticPointerR");
        }

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

        GUILayout.Label("PointerPosition:" + hp.gameObject.transform.position, style);

        GUILayout.Label("", style);
        GUILayout.Label("", style);
        GUILayout.Label("", style);

        if (hpL.HoldingObject) GUILayout.Label("Haptic Pointer L Holding", style);
        if (hpR.HoldingObject) GUILayout.Label("Haptic Pointer R Holding", style);

        //if (hp.HoldingObject)
        //{
        //    HoldState hs = hp.HoldingObject.GetComponent<HoldState>();
        //    GUILayout.Label(hp.HoldingObject.ToString(), style);
        //    GUILayout.Label(hs.ToString(), style);
        //    GUILayout.Label("Collision state: " + hs.Collision, style);
        //    GUILayout.Label("mass: " + hp.HoldingObject.mass, style);
        //    GUILayout.Label("center of mass: " + hp.HoldingObject.centerOfMass.ToString("f4"), style);
        //    GUILayout.Label("inertiaTensor: " + hp.HoldingObject.inertiaTensor.ToString("f4"), style);
        //    GUILayout.Label("inertiaTensorRotation: " + hp.HoldingObject.inertiaTensorRotation.ToString("f4"), style);
        //    GUILayout.Label("", style);
        //}
    }

} // end of class LocomoMain.

// end of file.
