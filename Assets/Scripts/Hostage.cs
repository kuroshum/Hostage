using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : Token
{
    // 管理オブジェクト
    public static TokenMgr<Hostage> parent = null;

    private GameMgr gm;
    private void SetGmaeMgr(GameMgr gm) { this.gm = gm; }

    private Player player;
    private void SetPlayer(Player player) { this.player = player; }

    private int ID;
    public void SetParam(int id) { ID = id; }

    private float moveSpeed;
    public void SetMoveSpeed(float move_speed) { moveSpeed = move_speed; }

    public static Hostage Add(int id, float move_speed, Vector3 pos, GameMgr gm, Player player)
    {
        Hostage hostage = parent.Add(pos.x, pos.y+1, pos.z);

        hostage.SetParam(id);

        hostage.SetMoveSpeed(move_speed);

        hostage.SetGmaeMgr(gm);

        hostage.SetPlayer(player);

        return hostage;
    }

    public void FollowPlayer()
    {
        this.transform.rotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
        if((this.transform.position - player.transform.position).sqrMagnitude > 3f)
        {
            this.transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GetHostageFlag() == true)
        {
            FollowPlayer();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "EnemyShot")
        {
            int hp = player.GetHp() - 1;
            player.SetHp(hp);
        }

    }

}
