using UnityEngine;

class PointerParameter
{
    private HapticPointer _hapticPointer = null;

    private uint _serialNumber;
    //private HapticPointer.DevType _deviceType;
    private bool _haptics;
    private bool _debugMode;
    private bool _gravity;
    private float _unitySpringK;
    private float _unityDamperB;
    private float _positionScale;
    private float _rotationScale;
    private float _deviceSpringK;
    private float _deviceDamperB;
    private bool _cascadeControl;
    private float _cascadeGain;
    private bool _toggleHold;
    private bool _toggleClutch;
    private int _holdChannel;
    private int _clutchChannel;
    private int _calibrationChannel;
    private static float _fixedDeltaTime = 0.0f;

    public PointerParameter(HapticPointer hapticPointer)
    {
        _hapticPointer = hapticPointer;
    }

    public void record()
    {
        _serialNumber = _hapticPointer.SerialNumber;
        //_deviceType = _hapticPointer.DeviceType;
        _haptics = _hapticPointer.Haptics;
        _debugMode = _hapticPointer.DebugMode;
        _gravity = _hapticPointer.Gravity;
        _unitySpringK = _hapticPointer.UnitySpringK;
        _unityDamperB = _hapticPointer.UnityDamperB;
        _positionScale = _hapticPointer.PositionScale;
        _rotationScale = _hapticPointer.RotationScale;
        _deviceSpringK = _hapticPointer.DeviceSpringK;
        _deviceDamperB = _hapticPointer.DeviceDamperB;
        _cascadeControl = _hapticPointer.CascadeControl;
        _cascadeGain = _hapticPointer.CascadeGain;
        _toggleHold = _hapticPointer.ToggleHold;
        _toggleClutch = _hapticPointer.ToggleClutch;
        _holdChannel = _hapticPointer.HoldChannel;
        _clutchChannel = _hapticPointer.ClutchChannel;
        _calibrationChannel = _hapticPointer.CalibrationChannel;
        if (_fixedDeltaTime == 0.0f)
            _fixedDeltaTime = HapticPointer.FixedDeltaTime;
    }

    public void recall(bool simulation)
    {
        if (simulation)
        {
            _hapticPointer.DebugMode = _debugMode;
            _hapticPointer.Haptics = _haptics;
            _hapticPointer.Gravity = _gravity;
            _hapticPointer.UnitySpringK = _unitySpringK;
            _hapticPointer.UnityDamperB = _unityDamperB;
            HapticPointer.FixedDeltaTime = _fixedDeltaTime;
        }
        else
        {
            _hapticPointer.SerialNumber = _serialNumber;
            //_hapticPointer.DeviceType = _deviceType;
            _hapticPointer.PositionScale = _positionScale;
            _hapticPointer.RotationScale = _rotationScale;
            _hapticPointer.DeviceSpringK = _deviceSpringK;
            _hapticPointer.DeviceDamperB = _deviceDamperB;
            _hapticPointer.CascadeControl = _cascadeControl;
            _hapticPointer.CascadeGain = _cascadeGain;
            _hapticPointer.ToggleHold = _toggleHold;
            _hapticPointer.ToggleClutch = _toggleClutch;
            _hapticPointer.HoldChannel = _holdChannel;
            _hapticPointer.ClutchChannel = _clutchChannel;
            _hapticPointer.CalibrationChannel = _calibrationChannel;
        }
    }

    public void serialize()
    {
        PlayerPrefs.SetInt(getKey("SerialNumber"), (int)_hapticPointer.SerialNumber);
        //PlayerPrefs.SetInt(getKey("DeviceType"), (int)_hapticPointer.DeviceType);
        PlayerPrefs.SetInt(getKey("Haptics"), _hapticPointer.Haptics ? 1 : 0);
        PlayerPrefs.SetInt(getKey("DebugMode"), _hapticPointer.DebugMode ? 1 : 0);
        PlayerPrefs.SetInt(getKey("Gravity"), _hapticPointer.Gravity ? 1 : 0);
        PlayerPrefs.SetFloat(getKey("UnitySpringK"), _hapticPointer.UnitySpringK);
        PlayerPrefs.SetFloat(getKey("UnityDamperB"), _hapticPointer.UnityDamperB);
        PlayerPrefs.SetFloat(getKey("PositionScale"), _hapticPointer.PositionScale);
        PlayerPrefs.SetFloat(getKey("RotationScale"), _hapticPointer.RotationScale);
        PlayerPrefs.SetFloat(getKey("DeviceSpringK"), _hapticPointer.DeviceSpringK);
        PlayerPrefs.SetFloat(getKey("DeviceDamperB"), _hapticPointer.DeviceDamperB);
        PlayerPrefs.SetInt(getKey("CascadeControl"), _hapticPointer.CascadeControl ? 1 : 0);
        PlayerPrefs.SetFloat(getKey("CascadeGain"), _hapticPointer.CascadeGain);
        PlayerPrefs.SetInt(getKey("ToggleHold"), _hapticPointer.ToggleHold ? 1 : 0);
        PlayerPrefs.SetInt(getKey("ToggleClutch"), _hapticPointer.ToggleClutch ? 1 : 0);
        PlayerPrefs.SetInt(getKey("HoldChannel"), _hapticPointer.HoldChannel);
        PlayerPrefs.SetInt(getKey("ClutchChannel"), _hapticPointer.ClutchChannel);
        PlayerPrefs.SetInt(getKey("CalibrationChannel"), _hapticPointer.CalibrationChannel);
        PlayerPrefs.SetFloat(getKey("FixedDeltaTime"), HapticPointer.FixedDeltaTime);
    }

