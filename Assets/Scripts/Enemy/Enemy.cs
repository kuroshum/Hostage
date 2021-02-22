using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Token
{
    /// <summary>
    /// 管理オブジェクト
    /// </summary>
    public static TokenMgr<Enemy> parent = null;

    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    private GameMgr gm;
    private void SetGmaeMgr(GameMgr gm) { this.gm = gm; }

    /// <summary>
    /// 敵のID
    /// </summary>
    [SerializeField]
    private int ID;
    public void SetParam(int id) { ID = id; }

    /// <summary>
    /// 現在いる部屋のID
    /// </summary>
    [SerializeField]
    private int stageID;
    public void SetStageID(int id) { stageID = id; }
    public int GetStageID() { return stageID; }

    /// <summary>
    /// 敵のスピード
    /// </summary>
    public float speed;

    /// <summary>
    /// normalステートの場合の速度
    /// </summary>
    private float normalSpeed;
    /// <summary>
    /// dangerステートの場合の速度
    /// </summary>
    private float dangerSpeed;

    /// <summary>
    /// 壁の向き
    /// </summary>
    private Vector3 wallDirection;
    /// <summary>
    /// 目的地の方向
    /// </summary>
    private Vector3 targetDirection;
    /// <summary>
    /// プレイヤーの方向
    /// </summary>
    private Vector3 playerDirection;

    private Vector3 trailDirection;
    
    /// <summary>
    /// 目的地のステージ
    /// </summary>
    private Stage targetStage;
    /// <summary>
    /// 目的地のステージの座標
    /// </summary>
    [SerializeField]
    private Vector3 targetStagePos;
    public void SetTargetStagePos(Vector3 pos) { targetStagePos = pos; }
    
    /// <summary>
    /// 目的地に行くまでに障害物があった場合に中継する地点
    /// </summary>
    [SerializeField]
    private Stage relayStage;
    /// <summary>
    /// 目的地に行くまでに障害物があった場合に中継する地点の座標
    /// </summary>
    [SerializeField]
    private Vector3 relayStagePos;
    public void SetRelayStagePos(Vector3 pos) { this.relayStagePos = pos; }

    /// <summary>
    /// 確率マップのクラスのインスタンス
    /// </summary>
    private ProbabilityMap pm;
    private void SetProbabilityMap(ProbabilityMap pm) { this.pm = pm; }

    /// <summary>
    /// シーン上にあるウェイポイント
    /// </summary>
    public static WayPoint wp;
    
    /// <summary>
    /// プレイヤーのインスタンス
    /// </summary>
    private Player player;
    
    /// <summary>
    /// 障害物が進行方向にあるか
    /// </summary>
    private bool ObstFlag;
    public bool GetObstFlag() { return ObstFlag; }
    public bool GetMoveFlag() { return moveFlag; }
    
    /// <summary>
    /// 障害物にぶつかってからの時間
    /// </summary>
    public float obstTime;

    /// <summary>
    /// 移動するかどうかのフラグ
    /// </summary>
    [SerializeField]
    private bool moveFlag;

    /// <summary>
    /// normalステートの場合の視界角度
    /// </summary>
    private float normalEyeSightRange = 50f;
    /// <summary>
    /// cautionステートの場合の視界角度
    /// </summary>
    private float cautionEyeSightRange = 80f;
    /// <summary>
    /// dangerステートの場合の視界角度
    /// </summary>
    private float dangerEyeSightRange = 100f;

    /// <summary>
    /// normalステートの場合の視界角度
    /// </summary>
    private float normalEyeSightLength = 20f;
    public float GetNormalEyeSightLength() { return normalEyeSightLength; }
    /// <summary>
    /// cautionステートの場合の視界角度
    /// </summary>
    private float cautionEyeSightLength = 40f;
    /// <summary>
    /// dangerステートの場合の視界角度
    /// </summary>
    private float dangerEyeSightLength = 60f;

    /// <summary>
    /// 攻撃を行うときの距離
    /// </summary>
    private float dangerAttackLength = 7f;
    
    /// <summary>
    /// 速度
    /// </summary>
    public Vector3 velocity;

    /// <summary>
    /// 向き
    /// </summary>
    private Vector2 forward;

    /// <summary>
    /// 初期弾数
    /// </summary>
    [SerializeField]
    private int SHOT_NUM;

    public float m_shotSpeed; // 弾の移動の速さ
    public float m_shotAngleRange; // 複数の弾を発射する時の角度
    public float m_shotTimer; // 弾の発射タイミングを管理するタイマー
    public int m_shotCount; // 弾の発射数
    public float m_shotInterval; // 弾の発射間隔（秒）

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector3 keyOffset;

    
    [SerializeField]
    private GameObject dangerEnemyUI;

    [SerializeField]
    private GameObject cautionEnemyUI;

    [SerializeField]
    private GameObject keyEnemyUI;

    private bool cautionEnemyUIFlag;
    public bool GetCautionEnemyUIFlag() { return cautionEnemyUIFlag; }

    private bool dangerEnemyUIFlag;
    public bool GetDangerEnemyUIFlag() { return dangerEnemyUIFlag; }

    private bool dangerUIFlag;
    public bool GetDangerUIFlag() { return dangerUIFlag; }

    private bool keyEnemyUIFlag;

    private bool startFlag;
    public void SetStartFlag(bool flag) { this.startFlag = flag; }

    private bool decoyFlag;

    private Vector3 decoyPos;

    private Decoy decoy;

    private GameObject tankTower;
    public void SetTankTower() { tankTower = this.transform.Find("TankFree_Tower").gameObject; }

    private Transform canonPos;
    public void SetCanonPos() { canonPos = tankTower.transform.Find("CanonPos").transform; }


    [SerializeField]
    private StateType states;
    public StateType GetStates() { return states; }
    public void SetStates(StateType states) { this.states = states; }

    private CautionTimeManager ctm;

    private DisplayPurpose dp;

    private float trailTime;

    private GameObject trailPrefab;

    private bool trailFlag;

    private int foundTrailInd;


    public static Enemy Add(int id, float x, float y, float z, int stageID, GameMgr gm, ProbabilityMap pm)
    {
        // Enemyインスタンスの取得
        Enemy e = parent.Add(x, y, z);

        e.SetGmaeMgr(gm);

        e.SetProbabilityMap(pm);

        e.SetStageID(stageID);

        // IDを設定したり固有の処理をする
        e.SetParam(id);

        e.SetTankTower();

        e.SetCanonPos();

        return e;
    }

    public void InitMgrTarget(float speed, Player p, bool keyUIFlag, DisplayPurpose dp)
    {
        relayStagePos = Vector3.zero;


        this.moveFlag = false;

        this.speed = speed;
        normalSpeed = speed;
        dangerSpeed = speed * 2;

        //
        wp = this.gameObject.GetComponent<WayPoint>();
        wp.Initialize();

        player = p;

        SetStates(StateType.Normal);

        obstTime = 0;

        trailTime = 0;

        SelectTarget(pm, ref GameMgr.stageList);

        cautionEnemyUIFlag = false;
        dangerEnemyUIFlag = false;

        dangerUIFlag = false;

        this.keyEnemyUIFlag = keyUIFlag;

        this.dp = dp;

        startFlag = false;

        decoyFlag = false;

        trailPrefab = Resources.Load("Prefabs/" + "tankTrail") as GameObject;
    }

    public void InitilizeUI()
    {
        GameObject prefab;

        // 
        prefab = Resources.Load("Prefabs/" + "DangerUI") as GameObject;
        dangerEnemyUI = Instantiate(prefab, this.transform.position + offset, prefab.transform.rotation) as GameObject;
        dangerEnemyUI.SetActive(false);

        prefab = Resources.Load("Prefabs/" + "CautionUI") as GameObject;
        cautionEnemyUI = Instantiate(prefab, this.transform.position + offset, prefab.transform.rotation) as GameObject;
        cautionEnemyUI.SetActive(false);

        if(keyEnemyUIFlag == true)
        {
            prefab = Resources.Load("Prefabs/" + "keyUI") as GameObject;
            keyEnemyUI = Instantiate(prefab, this.transform.position + offset, prefab.transform.rotation) as GameObject;
        }

        ctm = cautionEnemyUI.transform.Find("time").GetComponent<CautionTimeManager>();
        ctm.SetTime(5f);
    }

    public void InitilizeShot()
    {
        // 管理オブジェクトを生成
        EnemyShot.parent = new TokenMgr<EnemyShot>("EnemyShot", SHOT_NUM);
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    public void LookPlayer()
    {
        Quaternion rot = Quaternion.LookRotation(player.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, speed / 30);
        tankTower.transform.rotation = Quaternion.Lerp(tankTower.transform.rotation, rot, speed / 30);
    }

    /// <summary>
    /// 目的地の方向を向く
    /// </summary>
    /// <param name="targetPos"> 目的地の座標 </param>
    public void LookTarget(Vector3 targetPos, GameObject obj)
    {
        Quaternion rot = Quaternion.LookRotation(targetPos - this.transform.position);
        if (Vector3.Angle(obj.transform.forward, (targetPos - this.transform.position).normalized) > 2f)
        {
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, rot, speed / 30);
        }
        else
        {
            if (decoy == null || decoy.GetIsDeath() != false)
            {
                moveFlag = true;
            }

        }

    }


    /// <summary>
    /// 砲塔を回転させる
    /// </summary>
    public void RotateCanon()
    {
        tankTower.transform.RotateAround(this.transform.position, Vector3.up, Time.deltaTime * 90);
    }


    // 指定された 2 つの位置から角度を求めて返す
    public float GetAngle(Vector3 from, Vector3 to)
    {
        var dx = to.x - from.x;
        var dz = to.z - from.z;
        var rad = Mathf.Atan2(dz, dx);
        return rad * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 目的地に移動する
    /// </summary>
    public void MoveTarget()
    {
        // 中継地点がある場合は中継地点を目的地に設定
        Vector3 targetPos = targetStagePos;
        if(relayStagePos != Vector3.zero)
        {
            targetPos = relayStagePos;
        }

        // 目的地の方向を向く
        LookTarget(targetPos, this.gameObject);

        // 目的地の方向を向いたら移動する
        if (moveFlag == true)
        {
            // プレイヤーを攻撃している場合は、プレイヤーと重ならないようにする
            if(states == StateType.Danger && (player.transform.position - this.transform.position).sqrMagnitude < dangerAttackLength)
            {
                return;
            }

            // 目的地に移動
            velocity = (targetPos - this.transform.position).normalized * speed;
            transform.position += velocity * Time.deltaTime;
        }

    }
    
    /// <summary>
    /// 目的地に到達できているかを確認する関数
    /// </summary>
    public bool IsReachTarget()
    {
        // 中継地点を設定している場合
        if(relayStagePos != Vector3.zero)
        {
            return Vector3.SqrMagnitude(relayStagePos - this.transform.position) < 0.4f ? true : false;
        }
        // 中継地点が設定されていない（目的地までに障害物が無い）場合
        else
        {
            return Vector3.SqrMagnitude(targetStagePos - this.transform.position) < 0.4f ? true : false;
        }

    }

    /// <summary>
    /// 目的地を設定する関数
    /// </summary>
    /// <param name="pm"> 確率マップからプレイヤーがいる確率の高い座標を選択するためのインスタンス </param>
    /// <param name="stageList"> 保存している確率マップを取り出す用のリスト </param>
    public void SelectTarget(ProbabilityMap pm, ref List<Stage> stageList)
    {
        // 中継地点が設定されている場合、中継地点に到着したので中継地点を初期化
        if (relayStagePos != Vector3.zero)
        {
            relayStagePos = Vector3.zero;
            return;
        }
        // 中継地点が設定されていない場合、目的地に到着したので次の目的地を設定
        else
        {
            // 次の目的地を設定
            targetStage = pm.Sort_Prob(stageList, this);

            // 次の目的地の座標を設定
            targetStagePos = new Vector3(targetStage.obj.transform.position.x, targetStage.obj.transform.position.y + 1, targetStage.obj.transform.position.z);

            wp.Initialize();

            //
            moveFlag = false;

            SetStageID(targetStage.id);
        }

    }


    /// <summary>
    /// 障害物があるかを探索
    /// </summary>
    /// <param name="wall">  </param>
    /// <param name="start_to_end"> 目的地と現在地までの正規化ベクトル </param>
    /// <returns></returns>
    public bool obst(GameObject wall, Vector2 start_to_end)
    {
        Vector2 normal_start_to_end = start_to_end.normalized;

        // 壁と現在地のベクトルと正規化ベクトル
        Vector2 start_to_wall = new Vector2(wall.transform.position.x - this.transform.position.x, wall.transform.position.z - this.transform.position.z);
        Vector2 normal_start_to_wall = start_to_wall.normalized;

        // 目的地と壁のベクトルと正規化ベクトル
        Vector2 end_to_wall = new Vector2(wall.transform.position.x - targetStagePos.x, wall.transform.position.z - targetStagePos.z);

        // 
        float angle = Vector2.Angle(normal_start_to_end, normal_start_to_wall);

        if (angle < 20f && start_to_end.magnitude > start_to_wall.magnitude) 
        {
            if (start_to_wall.magnitude < 10f || end_to_wall.magnitude < 2f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 目的地までの直線上に障害物があるかを探索し、中継地点を選択
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="walls"></param>
    public void SelectTarget_in_Obst(Vector2 forward, List<Wall> walls)
    {
        // 目的地と現在地までのベクトルを計算し正規化
        Vector2 start_to_end = new Vector2(targetStagePos.x - this.transform.position.x, targetStagePos.z - this.transform.position.z);
        Vector2 normal_start_to_end = start_to_end.normalized;

        // 障害物を検索
        //foreach (Wall wall in walls)
        for (int i = 0; i < walls.Count; i++)
        {
            // 目的地までの直線に障害物があれば
            if (obst(walls[i].obj, start_to_end) == true)
            {
                // 壁と現在地との距離
                wallDirection = walls[i].obj.transform.position - this.transform.position;

                // 中継地点を探す
                if(relayStagePos == Vector3.zero)
                {
                    relayStagePos = wp.SearchWayPoint(wallDirection, targetStagePos, this.transform.position, walls, this);
                }
                break;
            }

        }

    }

    /// <summary>
    /// プレイヤーを探索
    /// </summary>
    /// <param name="playerPos"> プレイヤーの座標 </param>
    public void SearchPlayer(Vector3 playerPos, float length, float range, Decoy decoy, StateType state)
    {
        // プレイヤーと敵の間のベクトルを計算
        playerDirection = playerPos - this.transform.position;

        // プレイヤーと敵の間の距離が一定より近い場合
        if( playerDirection.sqrMagnitude < length)
        {
            // ベクトルから角度を計算
            var angle = Vector3.Angle(tankTower.transform.forward, playerDirection);

            // プレイヤーが扇形の視界に入っている場合
            if (angle <= range)
            {
                // プレイヤーと敵との間に障害物があるかを判定
                bool obstPlayerFlag = false;
                foreach(Wall s in GameMgr.wallList)
                {
                    Vector3 stageDirection = s.obj.transform.position - this.transform.position;
                    var stageAngle = Vector3.Angle(playerDirection, stageDirection);

                    if (stageAngle <= range && stageDirection.sqrMagnitude < playerDirection.sqrMagnitude)
                    {
                        Vector3 d = this.transform.position + Vector3.Project(stageDirection, playerDirection);
                        if ((d - s.obj.transform.position).sqrMagnitude < 1f)
                        {
                            obstPlayerFlag = true;
                            break;
                        }
                    }
                }

                // プレイヤーと敵との間に障害物がない場合
                if(obstPlayerFlag == false)
                {
                    // 次の目的地をプレイヤーの座標に設定
                    targetStagePos = playerPos;

                    relayStagePos = Vector3.zero;

                    // 攻撃ステートに設定
                    SetStates(state);

                    if (state == StateType.Danger)
                    {
                        speed = dangerSpeed;
                    }
                    else
                    {
                        speed = normalSpeed;
                    }

                    this.decoy = decoy;
                }
            }
        }
    }

    public void SearchPlayerTrail(Vector3 trailPos, GameObject trailObj, float length, float range, Decoy decoy, StateType state)
    {
        // プレイヤーと敵の間のベクトルを計算
        trailDirection = trailPos - this.transform.position;

        // プレイヤーと敵の間の距離が一定より近い場合
        if (trailDirection.sqrMagnitude < length)
        {
            // ベクトルから角度を計算
            var angle = Vector3.Angle(tankTower.transform.forward, trailDirection);

            // プレイヤーが扇形の視界に入っている場合
            if (angle <= range)
            {
                // プレイヤーと敵との間に障害物があるかを判定
                bool obstPlayerFlag = false;
                foreach (Wall s in GameMgr.wallList)
                {
                    Vector3 stageDirection = s.obj.transform.position - this.transform.position;
                    var stageAngle = Vector3.Angle(trailDirection, stageDirection);

                    if (stageAngle <= range && stageDirection.sqrMagnitude < trailDirection.sqrMagnitude)
                    {
                        Vector3 d = this.transform.position + Vector3.Project(stageDirection, trailDirection);
                        if ((d - s.obj.transform.position).sqrMagnitude < 1f)
                        {
                            obstPlayerFlag = true;
                            break;
                        }
                    }
                }

                // プレイヤーと敵との間に障害物がない場合
                if (obstPlayerFlag == false)
                {
                    // 次の目的地をプレイヤーの座標に設定
                    targetStagePos = trailPos;

                    relayStagePos = Vector3.zero;

                    // 攻撃ステートに設定
                    SetStates(state);

                    if (state == StateType.Danger)
                    {
                        speed = dangerSpeed;
                    }
                    else
                    {
                        speed = normalSpeed;
                    }

                    this.decoy = decoy;

                    trailFlag = true;

                    foundTrailInd = GameMgr.trailList.IndexOf(trailObj);
                }
            }
        }
    }


    /// <summary>
    /// 弾を発射する関数
    /// </summary>
    /// <param name="angleBase"></param>
    /// <param name="angleRange"></param>
    /// <param name="speed"></param>
    /// <param name="count"></param>
    private void ShootNWay(float angleBase, float angleRange, float speed, int count)
    {
        // var pos = transform.position + transform.forward; // プレイヤーの位置
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
                var shot = EnemyShot.Add(this.gameObject.tag, pos.x, pos.y, pos.z, player);

                // 弾を発射する方向と速さを設定する
                shot.Init(angle, speed, gm);

            }
        }
        // 弾を 1 つだけ発射する場合
        else if (count == 1)
        {
            // 発射する弾を生成する
            var shot = EnemyShot.Add(this.gameObject.tag, pos.x, pos.y, pos.z, player);

            // 弾を発射する方向と速さを設定する
            shot.Init(angleBase, speed, gm);

            shot.StartShotEffect();
        }
    }

    
    /// <summary>
    /// 弾を発射する関数
    /// </summary>
    /// <param name="playerPos"></param>
    public void Shoot(Vector3 playerPos)
    {
        // 弾の発射タイミングを管理するタイマーを更新する
        m_shotTimer += Time.deltaTime;

        // まだ弾の発射タイミングではない場合は、ここで処理を終える
        if (m_shotTimer < m_shotInterval) return;
        
        // 弾を発射する
        ShootNWay(GetAngle(this.transform.position, playerPos), m_shotAngleRange, m_shotSpeed, m_shotCount);

        // 弾の発射タイミングを管理するタイマーをリセットする
        m_shotTimer = 0;
    }

    


    int to_binary(float num)
    {
        return num > 0 ? 1 : -1;
    }

    public void FollowUI(GameObject UI, Vector3 offset)
    {
        UI.transform.localPosition = this.transform.localPosition + offset;
    }

    
    

    /*===================================================*/
    // 更新処理

    void Update()
    {
        if (startFlag == false) return;

        // 障害物にぶつかっている時間を計測
        if (ObstFlag) obstTime += Time.deltaTime;

        // 障害物に一定時間ぶつかっていた場合は障害物があると判定
        if (obstTime > 2.0f)
        {
            relayStagePos = Vector3.zero;
            obstTime = 0;
            ObstFlag = false;

            // 次の目的地を設定
            targetStage = pm.Sort_Prob(GameMgr.stageList, this);

            // 次の目的地の座標を設定
            targetStagePos = new Vector3(targetStage.obj.transform.position.x, targetStage.obj.transform.position.y + 1, targetStage.obj.transform.position.z);

            // ウェイポイントを初期化
            wp.Initialize();

            // 目的地の方向を向く前は移動しない
            moveFlag = false;
        }

        // 確率マップを更新
        pm.UpdateProbabilityMap(ref GameMgr.stageList, this.transform.forward, this.transform.position);


        switch (states)
        {
            // 通常ステートの場合
            case StateType.Normal:
                
                // 通常の巡回
                Patrol(normalEyeSightLength, normalEyeSightRange);


                // 注意・警告のUIを非表示にする
                dangerEnemyUIFlag = false;
                cautionEnemyUIFlag = false;

                dangerUIFlag = false;

                break;
            
            // 注意ステートの場合
            case StateType.Caution:

                if (trailFlag == true)
                {
                    // 現在見つけている跡より一つ新しい跡を取得
                    if(GameMgr.trailList.Count > foundTrailInd)
                    {
                        Vector3 pos = GameMgr.trailList[foundTrailInd].transform.position;
                        pos = new Vector3(pos.x, player.transform.position.y, pos.z);
                        // プレイヤーの跡を追跡
                        CautionFollowTrail(cautionEyeSightLength, cautionEyeSightRange, pos);
                    }
                    else
                    {
                        trailFlag = false;
                    }

                }
                else
                {
                    // 注意ステートの巡回
                    CautionPatrol(cautionEyeSightLength, cautionEyeSightRange, player.transform.position);
                }


                // 注意のUIを表示する
                dangerEnemyUIFlag = false;
                cautionEnemyUIFlag = true;

                dangerUIFlag = false;

                // cautionステートの時間を計測する
                ctm.UpdateCautionTime(this);

                break;
            
            // 警告ステートの場合
            case StateType.Danger:

                //targetStagePos = player.transform.position;

                // 生きているデコイを検知した場合
                if (decoy != null && decoy.GetIsDeath() == false)
                {
                    // その場でデコイに攻撃する
                    moveFlag = false;
                    DecoyAttack(decoy.transform.position);

                    dangerUIFlag = false;

                }
                else
                {
                    // デコイが死んだのを検知した場合
                    if (decoy != null && decoy.GetIsDeath() == true)
                    {
                        // 通常ステートに移行する
                        SetStates(StateType.Normal);
                        speed = normalSpeed;

                        dangerUIFlag = false;

                        //break;
                    }
                    // プレイヤーを検知した場合
                    else
                    {
                        // プレイヤーを検知した場合はプレイヤーを攻撃する
                        moveFlag = true;
                        Attack(player.transform.position);

                        dangerUIFlag = true;
                    }


                }

                // 警告のUIを表示する
                dangerEnemyUIFlag = true;
                cautionEnemyUIFlag = false;

                break;
        }

        dangerEnemyUI.SetActive(dangerEnemyUIFlag);
        cautionEnemyUI.SetActive(cautionEnemyUIFlag);

        forward.x = to_binary(this.transform.forward.x);
        forward.y = to_binary(this.transform.forward.y);

        // 障害物があった場合に回避するためのポイントを検索し設定
        SelectTarget_in_Obst(forward, GameMgr.wallList);

        // ターゲットの方向に移動する
        MoveTarget();


        // UIを敵と一緒に移動させるs
        FollowUI(cautionEnemyUI, offset);
        FollowUI(dangerEnemyUI, offset);

        // 鍵持ち持ちの場合は鍵UIを一緒に移動させる
        if(keyEnemyUIFlag == true)
        {
            FollowUI(keyEnemyUI, keyOffset);
        }

    }
    /*===================================================*/

    
    /// <summary>
    /// デコイを攻撃する
    /// </summary>
    /// <param name="pos"> デコイの座標 </param>
    public void DecoyAttack(Vector3 pos)
    {
        // デコイの方向を向く
        LookTarget(pos, tankTower);
        LookTarget(pos, this.gameObject);

        // デコイに向かって弾を打つ
        Shoot(pos);
    } 

    /// <summary>
    /// プレイヤーを攻撃する
    /// </summary>
    /// <param name="pos"> プレイヤーの座標 </param>
    public void Attack(Vector3 pos)
    {
        //
        targetStagePos = player.transform.position;

        // プレイヤーと敵の位置が一定距離離れた場合、通常ステートに移行する
        if ((pos - transform.position).sqrMagnitude > dangerEyeSightLength)
        {
            SetStates(StateType.Normal);
            speed = normalSpeed;
        }

        // 目的の位置に移動できているか
        if (IsReachTarget())
        {
            // 中継地点に移動した場合は中継地点をリセットする
            if (relayStagePos != Vector3.zero)
            {
                relayStagePos = Vector3.zero;
            }
            // 目的地点に移動した場合は目的地点を更新する
            else
            {
                targetStagePos = pos;
            }
        }

        // プレイヤーの方向を向く
        LookPlayer();

        // プレイヤーに向かって弾を打つ
        Shoot(pos);

    }

    public void CautionFollowTrail(float length, float range, Vector3 pos)
    {
        // 目的位置に移動できているか
        if (IsReachTarget())
        {
            // 中継地点に移動した場合は中継地点をリセットする
            if (relayStagePos != Vector3.zero)
            {
                relayStagePos = Vector3.zero;
            }
            // 目的地点に移動した場合は目的地点を更新する
            else
            {
                //targetStagePos = player.transform.position;
                targetStagePos = pos;
                foundTrailInd++;
            }
        }

        // デコイを探索
        foreach (Decoy d in GameMgr.decoyList)
        {
            if (d.GetIsDeath() == false)
            {
                SearchPlayer(d.transform.position, length, range, d, StateType.Danger);
            }
            else
            {
                decoyFlag = false;
            }
        }

        
        // デコイとプレイヤーの跡が見つからなかった場合、プレイヤーを探索
        if (decoyFlag == false)
        {
            SearchPlayer(player.transform.position, length, range, null, StateType.Danger);

        }
    }


    /// <summary>
    /// cautionステートの場合の巡回
    /// </summary>
    /// <param name="length"> 敵の視界の長さ </param>
    /// <param name="range"> 敵の視界の角度 </param>
    public void CautionPatrol(float length, float range, Vector3 pos)
    {
        targetStagePos = player.transform.position;

        // 目的位置に移動できているか
        if (IsReachTarget())
        {
            // 中継地点に移動した場合は中継地点をリセットする
            if (relayStagePos != Vector3.zero)
            {
                relayStagePos = Vector3.zero;
            }
            // 目的地点に移動した場合は目的地点を更新する
            else
            {
                //targetStagePos = player.transform.position;
                targetStagePos = pos;
            }
        }

        // デコイを探索
        foreach (Decoy d in GameMgr.decoyList)
        {
            if (d.GetIsDeath() == false)
            {
                SearchPlayer(d.transform.position, length, range, d, StateType.Danger);
            }
            else
            {
                decoyFlag = false;
            }
        }

        // デコイが見つからなかった場合、プレイヤーを探索
        if (decoyFlag == false)
        {
            SearchPlayer(player.transform.position, length, range, null, StateType.Danger);
            
        }

        // プレイヤーの方向を向く
        LookPlayer();


    }

    /// <summary>
    /// normalステートの場合の巡回
    /// </summary>
    /// <param name="length"> 敵の視界の長さ </param>
    /// <param name="range"> 敵の視界の角度 </param>
    public void Patrol(float length, float range)
    {
        // 目的地に移動できているか
        if (IsReachTarget())
        {
            // 中継地点に移動した場合は中継地点をリセットする
            if (relayStagePos != Vector3.zero)
            {
                relayStagePos = Vector3.zero;
            }
            // 目的地点に移動した場合は目的地点を更新する
            else
            {
                SelectTarget(pm, ref GameMgr.stageList);
            }

        }

        // デコイを探索
        foreach (Decoy d in GameMgr.decoyList)
        {
            if (d.GetIsDeath() == false)
            {
                SearchPlayer(d.transform.position, length, range, d, StateType.Danger);
            }
            else
            {
                decoyFlag = false;
            }
        }

        // デコイが見つからなかった場合、プレイヤーを探索
        if (decoyFlag == false)
        {
            // プレイヤーを探索
            SearchPlayer(player.transform.position, length, range, null, StateType.Danger);

            // プレイヤーの跡を探索
            foreach (GameObject obj in GameMgr.trailList)
            {
                var pos = new Vector3(obj.transform.position.x, player.transform.position.y, obj.transform.position.z);
                SearchPlayerTrail(pos, obj, length, range, null, StateType.Caution);
            }
        }

        RotateCanon();

    }


    /*===================================================*/
    // あたり判定の処理

    void OnTriggerEnter(Collider col)
    {
        // 弾にあたったら
        if (col.gameObject.tag == "Shot")
        {
            gm.RemoveEnemy(this);

            foreach(Enemy e in GameMgr.enemyList)
            {
                if((this.transform.position - e.transform.position).sqrMagnitude < normalEyeSightLength)
                {
                    //e.SetDangerFlag(true);
                    //targetStagePos = player.transform.position;
                    e.SetRelayStagePos(Vector3.zero);
                    e.SetTargetStagePos(player.transform.position);
                    e.SetStates(StateType.Caution);
                }
            }

            dangerEnemyUI.SetActive(false);
            cautionEnemyUI.SetActive(false);
            if (keyEnemyUIFlag == true)
            {
                keyEnemyUI.SetActive(false);
                foreach (Wall w in GameMgr.wallList)
                {
                    if (w.gateFlag)
                    {
                        w.obj.SetActive(false);
                    }
                }
                dp.SetPurposeText("人質の場所に向かおう");
                Debug.Log("Gate Open");
            }

            Vanish();
        }

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "WallCol")
        {
            ObstFlag = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "WallCol")
        {
            ObstFlag = false;
            obstTime = 0;
        }
    }

    /*===================================================*/

}
