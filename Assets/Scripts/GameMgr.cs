using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private Text cautionUIText;

    [SerializeField]
    private GameObject dangerUI;
    public void SetActiveDangerUI(bool flag) { dangerUI.SetActive(flag); }

    private Text dangerUIText;

    [SerializeField]
    private GameObject keyUI;

    [SerializeField]
    private GameObject purposeUI;

    [SerializeField]
    private GameObject stageFoundation;

    [SerializeField]
    private GameObject itemBox;

    [SerializeField]
    private GameObject shotItem;

    [SerializeField]
    private GameObject decoyItem;

    private GameObject mainCamera;

    private Player player;

    private Hostage hostage;

    private Follow cameraFollow;

    private ProbabilityMap pm;

    private LifeGauge lg;

    private PlayerShotGauge psg;

    private DecoyGauge dg;

    private DisplayPurpose dp;

    // シーン上にあるステージ
    public static List<Stage> stageList;

    // シーン上にある壁
    public static List<Wall> wallList;

    // シーン上にあるマップ（ステージ・壁含む）
    public static List<List<Map>> mapList;

    public static List<List<int>> stageIDList;

    public static List<Enemy> enemyList;

    public static List<Decoy> decoyList;

    private Vector2 forward;

    private Stage TargetStage;

    private Color TargetColor;

    private float rad = 0;

    public static int ROOM_NUM;

    public static int ENEMY_NUM;

    public static Vector3 startPos;
    public static Vector3 endPos;
    public static Vector3 stageCenterPos;

    // 人質が救出できる距離にプレイヤーがいるかどうか
    private bool hostageFlag;

    private bool moveCameraFlag;

    public static List<GameObject> trailList;

    private PopItem pi;

    public bool GetHostageFlag()
    {
        return hostageFlag;
    }

    public void StartExploson(Collider col, float time)
    {
        StartCoroutine(Exploson(col.gameObject, time));
    }

    public IEnumerator Exploson(GameObject col, float time)
    {
        ShotEffect es = ShotEffect.Add(col.transform.position.x, col.transform.position.y, col.transform.position.z);
        yield return new WaitForSeconds(time);
        es.Vanish();
    }

    public void StartStartExploson(Vector3 pos, float time)
    {
        StartCoroutine(StartExploson(pos, time));
    }

    public IEnumerator StartExploson(Vector3 pos, float time)
    {
        ShotStartEffect ess = ShotStartEffect.Add(pos.x, pos.y, pos.z);
        yield return new WaitForSeconds(time);
        ess.Vanish();
    }

    private void InitilizeDecoy()
    {
        Decoy.parent = new TokenMgr<Decoy>("Decoy", 10);

        decoyList = new List<Decoy>(10);
    }

    private void InitilizePlayer(Vector3 startPos, GameObject camera)
    {
        // 管理オブジェクトを生成
        Player.parent = new TokenMgr<Player>("Player", 1);

        //Vector3 startPos = stageList[startPosInd].obj.transform.position;

        player = Player.Add(0, playerMoveSpeed, playerApplySpeed, this, startPos.x, startPos.y + 1, startPos.z, camera);

        lg = GameObject.Find("HPUI").GetComponent<LifeGauge>();
        lg.SetLifeGauge(player.GetMaxHp());
        player.InitilizeHp(lg);

        psg = GameObject.Find("ShotGauge").GetComponent<PlayerShotGauge>();
        psg.SetShotGauge(player.GetMaxShotNum());

        dg = GameObject.Find("DecoyGauge").GetComponent<DecoyGauge>();
        dg.SetDecoyGauge(1);

        player.InitilizeShot();
        player.InitilizeShotGauge(psg);
        player.InitilizeDecoyGauge(dg);

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player.InitilizeShakeScreen();
        //Follow.objTarget = p.gameObject;

        trailList = new List<GameObject>();
    }

    void InitilizeEnemy()
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
            // アイテムボックスを生成
            if (i % 2 == 0)
            {
                Vector3 itemPos = new Vector3(stageList[ind].obj.transform.position.x, 0.0f, stageList[ind].obj.transform.position.z);
                var obj = Instantiate(itemBox, itemPos, itemBox.transform.rotation);
                pi = obj.GetComponent<PopItem>();
                pi.Init(this, null);
            }

            // 敵を生成
            Enemy e = Enemy.Add(enemyList.Count, stageList[ind].obj.transform.position.x, 1.0f, stageList[ind].obj.transform.position.z, stageList[ind].id, this, pm);
            var itemObj = Instantiate(itemBox, Vector3.zero, itemBox.transform.rotation);
            //itemObj.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

            pi = itemObj.GetComponent<PopItem>();
            pi.Init(this, e);

            // ショットアイテムとデコイアイテムを生成
            /*
            if (Random.Range(0,2) == 0)
            {
                var obj = Instantiate(shotItem, new Vector3(stageList[ind].obj.transform.position.x, 0.0f, stageList[ind].obj.transform.position.z), shotItem.transform.rotation);
            }
            else
            {
                var obj = Instantiate(decoyItem, new Vector3(stageList[ind].obj.transform.position.x, 0.0f, stageList[ind].obj.transform.position.z), decoyItem.transform.rotation);
            }
            */

            // 敵に鍵持ちのステータスを設定する
            if (keyFlag == 1)
            {
                e.InitMgrTarget(enemyMoveSpeed / 2, player, true, dp, pi);
                keyFlag++;
            }
            else
            {
                e.InitMgrTarget(enemyMoveSpeed / 2, player, false, dp, pi);
            }

            //Debug.Log(stageList[0].obj.transform.position);

            e.InitilizeShot();

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
            if (e.GetCautionEnemyUIFlag()) cautionCnt++;
            if (e.GetDangerUIFlag()) dangerCnt++;
        }

        if (dangerCnt > 0)
        {
            dangerUI.SetActive(true);
            cautionUI.SetActive(false);
            rad += Time.deltaTime * Mathf.PI * 1.5f;
            dangerUIText.color = new Color(dangerUIText.color.r, dangerUIText.color.g, dangerUIText.color.b, Mathf.Cos(rad));
        }
        else if (cautionCnt > 0)
        {
            dangerUI.SetActive(false);
            cautionUI.SetActive(true);
            rad += Time.deltaTime * Mathf.PI * 1.5f;
            cautionUIText.color = new Color(cautionUIText.color.r, cautionUIText.color.g, cautionUIText.color.b, Mathf.Cos(rad));
        }
        else
        {
            dangerUI.SetActive(false);
            cautionUI.SetActive(false);
            rad = 0;
        }
    }

    private IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(1f);

        moveCameraFlag = true;
    }

    public GameObject InstantiateShotItem(Vector3 pos)
    {
        GameObject obj = Instantiate(shotItem, pos, shotItem.transform.rotation);
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        return obj;
    }

    public GameObject InstantiateDecoyItem(Vector3 pos)
    {
        GameObject obj = Instantiate(decoyItem, pos, decoyItem.transform.rotation);
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        return obj;
    }

    private void Awake()
    {
        enemyList = null;
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

        if(SceneManager.GetActiveScene().name != "Select")
        {
            // ステージを自動生成
            CreateStage cs = createStageManager.GetComponent<CreateStage>();
            cs.Create(new Vector3(-1, 0, 1), ROOM_NUM);
        }
        else
        {
            stageCenterPos = Vector3.zero;
        }

        

        stageFoundation.transform.position = new Vector3(stageCenterPos.x, -158f, stageCenterPos.z);

        pm = this.GetComponent<ProbabilityMap>();

        TargetColor = new Color(0.0f, 1.0f, 0.0f, 0.0f);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.position = new Vector3(stageCenterPos.x, 65f, stageCenterPos.z-30f);
        cameraFollow = mainCamera.GetComponent<Follow>();
        Camera camera= mainCamera.GetComponent<Camera>();


        dp = this.GetComponent<DisplayPurpose>();


        InitilizePlayer(startPos, mainCamera);

        if (SceneManager.GetActiveScene().name != "Select")
        {
            InitilizeHostage(endPos);
            InitilizeDecoy();

            InitilizeEnemy();

            InitilizeStartEffect(startPos, startEffect);
        }
        else
        {
            mainCamera.transform.position = new Vector3(0f, -90f, -5f);
            player.transform.position = new Vector3(0, -104, 0);

            dp.SetPurposeText("難易度を選ぼう");
        }


        ShotEffect.parent = new TokenMgr<ShotEffect>("Exploson", ENEMY_NUM);
        ShotStartEffect.parent = new TokenMgr<ShotStartEffect>("Exploson7", ENEMY_NUM);

        hostageFlag = false;

        moveCameraFlag = false;

        dangerUIText = dangerUI.GetComponentInChildren<Text>();
        cautionUIText = cautionUI.GetComponentInChildren<Text>();

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
                if(enemyList != null)
                {
                    foreach (Enemy e in enemyList)
                    {
                        e.SetStartFlag(true);
                    }
                }
                
            }
        }

        if(player.GetHp() <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }

        if(hostageFlag==true && (player.transform.position - startPos).sqrMagnitude < 2f)
        {
            SceneManager.LoadScene("GameClear");
        }


        if (SceneManager.GetActiveScene().name != "Select")
        {
            UpdateAlertUIStates();

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
                            //Debug.Log(ind + " stageID : " + stageList[ind].id);
                            Enemy e = Enemy.Add(enemyList.Count, stageList[ind].obj.transform.position.x, 1.0f, stageList[ind].obj.transform.position.z, stageList[ind].id, this, pm);
                            var itemObj = Instantiate(itemBox, Vector3.zero, itemBox.transform.rotation);
                            pi = itemObj.GetComponent<PopItem>();
                            pi.Init(this, e);

                            e.InitMgrTarget(enemyMoveSpeed / 2, player, false, dp, pi);

                            //Debug.Log(stageList[0].obj.transform.position);

                            e.InitilizeShot();

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
}
