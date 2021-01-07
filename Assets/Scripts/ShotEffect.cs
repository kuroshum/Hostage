using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffect : Token
{
    // 管理オブジェクト
    public static TokenMgr<ShotEffect> parent = null;

    public static ShotEffect Add(float x, float y, float z)
    {
        // Enemyインスタンスの取得
        ShotEffect se = parent.Add(x, y, z);

        return se;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
