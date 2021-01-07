using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    [SerializeField]
    private GameObject createStageManager;

    [SerializeField]
    private float playerMoveSpeed = 3.0f;

    [SerializeField]
    private float playerApplySpeed = 0.2f;

    [SerializeField]
    private float enemyMoveSpeed = 3.0f;

    [SerializeField]
    private float enemyApplySpeed = 0.2f;

    [SerializeField]
    private GameObject startEffect;

    [SerializeField]
    private GameObject enemyEffect;

    [SerializeField]
    private GameObject hostageEffect;

    [SerializeField]
    private GameObject cautionUI;
    public void SetActiveCautionUI(bool flag) { cautionUI.SetActive(flag); }

    [SerializeField]
    private GameObject dangerUI;
    public void SetActiveDangerUI(bool flag) { dangerUI.SetActive(flag); }

    [SerializeField]
    private GameObject keyUI;

    [SerializeField]
    private GameObject stageFoundation;

    [SerializeField]
    private GameObject shotItem;

    private GameObject mainCamera;

    private Player player;

    private Hostage hostage;

    private Follow cameraFollow;

    private ProbabilityMap pm;

    private LifeGauge lg;

    private PlayerShotGauge psg;

    private DisplayPurpose dp;

    // シーン上にあるステージ
    public static List<Stage> stageList;

    // シーン上にある壁
    public static List<Wall> wallList;

    // シーン上にあるマップ（ステージ・壁含む）
    public static List<List<Map>> mapList;

    public static List<List<int>> stageIDList;

    public static List<Enemy> enemyList;

    private Vector2 forward;

    private Stage TargetStage;

    private Color TargetColor;

    public static int ROOM_NUM = 8;

    public static int ENEMY_NUM = 10;

    public static Vector3 startPos;
    public static Vector3 endPos;
    public static Vector3 stageCenterPos;

    // 人質が救出できる距離にプレイヤーがいるかどうか
    private bool hostageFlag;

    private bool moveCameraFlag;

    public bool GetHostageFlag()
    {
        return hostageFlag;
    }

    public void StartExploson(Collision col, float time)
    {
        StartCoroutine(Exploson(col.gameObject, time));
    }

    public IEnumerator Exploson(GameObject col, float time)
    {
        ShotEffect es = ShotEffect.Add(col.transform.position.x, col.transform.position.y, col.transform.position.z);
        yield return new WaitForSeconds(time);
        es.Vanish();
    }

    private void InitilizePlayer(Vector3 startPos)
    {
        // 管理オブジェクトを生成
        Player.parent = new TokenMgr<Player>("Player", 1);

        //Vector3 startPos = stageList[startPosInd].obj.transform.position;

        player = Player.Add(0, playerMoveSpeed, playerApplySpeed, this, startPos.x, startPos.y + 1, startPos.z);

        lg = GameObject.Find("HPUI").GetComponent<LifeGauge>();
        lg.SetLifeGauge(player.GetMaxHp());
        player.InitilizeHp(lg);

        psg = GameObject.Find("ShotGauge").GetComponent<PlayerShotGauge>();
        psg.SetShotGauge(player.GetMaxShotNum());

        player.InitilizeShot();
        player.InitilizeShotGauge(psg);
        //Follow.objTarget = p.gameObject;
    }

    void Initilize_Enemy()
    {
        // 管理オブジェクトを生成
        Enemy.parent = new TokenMgr<Enemy>("Enemy", ENEMY_NUM);
        Random.InitState(10);

        int keyFlag = 0;

        /*===================================================*/
        // 敵を生成
        enemyList = new List<Enemy>(ENEMY_NUM);
        for (int i = 0; i < ENEMY_NUM; i++)
        {
            int enemyNum = i % (ROOM_NUM) + 1;
            if (i % (ROOM_NUM) + 1 == 1 || i % (ROOM_NUM) + 1 == ROOM_NUM+1)
            {
                enemyNum = Random.Range(3, ROOM_NUM + 1);
            }
            int ind = 0;
            for(int j = 0; j < stageList.Count; j++)
            {
                //
                if (stageList[j].id == enemyNum)
                {
                    ind = j;
                    if(stageList[j].id == ROOM_NUM && keyFlag == 0)
                    {
                        keyFlag++;
                    }
                    break;
                }
            }
            //Debug.Log("enemy : " + ind);
            Debug.Log(ind + " stageID : " + stageList[ind].id);
            Enemy e = Enemy.Add(enemyList.Count, stageList[ind].obj.transform.position.x, 1.0f, stageList[ind].obj.transform.position.z, stageList[ind].id, this, pm);

            var obj = Instantiate(shotItem, new Vector3(stageList[ind].obj.transform.position.x, 0.0f, stageList[ind].obj.transform.position.z), shotItem.transform.rotation);

            if (keyFlag == 1)
            {
                e.InitMgrTarget(enemyMoveSpeed / 2, player, true, dp);
                keyFlag++;
            }
            else
            {
                e.InitMgrTarget(enemyMoveSpeed / 2, player, false, dp);
            }

            //Debug.Log(stageList[0].obj.transform.position);

            e.Initilize_Shot();

            e.InitilizeUI();

            enemyList.Add(e);

            //InitilizeEffect(e.transform.position, enemyEffect);
            StartCoroutine(InitilizeEffect(e.transform.position, enemyEffect));
        }
        /*===================================================*/
    }

    private void InitilizeHostage(Vector3 startPos)
    {
        // 管理オブジェクトを生成
        Hostage.parent = new TokenMgr<Hostage>("Hostage", 1);

        //Vector3 startPos = stageList[startPosInd].obj.transform.position;

        hostage = Hostage.Add(0, playerMoveSpeed, startPos, this, player);

        StartCoroutine(InitilizeEffect(hostage.transform.position, hostageEffect));

        //Follow.objTarget = p.gameObject;
    }

    private IEnumerator InitilizeEffect(Vector3 startPos, GameObject effect)
    {
        var obj = Instantiate(effect, startPos, effect.transform.rotation);

        yield return new WaitForSeconds(3f);

        obj.SetActive(false);
    }


    private void InitilizeStartEffect(Vector3 startPos, GameObject effect)
    {
        var obj = Instantiate(effect, startPos, effect.transform.rotation);
    }

    public void Add_Enemy(Vector3 pos, int stageID)
    {
        Enemy e = Enemy.Add(enemyList.Count, pos.x, 0.75f, pos.z, stageID, this, pm);
        Stage targetStage = pm.Sort_Prob(stageList, e);

        e.InitMgrTarget(enemyMoveSpeed / 2, player, false, dp);

        enemyList.Add(e);

    }

    public void RemoveEnemy(Enemy e)
    {
        enemyList.Remove(e);
    }

    public void UpdateAlertUIStates()
    {
        int cautionCnt = 0;
        int dangerCnt = 0;
        foreach(Enemy e in enemyList)
        {
            if (e.GetCautionFlag()) cautionCnt++;
            if (e.GetDangerFlag()) dangerCnt++;
        }

        if (dangerCnt > 0)
        {
            dangerUI.SetActive(true);
            cautionUI.SetActive(false);
        }
        else if (cautionCnt > 0)
        {
            dangerUI.SetActive(false);
            cautionUI.SetActive(true);
        }
        else
        {
            dangerUI.SetActive(false);
            cautionUI.SetActive(false);
        }
    }

    private IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(1f);

        moveCameraFlag = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        stageList = new List<Stage>();
        wallList = new List<Wall>();

        mapList = new List<List<Map>>();
        mapList.Add(new List<Map>());

        stageIDList = new List<List<int>>();
        for (int i = 0; i < ROOM_NUM + 1; i++)
        {
            stageIDList.Add(new List<int>());
        }

        CreateStage cs = createStageManager.GetComponent<CreateStage>();
        cs.Create(new Vector3(-1,0,1), ROOM_NUM);

        stageFoundation.transform.position = new Vector3(stageCenterPos.x, -158f, stageCenterPos.z);

        pm = this.GetComponent<ProbabilityMap>();

        TargetColor = new Color(0.0f, 1.0f, 0.0f, 0.0f);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.position = new Vector3(stageCenterPos.x, 65f, stageCenterPos.z-30f);
        cameraFollow = mainCamera.GetComponent<Follow>();


        dp = this.GetComponent<DisplayPurpose>();


        InitilizePlayer(startPos);
        InitilizeHostage(endPos);

        Initilize_Enemy();

        InitilizeStartEffect(startPos, startEffect);

        ShotEffect.parent = new TokenMgr<ShotEffect>("Exploson", ENEMY_NUM);

        hostageFlag = false;

        moveCameraFlag = false;

        cautionUI.SetActive(false);
        dangerUI.SetActive(false);
        StartCoroutine("MoveCamera");

    }

    // Update is called once per frame
    void Update()
    {
        if(moveCameraFlag == true)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, player.transform.position + cameraFollow.offset, Time.deltaTime*25);
            if((mainCamera.transform.position - (player.transform.position + cameraFollow.offset)).sqrMagnitude < 10f)
            {
                player.SetStartFlag(true);
                cameraFollow.enabled = true;
                moveCameraFlag = false;
                foreach (Enemy e in enemyList)
                {
                    e.SetStartFlag(true);
                }
            }
        }

        // プレイヤーに人質が近づいたら人質を連れる
        if ((player.transform.position - endPos).sqrMagnitude < 3f)
        {
            // 警告ステートの場合は人質を救出できない
            int cnt = 0;
            foreach(Enemy e in enemyList)
            {
                //if (e.GetAttack()) cnt++;
                if (e.GetStates() == StateType.Danger) cnt++;
            }
            if(cnt == 0)
            {
                hostageFlag = true;
                dp.SetPurposeText("スタート地点に戻ろう");
                
                // 敵が
                if (Enemy.parent.Count() < (ENEMY_NUM / 2))
                {
                    for (int i = 0; i < ENEMY_NUM / 4; i++)
                    {
                        int enemyNum = Random.Range(1, 3);
                        int ind = 0;
                        for (int j = 0; j < stageList.Count; j++)
                        {
                            //
                            if (stageList[j].id == enemyNum)
                            {
                                ind = j;
                                break;
                            }
                        }
                        //Debug.Log("enemy : " + ind);
                        Debug.Log(ind + " stageID : " + stageList[ind].id);
                        Enemy e = Enemy.Add(enemyList.Count, stageList[ind].obj.transform.position.x, 1.0f, stageList[ind].obj.transform.position.z, stageList[ind].id, this, pm);

                        e.InitMgrTarget(enemyMoveSpeed / 2, player, false, dp);

                        //Debug.Log(stageList[0].obj.transform.position);

                        e.Initilize_Shot();

                        e.InitilizeUI();

                        e.SetStartFlag(true);

                        enemyList.Add(e);
                    }
                }
            }
        }

        // プレイヤーの周りにいない敵は描画しない
        foreach(Enemy e in enemyList)
        {
            if((player.transform.position - e.transform.position).sqrMagnitude < 300f)
            {
                e.gameObject.SetActive(true);
            }
            else
            {
                e.gameObject.SetActive(false);
            }
        }
    }
}
