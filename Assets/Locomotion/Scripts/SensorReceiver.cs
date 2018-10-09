using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// ロコモーションデバイスからシリアルデータを受け取り，歩行入力に変換する処理
/// </summary>

//ロコモーションデバイスではマイコンのArduinoを使用している．
//本スクリプトは移動させたいオブジェクト(プレイヤー)にアタッチする．
//シリアル通信を用いる際は，SerialHandler.csと併用する．
//参考サイト：http://tips.hecomi.com/entry/2014/07/28/023525

public class SensorReceiver : MonoBehaviour
{
    //シーン中のSerialHandler.csをインスペクター上で代入する
    public SerialHandler serialHandler;

    //荷重センサー・移動量計算関連
    private float lt, rt, lb, rb;   //それぞれ左上，右上，左下，右下のセンサーの入力値を格納
    private float lbOld, rbOld;     //一時変数
    private float pressure;         //センサーの入力値を移動量に反映するための一時変数
    private Vector3 dir;            //VRカメラが向いている方向に移動するための一時変数

    //計算に用いる定数　それぞれ必要に応じて調整する
    const float TH1 = 2.0f;            //20.0f 一定以上の前傾姿勢を検出し，歩行を行うか否か判断するスレッショルド
    const float TH2 = 0.2f;             //1.0f 一定以上の足踏みを検出し，歩行を行うか否か判断するスレッショルド
    const float MOVING_VALUE = 0.05f;   //0.1f 移動量の倍率

    public GameObject vrCamera;
    //public Text ltText, rtText, lbText, rbText;
    private Rigidbody rbPlayer;

    public bool allowMove = false;

    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        //シリアルデータを受け取ったとき，このスクリプトのメソッドOnDataReceived()を呼び出すための記述
        serialHandler.OnDataReceived += OnDataReceived;
    }

    void Update()
    {
        //歩行処理
        if (allowMove)
        {
            pressure = (lt + rt) / 2.0f;

            if ((lb > TH1 && (lb - lbOld) > TH2) || (rb > TH1 && (rb - rbOld) > TH2))    //スレッショルドTH1，TH2を満たしていた場合のみ歩行処理を行う
            {
                dir = vrCamera.transform.TransformDirection(0.0f, 0.0f, 1.0f);                                  //VRカメラが向いている方向を読み取る
                rbPlayer.velocity = new Vector3(dir.x, 0.0f, dir.z).normalized * pressure * MOVING_VALUE;     //y方向(空中)への移動を無効化し，ベクトルを正規化，pressureを反映して座標に加算

                ////旧歩行計算処理　歩くときにカクカクするため変更
                //dir = vrCamera.transform.TransformDirection(0.0f, 0.0f, 1.0f);                                  //VRカメラが向いている方向を読み取る
                //transform.position += new Vector3(dir.x, 0.0f, dir.z).normalized * pressure * MOVING_VALUE;     //y方向(空中)への移動を無効化し，ベクトルを正規化，pressureを反映して座標に加算
            }
        }

        lbOld = lb;
        rbOld = rb;

        //ltText.text = "LT:" + lt;
        //rtText.text = "RT:" + rt;
        //lbText.text = "LB:" + lb;
        //rbText.text = "RB:" + rb;
    }

    void OnDataReceived(string message)
    {
        //シリアルデータを受け取るごとに呼び出される処理

        Debug.Log(message);

        var data = message.Split(new string[] {","}, System.StringSplitOptions.None); //受け取った一続きのシリアルデータを区切り文字で複数に分割

        if (data.Length < 2) return;

        try
        {
            //分割した各シリアルデータを対応する変数に格納する
            var d0 = float.Parse(data[0]); rt = d0;
            var d1 = float.Parse(data[1]); lt = d1;
            var d2 = float.Parse(data[2]); rb = d2;
            var d3 = float.Parse(data[3]); lb = d3;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}