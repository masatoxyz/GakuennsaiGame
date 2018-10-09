//
// HapticPointer.cs 
//
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

using Spidar = TokyoTech.Spidar.Spidar;
using SpidarVector = TokyoTech.Spidar.Vector3;
using SpidarQuaternion = TokyoTech.Spidar.Quaternion;

//
// class HapticPointer
//
public class HapticPointer : MonoBehaviour
{
    public enum DevType
    {
        GX_MAXON_L,
        GX_MAXON_R,
        GC_MABCHI_L,
        GC_MABCHI_R,
        J_MAXON_LL,
        J_MAXON_R,
        J_MABUCHI,
        //H_MABUCHI_L,
        //H_MABUCHI_R,
        ALUMI_MABUCHI,
        C_L,
        C_R,
        ALUMI_MABUCHI_5V,
        C2_L,
        C2_R,
        C2_L_Grasp,
    }

    public uint     SerialNumber        = 0;
    public DevType  DeviceType          = DevType.J_MABUCHI;
    public bool     Haptics             = false;
    public bool     DebugMode           = false;
    public bool     Gravity             = false;
    public float    UnitySpringK        = 0;
    public float    UnityDamperB        = 0;
    public float    PositionScale       = 0;
    public float    RotationScale       = 0;
    public float    DeviceSpringK       = 0;
    public float    DeviceDamperB       = 0;
    public bool     CascadeControl      = false;
    public float    CascadeGain         = 0;
    public bool     ToggleHold          = false;
    public bool     ToggleClutch        = false;
    public int      HoldChannel         = 0;
    public int      ClutchChannel       = 0;
    public int      CalibrationChannel  = 0;
    public bool     TransmitOnCollide   = false;
    public bool     TransmitOnHold      = false;
    public Material HoldingMaterial     = null;
    public Material CollidingMaterial   = null;
    public Material FreeMaterial        = null;
    public Mesh     HoldingMesh         = null;
    public Mesh     CollidingMesh       = null;
    public Mesh     FreeMesh            = null;
    public static float FixedDeltaTime  = 0.002f;

    //
    public bool         Activated       { get { return spidar != null; } }
    public bool         RequestInit     { get { bool flag = requestInit; requestInit = false; return flag; } }
    public float        DeviceDeltaTime { get { return spidar != null ? spidar.GetDeltaTime() : 0.0f; } }
    public uint         GpioValue       { get { return spidar != null ? spidar.GetGpioValue() : 0; } }
    public Rigidbody    CollidingObject { get { return collidingObject; } }
    public Rigidbody    HoldingObject   { get { return holdingObject; } }
    public Vector3      PositionOffset  { get; set; }
    public Quaternion   RotationOffset  { get; set; }

    //
    private Spidar spidar = null;
    private SpringDamperModel model = new SpringDamperModel();

    private bool[] gpioDownState = new bool[8];
    private bool[] gpioUpState = new bool[8];

    private bool clutchEngaged = true;
    private Vector3 clutchedPositionOffset = Vector3.zero;
    private Vector3 clutchedPosition = Vector3.zero;
    private Quaternion clutchedRotation = Quaternion.identity;

    private Pose pose;
    private Pose prevPose;
    private Pose rawPose;

    private uint triggerEnterCount = 0;
    private Rigidbody collidingObject = null;
    private Rigidbody holdingObject = null;
    private Rigidbody transmitObject = null;

    private Renderer meshRenderer = null;
    private MeshFilter meshFilter = null;

    private bool curMultiHold = false;
    private bool prvMultiHold = false;

    private bool requestInit = false;
    private int updateSkipCount = 0;
    private Hashtable stockMaterial = new Hashtable();

    //**** Jenga System ****//
    public GameObject hpTarget;
    public GameObject rotateTarget;
    private bool calibrated = false;
    public GameObject handCollider;
    //public TimerManager timerScript;

    //**** For Test ****//
    public bool keyHold = false;

    //calibrate
    public bool calibratedOk = false;

