using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyGauge : MonoBehaviour
{
    [SerializeField]
    private GameObject decoyObj;

    // デコイゲージ全削除＆HP分作成
    public void SetDecoyGauge(int decoyNum)
    {
        // デコイゲージを一旦全削除
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //　現在の弾ゲージ数分のデコイゲージを作成
        for (int i = 0; i < decoyNum; i++)
        {
            Instantiate(decoyObj, transform);
        }
    }
    //　ダメージ分だけ削除
    public void DeleteDecoyGauge(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            //　最後のデコイゲージを削除
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

