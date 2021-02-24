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

    private IEnumerator removeDecoyCoroutine;
    public IEnumerator GetRemoveDecotCoroutine() { return removeDecoyCoroutine; }
    public void SetRemoveDecoyCoroutine(IEnumerator enumerator) { removeDecoyCoroutine = enumerator; }

    private Renderer rendererBody;
    private Renderer rendererTower;
    private Color color;

    private float alpha;

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

        d.SetHp(3);

        return d;
    }

    public void InitilizeObjColor()
    {
        rendererBody = GetComponent<Renderer>();
        rendererTower = transform.Find("TankFree_Tower").GetComponent<Renderer>();

        alpha = 1f;

        rendererBody.material.color = new Color(rendererBody.material.color.r, rendererBody.material.color.g, rendererBody.material.color.b, alpha);
        rendererTower.material.color = new Color(rendererTower.material.color.r, rendererTower.material.color.g, rendererTower.material.color.b, alpha);

        SetRemoveDecoyCoroutine(RemoveDecoyCoroutine());
    }

    public IEnumerator RemoveDecoyCoroutine()
    {
        yield return new WaitForSeconds(3f);

        hp = 0;
    }

    public void StaetRemoveDecoyCoroutine()
    {
        StartCoroutine(removeDecoyCoroutine);
    }

    private void RemoveDecoy()
    {
        isDeath = true;
        GameMgr.decoyList.Remove(this);
        Vanish();
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            alpha -= Time.deltaTime;
            rendererBody.material.color = new Color(rendererBody.material.color.r, rendererBody.material.color.g, rendererBody.material.color.b, alpha);
            rendererTower.material.color = new Color(rendererTower.material.color.r, rendererTower.material.color.g, rendererTower.material.color.b, alpha);

            if (rendererTower.material.color.a <= 0)
            {
                RemoveDecoy();
            }
        }



    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "EnemyShot")
        {
            // デコイを消去するコルーチンを最初から再生
            StopCoroutine(removeDecoyCoroutine);
            removeDecoyCoroutine = null;
            removeDecoyCoroutine = RemoveDecoyCoroutine();
            StartCoroutine(removeDecoyCoroutine);

            hp--;
            SetHp(hp);
        }

    }
}