    /// <summary>
    /// SPIDARの初期化を明示的に実行する．
    /// 初期化に成功した場合はtrueを返す．
    /// </summary> 
    /// <returns>
    /// true: 成功
    /// false: 失敗
    /// </returns> 
    public bool Initialize()
    {
        if (spidar != null)
        {
            spidar.Stop();
            spidar.Dispose();
            spidar = null;
        }

        spidar = Spidar.Create(SerialNumber, (int)DeviceType);

        if (spidar != null)
        {
            spidar.Start();

            for (int i = 0; i < 8; ++i)
            {
                gpioDownState[i] = true;
                gpioUpState[i] = true;
            }

            updateSkipCount = 100;
            meshRenderer.enabled = true;
            return true;
        }
        else
        {
            meshRenderer.enabled = false;
            return false;
        }
    }

    /// <summary>
    /// 触れている剛体オブジェクトを掴む．
    /// オブジェクトを掴むことに成功した場合はtrueを返す．
    /// 触れているオブジェクトがない場合や，すでにオブジェクトを掴んでいる場合はなにもせずにfalseを返す．
    /// </summary> 
    /// <returns>
    /// true: 成功
    /// false: 失敗
    /// </returns> 
    public bool HoldObject()
    {
        if (holdingObject != null || collidingObject == null)
            return false;

        holdingObject = collidingObject;

        HoldState hs = GetHoldState();
        hs.OnHoldObject();

        model.Clear();
        model.SpringK = UnitySpringK;
        model.DamperB = UnityDamperB;
        model.pointerOrigin = pose;
        model.rigidbodyOrigin = (Pose)holdingObject;

        meshRenderer.material = HoldingMaterial;
        meshFilter.mesh = HoldingMesh;

        TransmitObject(holdingObject, TransmitOnHold);

        holdingObject.GetComponent<CutFriction>().holding = true;

        return true;
    }

    /// <summary>
    /// 掴んでいるオブジェクトを放す．
    /// 成功した場合はtrueを返し，オブジェクトを掴んでいない場合は何もせずにfalseを返す．
    /// </summary> 
    /// <returns>
    /// true: 成功
    /// false: 失敗
    /// </returns>   
    public bool ReleaseObject()
    {
        if (holdingObject == null)
            return false;

        if (spidar != null)
            spidar.ClearForce();

        HoldState hs = GetHoldState();
        hs.OnReleaseObject();
        RemoveHoldState();

        holdingObject.GetComponent<CutFriction>().holding = false;

        holdingObject = null;

        model.Clear();

        TransmitObject(null, false);

        if (collidingObject != null)
        {
            meshRenderer.material = CollidingMaterial;
            meshFilter.mesh = CollidingMesh;
            TransmitObject(collidingObject, TransmitOnCollide);
        }
        else
        {
            meshRenderer.material = FreeMaterial;
            meshFilter.mesh = FreeMesh;
        }

        return true;
    }

    /// <summary>
    /// クラッチをつなぎ，クラッチが離れている間の姿勢の変化によるフセット値を更新する．
    /// 成功した場合はtrueを返し，クラッチが離れていない場合は何もせずにfalseを返す．
    /// </summary> 
    /// <returns>
    /// true: 成功
    /// false: 失敗
    /// </returns>  
    public bool EngageClutch()
    {
        if (clutchEngaged)
            return false;

        clutchedPositionOffset = clutchedPosition - rawPose.position;

        pose.position = RotationOffset * (rawPose.position + clutchedPositionOffset) + PositionOffset;
        pose.rotation = RotationOffset * rawPose.rotation;
        pose.velocity = Vector3.zero;
        pose.angularVelocity = Vector3.zero;

        prevPose = pose;

        if (holdingObject != null)
        {
            Quaternion q = QuaternionUtility.Rotate(model.pointerOrigin.rotation, RotationOffset * clutchedRotation);
            model.pointerOrigin.rotation = Quaternion.Inverse(q) * pose.rotation;
        }
        clutchEngaged = true;

        return true;
    }

    /// <summary>
    /// クラッチを離し，その際のグリップの姿勢を保存する．
    /// 成功した場合はtrueを返し，クラッチがすでに離れている場合は何もせずにfalseを返す．
    /// </summary> 
    /// <returns>
    /// true: 成功
    /// false: 失敗
    /// </returns>  
    public bool ReleaseClutch()
    {
        if (!clutchEngaged)
            return false;

        clutchedPosition = rawPose.position + clutchedPositionOffset;
        clutchedRotation = rawPose.rotation;
        clutchEngaged = false;

        return true;
    }