    public void deserialize()
    {
        _hapticPointer.SerialNumber = (uint)
            PlayerPrefs.GetInt(getKey("SerialNumber"), (int)_hapticPointer.SerialNumber);
        //_hapticPointer.DeviceType = (HapticPointer.DevType)
        //    PlayerPrefs.GetInt(getKey("DeviceType"), (int)_hapticPointer.DeviceType);
        _hapticPointer.Haptics = 
            PlayerPrefs.GetInt(getKey("Haptics"), _hapticPointer.Haptics ? 1 : 0) == 1;
        _hapticPointer.DebugMode = 
            PlayerPrefs.GetInt(getKey("DebugMode"), _hapticPointer.DebugMode ? 1 : 0) == 1;
        _hapticPointer.Gravity = 
            PlayerPrefs.GetInt(getKey("Gravity"), _hapticPointer.Gravity ? 1 : 0) == 1;
        _hapticPointer.UnitySpringK = 
            PlayerPrefs.GetFloat(getKey("UnitySpringK"), _hapticPointer.UnitySpringK);
        _hapticPointer.UnityDamperB = 
            PlayerPrefs.GetFloat(getKey("UnityDamperB"), _hapticPointer.UnityDamperB);
        _hapticPointer.PositionScale = 
            PlayerPrefs.GetFloat(getKey("PositionScale"), _hapticPointer.PositionScale);
        _hapticPointer.RotationScale = 
            PlayerPrefs.GetFloat(getKey("RotationScale"), _hapticPointer.RotationScale);
        _hapticPointer.DeviceSpringK = 
            PlayerPrefs.GetFloat(getKey("DeviceSpringK"), _hapticPointer.DeviceSpringK);
        _hapticPointer.DeviceDamperB = 
            PlayerPrefs.GetFloat(getKey("DeviceDamperB"), _hapticPointer.DeviceDamperB);
        _hapticPointer.CascadeControl = 
            PlayerPrefs.GetInt(getKey("CascadeControl"), _hapticPointer.CascadeControl ? 1 : 0) == 1;
        _hapticPointer.CascadeGain = 
            PlayerPrefs.GetFloat(getKey("CascadeGain"), _hapticPointer.CascadeGain);
        _hapticPointer.ToggleHold = 
            PlayerPrefs.GetInt(getKey("ToggleHold"), _hapticPointer.ToggleHold ? 1 : 0) == 1;
        _hapticPointer.ToggleClutch = 
            PlayerPrefs.GetInt(getKey("ToggleClutch"), _hapticPointer.ToggleClutch ? 1 : 0) == 1;
        _hapticPointer.HoldChannel = 
            PlayerPrefs.GetInt(getKey("HoldChannel"), _hapticPointer.HoldChannel);
        _hapticPointer.ClutchChannel = 
            PlayerPrefs.GetInt(getKey("ClutchChannel"), _hapticPointer.ClutchChannel);
        _hapticPointer.CalibrationChannel = 
            PlayerPrefs.GetInt(getKey("CalibrationChannel"), _hapticPointer.CalibrationChannel);
        HapticPointer.FixedDeltaTime = 
            PlayerPrefs.GetFloat(getKey("FixedDeltaTime"), HapticPointer.FixedDeltaTime);
    }

    public void check()
    {
        HapticPointer.FixedDeltaTime = Mathf.Clamp(HapticPointer.FixedDeltaTime, 0.002f, 0.02f);
        _hapticPointer.UnitySpringK = Mathf.Clamp(_hapticPointer.UnitySpringK, 0, System.Single.MaxValue);
        _hapticPointer.UnityDamperB = Mathf.Clamp(_hapticPointer.UnityDamperB, 0, System.Single.MaxValue);
        _hapticPointer.PositionScale = Mathf.Clamp(_hapticPointer.PositionScale, 1, System.Single.MaxValue);
        _hapticPointer.RotationScale = Mathf.Clamp(_hapticPointer.RotationScale, 1, System.Single.MaxValue);
        _hapticPointer.DeviceSpringK = Mathf.Clamp(_hapticPointer.DeviceSpringK, 0, System.Single.MaxValue);
        _hapticPointer.DeviceDamperB = Mathf.Clamp(_hapticPointer.DeviceDamperB, 0, System.Single.MaxValue);
        _hapticPointer.CascadeGain = Mathf.Clamp(_hapticPointer.CascadeGain, 0, System.Single.MaxValue);
        _hapticPointer.HoldChannel = Mathf.Clamp(_hapticPointer.HoldChannel, 0, 8);
        _hapticPointer.ClutchChannel = Mathf.Clamp(_hapticPointer.ClutchChannel, 0, 8);
        _hapticPointer.CalibrationChannel = Mathf.Clamp(_hapticPointer.CalibrationChannel, 0, 8);
    }

    string getKey(string keyBase)
    {
        return _hapticPointer.name + "_" + keyBase;
    }
}
