using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : Token
{
    // 管理オブジェクト
    public static TokenMgr<Decoy> parent = null;

    private GameMgr gm;
    private void SetGmaeMgr(GameMgr gm) { this.gm = gm; }

    [SerializeField]
    private int ID;
    public void SetParam(int id) { ID = id; }

    private int maxHp = 3;
    private int hp;
    public void SetHp(int hp) { this.hp = hp; }
    public int GetHp() { return hp; }

    public bool isDeath;
    public void SetIsDeath(bool death) { isDeath = death; }
    public bool GetIsDeath() { return isDeath; }

    public static Decoy Add(int id, GameMgr gm, Vector3 pos)
    {
        Decoy d = parent.Add(pos.x, pos.y, pos.z);

        d.SetParam(id);

        d.SetGmaeMgr(gm);

        d.SetIsDeath(false);

        return d;
    }

    public void InitilizeHp()
    {
        SetHp(maxHp);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            SetIsDeath(true);
            GameMgr.decoyList.RemoveAt(ID);
            Vanish();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "EnemyShot")
        {
            hp--;
            SetHp(hp);
        }

    }
}