    /// <summary>
    /// デバイスのグリップ姿勢のキャリブレーションを行う．
    /// 安全のため，オブジェクトを掴んでいる場合は離し，クラッチによるオフセット値もクリアする．
    /// </summary> 
    public void Calibrate()
    {
        if (holdingObject != null)
            ReleaseObject();

        clutchEngaged = false;

        if (spidar != null)
            spidar.Calibrate();

        clutchedPositionOffset = Vector3.zero;
        clutchedPosition = Vector3.zero;
        clutchedRotation = Quaternion.identity;
        clutchEngaged = true;
    }

    /// <summary>
    /// エンコーダ値を取得する．
    /// </summary>
    /// <param name="count">
    /// エンコーダ値を保存する配列の参照
    /// </param>
    public void GetEncoderCount(ref int[] count)
    {
        if (spidar != null)
            spidar.GetEncoderCount(ref count);
    }

    /// <summary>
    /// GPIO値がhigh(1)からlow(0)に変化した際にtrueを返し，それ以外の場合はfalseを返す．
    /// 他にチャンネル番号が基板のGPIO数の範囲にない場合もfalseを返す．
    /// </summary>
    /// <param name="channel">
    /// 取得するGPIOのチャンネル番号
    /// </param>
    /// <returns>
    /// true: GPIO値が1から0に変化した場合
    /// false: それ以外
    /// </returns>  
    public bool GetGpioDown(int channel)
    {
        if (spidar == null)
            return false;

        if (channel == 0 || channel > spidar.GpioCount)
            return false;

        int checkValue = 1 << (channel - 1);

        if ((GpioValue & checkValue) == 0)
        {
            if (!gpioDownState[channel - 1]) return false;
            gpioDownState[channel - 1] = false;
            return true;
        }
        else
        {
            gpioDownState[channel - 1] = true;
            return false;
        }
    }

    /// <summary>
    /// GPIO値がlow(0)からhigh(1)に変化した際にtrueを返し，それ以外の場合はfalseを返す．
    /// 他にチャンネル番号が基板のGPIO数の範囲にない場合もfalseを返す．
    /// </summary>
    /// <param name="channel">
    /// 取得するGPIOのチャンネル番号
    /// </param>
    /// <returns>
    /// true: GPIO値が0から1に変化した場合
    /// false: それ以外
    /// </returns>   
    public bool GetGpioUp(int channel)
    {
        if (spidar == null)
            return false;

        if (channel == 0 || channel > spidar.GpioCount)
            return false;

        int checkValue = 1 << (channel - 1);

        if ((GpioValue & checkValue) == 0)
        {
            gpioUpState[channel - 1] = true;
            return false;
        }
        else
        {
            if (!gpioUpState[channel - 1]) return false;

            gpioUpState[channel - 1] = false;
            return true;
        }
    }

    //
    // private functions
    //

    void Start()
    {
        PositionOffset = transform.position;
        RotationOffset = transform.rotation;

        model.Clear();

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        collidingObject = null;
        holdingObject = null;
        transmitObject = null;

        meshRenderer.material = FreeMaterial;
        meshFilter.mesh = FreeMesh;

        requestInit = !Initialize();
    }

    void OnDestroy()
    {
        ReleaseObject();

        if (spidar != null)
        {
            spidar.Stop();
            spidar.Dispose();
        }

        PointerParameter parameter = new PointerParameter(this);
        parameter.serialize();
    }

