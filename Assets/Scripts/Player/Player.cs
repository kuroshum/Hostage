using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : Token
{
    // 管理オブジェクト
    public static TokenMgr<Player> parent = null;

    private GameMgr gm;
    private void SetGmaeMgr(GameMgr gm) { this.gm = gm; }


    private int ID;
    public void SetParam(int id) { ID = id; }

    [SerializeField]
    private Vector3 velocity;
    public Vector3 GetVelocity() { return velocity; }

    private float moveSpeed;
    public void SetMoveSpeed(float move_speed) { moveSpeed = move_speed; }

    private float applySpeed;
    public void SetApplySpeed(float apply_speed) { applySpeed = apply_speed; }

    private Vector3 oldVeclocity;


    public float m_shotSpeed; // 弾の移動の速さ
    public float m_shotAngleRange; // 複数の弾を発射する時の角度
    public float m_shotTimer; // 弾の発射タイミングを管理するタイマー
    public int m_shotCount; // 弾の発射数
    public float m_shotInterval; // 弾の発射間隔（秒

    // プレイヤーのMaxHP
    private int maxHp = 5;
    // プレイヤーのHP
    [SerializeField]
    private int hp;

    private int maxShotNum = 10;
    public int GetMaxShotNum() { return maxShotNum; }
    [SerializeField]
    private int shotNum;

    private int maxDecoyNum = 3;
    public  int GetMaxDecoyNum() { return maxDecoyNum; }
    [SerializeField]
    private int decoyNum;

    private GameObject tankTower;
    public void SetTankTower() { tankTower = this.transform.Find("TankFree_Tower").gameObject; }

    private Transform canonPos;
    public void SetCanonPos() { canonPos = tankTower.transform.Find("CanonPos").transform; }

    private LifeGauge lg;

    private PlayerShotGauge psg;

    private DecoyGauge dg;

    private GameObject mainCamera;
    private void SetMainCamera(GameObject mainCamera) { this.mainCamera = mainCamera; }

    private Vector3 currentPosition = Vector3.zero;

    private bool startFlag;
    public void SetStartFlag(bool flag) { this.startFlag = flag; }

    private bool moveFlag;
    public void SetMoveFlag(bool flag) { this.moveFlag = flag; }

    private float trailTime;

    private GameObject trailPrefab;
    private void SetTrailPrefab() { trailPrefab = Resources.Load("Prefabs/" + "tankTrail") as GameObject; }
    private Material trailPrefabMaterial;
    private void SetTrailPrefabMaterial() { trailPrefabMaterial = GetComponent<MeshRenderer>().material; }

    private ShakeScreen ss;

    private Follow follow;

    private const float maxTrailTime = 0.35f;

    // スピードアップできる時間
    private float sppedUpTime;
    private const float maxSpeedUpTime = 3f;

    // スピードアップするリキャストタイム
    private float speedUpRecastTime;
    private float maxspeedUpRecastTime = 5f;

    private bool speedUoFlag;
    private bool speedUpRecastFlag;

    public static Player Add(int id, float move_speed, float apply_speed, GameMgr gm,  float x, float y, float z, GameObject mainCamera)
    {
        // Enemyインスタンスの取得
        Player p = parent.Add(x, y, z);

        // IDを設定したり固有の処理をする
        p.SetParam(id);

        p.SetMoveSpeed(move_speed);

        p.SetApplySpeed(apply_speed);

        p.SetGmaeMgr(gm);

        p.SetStartFlag(false);
        p.SetMoveFlag(true);

        p.SetMainCamera(mainCamera);

        p.SetTankTower();

        p.SetCanonPos();

        p.SetTrailPrefab();
        p.SetTrailPrefabMaterial();

        return p;
    }

    public void InitilizeSppedUpGauge()
    {
        sppedUpTime = 0f;
        speedUpRecastTime = 0f;
        speedUoFlag = false;
        speedUpRecastFlag = false;
    }

    public void InitilizeShotGauge(PlayerShotGauge psg)
    {
        this.psg = psg;
        shotNum = maxShotNum;
    }

    public void InitilizeDecoyGauge(DecoyGauge dg)
    {
        this.dg = dg;
        decoyNum = 1;
    }

    public void InitilizeHp(LifeGauge lg)
    {
        this.lg = lg;
        hp = maxHp;
    }

    public void InitilizeShakeScreen()
    {
        follow = mainCamera.GetComponent<Follow>();
        ss = mainCamera.GetComponent<ShakeScreen>();
    }

    public void SetHp(int hp)
    {
        lg.DeleteLifeGauge(1);

        this.hp = hp;

        /*
        if (hp <= 0)
        {
            //　HP表示用UIを非表示にする
            HideStatusUI();
        }
        */
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    // 指定された 2 つの位置から角度を求めて返す
    public float GetAngle(Vector2 from, Vector2 to)
    {
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg;
    }

    public void InitilizeShot()
    {
        // 管理オブジェクトを生成
        PlayerShot.parent = new TokenMgr<PlayerShot>("PlayerShot", maxShotNum);

    }

    // 弾を発射する関数
    private void ShootNWay(float angleBase, float angleRange, float speed, int count)
    {
        //var pos = transform.position + transform.forward / 2f; // プレイヤーの位置
        var pos = canonPos.position;
        var rot = transform.rotation; // プレイヤーの向き

        // 弾を複数発射する場合
        if (1 < count)
        {
            // 発射する回数分ループする
            for (int i = 0; i < count; ++i)
            {
                // 弾の発射角度を計算する
                var angle = angleBase +
                    angleRange * ((float)i / (count - 1) - 0.5f);

                // 発射する弾を生成する
                //var shot = Instantiate(shotPrefab, pos, rot);
                var shot = PlayerShot.Add(this.gameObject.tag, pos.x, pos.y, pos.z);

                // 弾を発射する方向と速さを設定する
                shot.Init(angle, speed, gm);

                ///shot.UpdateShot();
            }
        }
        // 弾を 1 つだけ発射する場合
        else if (count == 1)
        {
            // 発射する弾を生成する
            //var shot = Instantiate(shotPrefab, pos, rot);
            var shot = PlayerShot.Add(this.gameObject.tag, pos.x, pos.y, pos.z);

            // 弾を発射する方向と速さを設定する
            shot.Init(angleBase, speed, gm);

            //shot.StartShotEffect();

            //shot.UpdateShot();
        }
    }

    private float LookCanon()
    {
        // プレイヤーのスクリーン座標を計算する
        var screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

        // プレイヤーから見たマウスカーソルの方向を計算する
        var direction = Input.mousePosition - screenPos;

        // マウスカーソルが存在する方向の角度を取得する
        float angle = GetAngle(Vector3.zero, direction);

        // プレイヤーがマウスカーソルの方向を見るようにする
        var angles = tankTower.transform.eulerAngles;
        angles.y = angle - 90;
        tankTower.transform.eulerAngles = -angles;

        return angle;
    }

    private void LookCrawler(Vector3 diff)
    {
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(diff), Time.deltaTime*10);
    }

    private IEnumerator Stun()
    {
        moveFlag = false;
        yield return new WaitForSeconds(0.05f);
        moveFlag = true;
    }

    private void LeftTrail()
    {
        trailTime += Time.deltaTime;
        float time;
        if (Input.GetKey(KeyCode.LeftShift) && sppedUpTime < maxSpeedUpTime)
        {
            time = maxTrailTime / 1.5f;
        }
        else
        {
            time = maxTrailTime;
        }
        if (trailTime > time)
        {
            trailTime = 0f;
            Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y - 0.459f, this.transform.position.z);
            var obj = Instantiate(trailPrefab, pos, this.transform.rotation);
            GameMgr.trailList.Add(obj);
            StartCoroutine(DisappearTrail(obj));
        }
    }

    private IEnumerator DisappearTrail(GameObject obj)
    {
        int step = 900;
        for (int i = 0; i < step; i++)
        {
            obj.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1 - 1.0f * i / step);
            yield return null;
        }
        GameMgr.trailList.RemoveAt(0);
        Destroy(obj);
    }

    //public void UpdatePlayer()
    void Update()
    {
        //if (startFlag == false) return;

        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得ます
        velocity = Vector3.zero;
        if (moveFlag == true)
        {
            if (Input.GetKey(KeyCode.W))
            {
                velocity.z += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                velocity.x -= 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                velocity.z -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                velocity.x += 1;
            }

            if (Input.GetKey(KeyCode.LeftShift) && speedUpRecastFlag == false)
            {
                speedUoFlag = true;

                // スピードアップする
                velocity = velocity.normalized * 2f;

            }
            // スピードアップがリキャストタイムに入った場合
            if(speedUpRecastFlag == true)
            {
                // 
                sppedUpTime = 0f;
                speedUpRecastTime += Time.deltaTime;
                gm.GetSpeedUpGaugeUI().fillAmount = speedUpRecastTime / maxspeedUpRecastTime;
                if (speedUpRecastTime > maxspeedUpRecastTime)
                {
                    speedUpRecastFlag = false;
                    speedUpRecastTime = 0f;
                }
            }
            else
            {
                if (gm.GetSpeedUpGaugeUI().fillAmount > 0f && speedUoFlag == true)
                {
                    gm.GetSpeedUpGaugeUI().fillAmount -= 1f / maxSpeedUpTime * Time.deltaTime;
                }
                velocity = velocity.normalized;
            }

            if (speedUoFlag == true)
            {
                // スピードアップしている時間を計測
                sppedUpTime += Time.deltaTime;

                // 上限時間までスピードアップした場合はスピードアップを止めてリキャストに入る
                if (sppedUpTime > maxSpeedUpTime)
                {
                    speedUpRecastFlag = true;
                    speedUoFlag = false;
                }

                gm.GetSpeedUpGaugeUI().fillAmount -= 1f / maxSpeedUpTime * Time.deltaTime;
            }
            //velocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        }

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            // プレイヤーの位置(transform.position)の更新
            // 移動方向ベクトル(velocity)を足し込みます
            //this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + velocity.normalized, moveSpeed * Time.deltaTime);
            this.transform.position += velocity * moveSpeed * Time.deltaTime;

            // キーボードの方向キーの移動量を取得し、その移動量を前の値から新しい値へと補間した状態をvelocityに入れ直す
            velocity = Vector3.MoveTowards(oldVeclocity, velocity, moveSpeed * Time.deltaTime);
            oldVeclocity = velocity;

            this.transform.LookAt(transform.position + velocity);

            LeftTrail();
        }

        float angle = LookCanon();

        // 弾の発射タイミングを管理するタイマーを更新する
        m_shotTimer += Time.deltaTime;
        
        // マウスクリック左で弾を発射
        if (Input.GetMouseButtonDown(0))
        {
            // 発射間隔と残弾数のチェック
            if (m_shotTimer >= m_shotInterval && shotNum > 0)
            {
                // 弾を発射する
                ShootNWay(angle, m_shotAngleRange, m_shotSpeed, m_shotCount);

                // 持ち弾を減らす
                shotNum--;
                psg.SetShotGauge(shotNum);

                // 弾の発射タイミングを管理するタイマーをリセットする
                m_shotTimer = 0;

                // 近くに敵がいた場合はその敵のステートを攻撃にする
                if(GameMgr.enemyList != null)
                {
                    foreach (Enemy e in GameMgr.enemyList)
                    {
                        if ((this.transform.position - e.transform.position).sqrMagnitude < e.GetNormalEyeSightLength())
                        {
                            if (e.GetStates() != StateType.Danger)
                            {
                                e.SetRelayStagePos(Vector3.zero);
                                e.SetTargetStagePos(this.transform.position);
                                e.SetStates(StateType.Caution);
                            }

                        }
                    }
                }
                
                StartCoroutine("Stun");
            }
        }

        // マウスクリックでデコイを配置
        if (Input.GetMouseButtonDown(1))
        {
            // デコイ数をチェック
            // マウスクリックした場所から近いステージにデコイを配置する
            if (decoyNum > 0)
            {
                // メインカメラからクリックした地点のベクトルでRayを飛ばす
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // 
                RaycastHit hit = new RaycastHit();

                //レイを投げて何かのオブジェクトに当たった場合
                if (Physics.Raycast(ray, out hit))
                {
                    // 衝突したオブジェクトがステージかウェイポイントの場合はデコイ配置
                    if(hit.collider.tag == "Stage" || hit.collider.tag == "Point")
                    {
                        //レイが当たった位置(hit.point)にオブジェクトを生成する
                        currentPosition = hit.point;
                        currentPosition.y = 0;

                        Decoy decoy = Decoy.Add(GameMgr.decoyList.Count, gm, currentPosition);
                        decoy.InitilizeObjColor();
                        GameMgr.decoyList.Add(decoy);
                        decoyNum--;
                        dg.SetDecoyGauge(decoyNum);

                        decoy.StaetRemoveDecoyCoroutine();
                    }

                }
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "ShotItem")
        {
            shotNum++;
            if (shotNum > maxShotNum) shotNum = maxShotNum;
            psg.SetShotGauge(shotNum);
            col.gameObject.SetActive(false);
        }

        if(col.gameObject.tag == "DecoyItem")
        {
            decoyNum++;
            if (decoyNum > maxDecoyNum) decoyNum = maxDecoyNum;
            dg.SetDecoyGauge(decoyNum);
            col.gameObject.SetActive(false);
        }

        if(col.gameObject.tag == "EnemyShot")
        {
            follow.enabled = false;
            ss.Shake(0.25f, 0.1f, follow);
        }
    }
}
