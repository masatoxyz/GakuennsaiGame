using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JengaRules : MonoBehaviour {

    public Text gameOverText, fallenNumText;
    bool gameInProgress = true;
    public int fallenPieceCount = 0;
    float count = 0.0f;
    int count2 = 0;

	void Update () {
        fallenNumText.text = fallenPieceCount.ToString();

		//ゲームオーバー判定
        if (fallenPieceCount >= 5)
        {
            gameInProgress = false;
        }

        if (!gameInProgress)
        {
            if (count2 < 5)
            {
                count += Time.deltaTime;

                if (count > 0.2f)
                {
                    count = 0.0f;

                    if (count2 % 2 == 0)
                    {
                        gameOverText.enabled = true;
                    }
                    if (count2 % 2 == 1)
                    {
                        gameOverText.enabled = false;
                    }

                    count2++;
                }
            }
        }
	}
}