    void Update()
    {
        PositionOffset = hpTarget.transform.position;
        RotationOffset = rotateTarget.transform.rotation;

        if (updateSkipCount > 0)
        {
            updateSkipCount--;
            return;
        }

        if (FixedDeltaTime != Time.fixedDeltaTime)
            Time.fixedDeltaTime = FixedDeltaTime;

        if (!calibrated)
        {
            if (GetGpioDown(CalibrationChannel))
            {
                //ReleaseObject();
                Calibrate();
                handCollider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                handCollider.transform.position = hpTarget.transform.position;
                calibrated = true;
                calibratedOk = true;
                Invoke("CatchObject", 0.1f);
            }
        }

        if (GetGpioDown(HoldChannel))
        {
            //ReleaseObject();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

                                        //if (GetGpioDown(CalibrationChannel))
                                        //    Calibrate();

                                        //if (ToggleHold)
                                        //{
                                        //    if (GetGpioDown(HoldChannel) && !ReleaseObject())
                                        //        HoldObject();
                                        //}
                                        //else
                                        //{
                                        //    if (GetGpioDown(HoldChannel))
                                        //        HoldObject();

                                        //    if (GetGpioUp(HoldChannel))
                                        //        ReleaseObject();
                                        //}

                                        //if (ToggleClutch)
                                        //{
                                        //    if (GetGpioDown(ClutchChannel) && !ReleaseClutch())
                                        //        EngageClutch();
                                        //}
                                        //else
                                        //{
                                        //    if (GetGpioDown(ClutchChannel))
                                        //        ReleaseClutch();

                                        //    if (GetGpioUp(ClutchChannel))
                                        //        EngageClutch();
                                        //}
                                        //if (Input.GetKeyDown(KeyCode.C))
                                        //    Calibrate();

                                        //if (keyHold && Input.GetKeyDown(KeyCode.Z) && !ReleaseObject())
                                        //{
                                        //    HoldObject();
                                        //}
    }

    //void CalibrateOnce ()
    //{
    //    Calibrate();
    //    handCollider.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    handCollider.transform.position = hpTarget.transform.position;
    //    calibrated = true;
    //    calibratedOk = true;
    //}

    void CatchObject()
    {
        HoldObject();
        ReleaseObject();
        HoldObject();
    }

    void FixedUpdate()
    {
        CheckMultiHold();
        GetSpidarPose();
        SetSpidarForce();
        SetObjectForce();
    }

    void OnTriggerEnter(Collider collider)
    {
        ++triggerEnterCount;

        collidingObject = collider.GetComponentInParent<Rigidbody>();

        if (holdingObject == null)
        {
            meshRenderer.material = CollidingMaterial;
            meshFilter.mesh = CollidingMesh;

            TransmitObject(collidingObject, TransmitOnCollide);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        --triggerEnterCount;
        if (triggerEnterCount > 0) return;

        collidingObject = null;

        if (holdingObject == null)
        {
            meshRenderer.material = FreeMaterial;
            meshFilter.mesh = FreeMesh;

            TransmitObject(null, false);
        }
    }

    void CheckMultiHold()
    {
        if (holdingObject == null)
        {
            curMultiHold = false;
            prvMultiHold = false;
            return;
        }

        GameObject obj = holdingObject.gameObject;

        HoldState[] hsList = obj.GetComponents<HoldState>();

        if (prvMultiHold && !curMultiHold)
            for (int i = 0; i < hsList.Length; ++i)
                hsList[i].CancelCollision();

        prvMultiHold = curMultiHold;
        curMultiHold = hsList.Length > 1;
    }

    void GetSpidarPose()
    {
        prevPose = pose;

        SpidarVector pos, vel, avel;
        SpidarQuaternion rot;

        pos = vel = avel = SpidarVector.zero;
        rot = SpidarQuaternion.identity;

        if (spidar != null)
            spidar.GetPose(out pos, out rot, out vel, out avel);

        rawPose.position = Converter.ScaleUp(Converter.Convert(pos), PositionScale);
        rawPose.rotation = Converter.ScaleUp(Converter.Convert(rot), RotationScale);
        rawPose.velocity = Converter.ScaleUp(Converter.Convert(vel), PositionScale);
        rawPose.angularVelocity = Converter.ScaleUp(Converter.Convert(avel), RotationScale);

        if (clutchEngaged)
        {
            pose.position = RotationOffset * (rawPose.position + clutchedPositionOffset) + PositionOffset;
            pose.rotation = RotationOffset * rawPose.rotation;
            pose.velocity = RotationOffset * rawPose.velocity;
            pose.angularVelocity = RotationOffset * rawPose.angularVelocity;
        }
        else
        {
            pose.position = RotationOffset * clutchedPosition + PositionOffset;
            pose.rotation = RotationOffset * clutchedRotation;
            pose.velocity = Vector3.zero;
            pose.angularVelocity = Vector3.zero;
        }

        transform.position = pose.position;
        transform.rotation = pose.rotation;
    }

    void SetSpidarForce()
    {
        if (spidar == null)
            return;

        spidar.SetHaptics(Haptics);
        spidar.SetCascadeGain(CascadeGain);

        if (holdingObject == null || !clutchEngaged) return;

        HoldState hs = GetHoldState();

        Vector3 g = Vector3.zero;

        if (Gravity)
        {
            g = Vector3.down * holdingObject.mass * 9.81f;
            g = Quaternion.Inverse(RotationOffset) * g;
        }

        if (!hs.Collision && !curMultiHold)
        {
            if (Gravity)
            {
                spidar.SetForce(Converter.Convert(g), 0, 0, Converter.Convert(Vector3.zero), 0, 0, true, false);
            }
            else
            {
                spidar.ClearForce(true);
            }
            return;
        }

        float deviceR2 = spidar.GetGripRadius() * spidar.GetGripRadius();

        float forceScale = DeviceSpringK / (UnitySpringK * PositionScale);
        float torqueScale = DeviceSpringK / (UnitySpringK * RotationScale);

        Vector3 f = -model.CalcForce(pose, holdingObject) * forceScale;
        Vector3 t = -model.CalcTorque(pose, holdingObject) * torqueScale * deviceR2;

        f = Quaternion.Inverse(RotationOffset) * f;
        t = Quaternion.Inverse(RotationOffset) * t;

        float forceK = DeviceSpringK;
        float forceB = DeviceDamperB;

        float torqueK = DeviceSpringK * deviceR2;
        float torqueB = DeviceDamperB * deviceR2;

        spidar.SetForce(Converter.Convert(f + g), forceK, forceB, Converter.Convert(t), torqueK, torqueB, false, CascadeControl);
    }

    void SetObjectForce()
    {
        if (holdingObject == null) return;

        float timeStep = Time.fixedDeltaTime * Application.targetFrameRate;

        Pose temp = Pose.Lerp(prevPose, pose, timeStep);

        Vector3 force = holdingObject.mass * model.CalcForce(temp, holdingObject);
        Vector3 torque = holdingObject.inertiaTensor.magnitude * 4 * model.CalcTorque(temp, holdingObject);

        if (curMultiHold)
        {
            force *= 0.5f;
            torque *= 0.5f;
        }

        holdingObject.AddForce(force);
        holdingObject.AddTorque(torque);
    }

    HoldState GetHoldState()
    {
        GameObject obj = holdingObject.gameObject;

        HoldState [] hsList = obj.GetComponents<HoldState>();

        for (int i = 0; i < hsList.Length; ++i)
            if (hsList[i].Owner == this)
                return hsList[i];

        HoldState hs = obj.AddComponent<HoldState>();
        hs.Owner = this;
        return hs;
    }

    void RemoveHoldState()
    {
        GameObject obj = holdingObject.gameObject;

        HoldState[] hsList = obj.GetComponents<HoldState>();

        for (int i = 0; i < hsList.Length; ++i)
            if (hsList[i].Owner == this)
                Destroy(hsList[i]);
    }

    void TransmitObject(Rigidbody obj, bool flag)
    {
        const string MatName = "__HapticPointer_Material_transparent";

        if (transmitObject != null)
        {
            Renderer[] renderers = transmitObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                    continue;

                renderer.material = stockMaterial[renderer] as Material;
                stockMaterial.Remove(renderer);
            }

            transmitObject = null;
        }

        if (flag)
        {
            Renderer [] renderers = obj.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                    continue;

                if (renderer.materials.Length > 0 &&
                    (renderer.material.name.Length < MatName.Length ||
                     renderer.material.name.Substring(0, MatName.Length) != MatName))
                {
                    stockMaterial[renderer] = renderer.material;

                    Material newMaterial = new Material(Resources.Load<Material>("Transmission"));

                    Color orgColor = renderer.material.GetColor("_Color");
                    Color color = new Color(orgColor.r, orgColor.g, orgColor.b, 0.5f);

                    newMaterial.name = MatName;
                    newMaterial.SetColor("_Color", color);

                    renderer.material = newMaterial;

                    transmitObject = obj;
                }
            }
        }
    }

} // end of class HapticPointer.

// end of file.
