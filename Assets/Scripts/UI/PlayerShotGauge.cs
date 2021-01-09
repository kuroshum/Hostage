using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotGauge : MonoBehaviour
{
    [SerializeField]
    private GameObject shotObj;

    // 弾ゲージ全削除＆HP分作成
    public void SetShotGauge(int shotNum)
    {
        //　弾ゲージを一旦全削除
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //　現在の弾ゲージ数分のライフゲージを作成
        for (int i = 0; i < shotNum; i++)
        {
            Instantiate(shotObj, transform);
        }
    }
    //　ダメージ分だけ削除
    public void DeleteShotGauge(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            //　最後のライフゲージを削除
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
