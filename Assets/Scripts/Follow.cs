using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public static GameObject objTarget;
    public Vector3 offset;

    void Start()
    {
        objTarget = GameObject.Find("Player(Clone)");
        updatePostion();

        // x軸を軸にして毎秒2度、回転させるQuaternionを作成（変数をrotとする）
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);
        // 現在の自信の回転の情報を取得する。
        Quaternion q = this.transform.rotation;
        // 合成して、自身に設定
        //this.transform.rotation = q * rot;
    }

    void LateUpdate()
    {
        if (objTarget != null)
        {
            updatePostion();
        }
    }

    void updatePostion()
    {
        Vector3 pos = objTarget.transform.localPosition;

        transform.localPosition = pos + offset;
    }
}
