using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotStartEffect : Token
{
    //管理オブジェクト
    public static TokenMgr<ShotStartEffect> parent = null;

    public static ShotStartEffect Add(float x, float y, float z)
    {
        // Enemyインスタンスの取得
        ShotStartEffect sse = parent.Add(x, y, z);

        return sse;
    }

}
