using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

/// <summary>
/// Arduinoとシリアル通信を行うための内部処理
/// </summary>

//特にいじる必要はなし．
//シーン中のどこかに本スクリプトをアタッチする．また，インスペクター上でArduinoと通信するPort番号を記述する．
//シリアル通信を用いる際は，SensorReceiver.csと併用する．
//参考サイト：http://tips.hecomi.com/entry/2014/07/28/023525

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    public string portName; //インスペクター上で適宜記述する必要あり
    private int baudRate = 9600;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    void Awake()
    {
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_)
        {
            OnDataReceived(message_);
        }
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort_.Open();

        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();
    }

    private void Close()
    {
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                // if (serialPort_.BytesToRead > 0) {
                message_ = serialPort_.ReadLine();
                isNewMessageReceived_ = true;
                // }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        try
        {
            serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
