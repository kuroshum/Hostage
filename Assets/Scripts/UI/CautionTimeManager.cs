using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CautionTimeManager : MonoBehaviour
{
    private float cautionTime;

    private float maxCautionTime;

    //　タイマー表示用テキスト
    [SerializeField]
    private Text timerText;

    void Start()
    {
    }

    public void SetTime(float time)
    {
        cautionTime = time;
        maxCautionTime = cautionTime;
    }

    public void UpdateCautionTime(Enemy e)
    {
        //Debug.Log(cautionTime);
        cautionTime -= Time.deltaTime;
        if (cautionTime <= 0f)
        {
            cautionTime = maxCautionTime;
            e.SetStates(StateType.Normal);
        }
        timerText.text = ((int)cautionTime).ToString("00") + ":" + (Mathf.Floor((cautionTime - (int)cautionTime) * 100)).ToString("00");
    }
}
