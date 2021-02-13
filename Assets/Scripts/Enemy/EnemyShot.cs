using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : Token
{

    // 管理オブジェクト
    public static TokenMgr<EnemyShot> parent = null;


    [SerializeField]
    private GameObject Effect;

    // 管理オブジェクト
    //public static TokenMgr<Shot> parent = null;

    protected string tagName;
    public void SetTagName(string tag_name) { tagName = tag_name; }

    // 速度
    protected Vector3 velocity;
    public Vector3 GetVelocity() { return velocity; }

    protected GameMgr gm;

    private AudioSource sound;

    private Player player;
    private void SetPlayer(Player player) { this.player = player; }

    private float speed;

    //[SerializeField]
    //private GameObject Effect;

    /*
    public static Shot Add(string tag_name, float x, float y, float z)
    {
        // Enemyインスタンスの取得
        Shot s = parent.Add(x, y, z);

        s.SetTagName(tag_name);

        return s;
    }
    */

    // 指定された角度（ 0 ～ 360 ）をベクトルに変換して返す
    public static Vector3 GetDirection(float angle)
    {
        return new Vector3
        (
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );
    }

    // 弾を発射する時に初期化するための関数
    public void Init(float angle, float speed, GameMgr gm)
    {
        this.gm = gm;

        this.speed = speed;

        // 弾の発射角度をベクトルに変換する
        var direction = GetDirection(angle);

        // 発射角度と速さから速度を求める
        velocity = direction;

        // 弾が進行方向を向くようにする
        var angles = transform.localEulerAngles;
        angles.y = angle - 90;
        transform.localEulerAngles = angles;

        sound = this.gameObject.GetComponent<AudioSource>();

        // 2 秒後に削除する
        //Destroy(gameObject, 2);
    }

    void Update()
    {
        // 移動する
        transform.localPosition += velocity * speed * Time.deltaTime;
    }

    public static EnemyShot Add(string tag_name, float x, float y, float z, Player player)
    {
        // Enemyインスタンスの取得
        EnemyShot es = parent.Add(x, y, z);

        es.SetTagName(tag_name);

        es.SetPlayer(player);

        return es;
    }

    public void StartShotEffect()
    {
        gm.StartStartExploson(this.transform.position + velocity, 1f);
    }

    void OnTriggerEnter(Collider col)
    {

        if(col.gameObject.tag == "Player")
        {
            Vector2 eshot_forward = new Vector2(this.transform.forward.x, this.transform.forward.z);
            Vector2 player_forward = new Vector2(col.transform.forward.x, col.transform.forward.z);

            // 
            if (Mathf.Abs(player.GetAngle(player_forward, eshot_forward)) > 30f && Mathf.Abs(Vector2.Dot(player_forward, eshot_forward)) > 0.1f)
            {
                Debug.Log("跳弾");
                Vector2 nomal_vector = new Vector2(-player_forward.y, player_forward.x);
                velocity = eshot_forward + 2 * Vector2.Dot(-eshot_forward, nomal_vector) * nomal_vector;
                velocity.y = 0f;
            }
            else
            {
                Debug.Log("貫通");
                player.SetHp(player.GetHp() - 1);
                Vanish();
            }
        }

        if (col.gameObject.tag == "PlayerShot" || col.gameObject.tag == "Decoy")
        {
            Vanish();
        }

        if (col.gameObject.tag == "Wall")
        {

            //if (tagName == "Player" && GameMgr.ENEMY_NUM > gm.GetActiveEnemyNum())
            /*
            if (tagName == "Player" && GameMgr.ENEMY_NUM > gm.enemyList.Count)
            {
                gm.Add_Enemy(transform.position);
            }
            */
            Vanish();
        }

        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Wall" || col.gameObject.tag == "Decoy")
        {
            //sound.Play();
            gm.StartExploson(col, 1f);
            //ShotEffect es = ShotEffect.Add(col.transform.position.x, col.transform.position.y, col.transform.position.z);
            //GameObject MakedObject = Instantiate(Effect, col.transform.position, Quaternion.identity) as GameObject;
        }

    }
}