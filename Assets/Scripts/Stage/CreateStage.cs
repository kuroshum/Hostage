using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStage : MonoBehaviour {
    [SerializeField]
    private GameObject gfloor;
    [SerializeField]
    private GameObject gstage;
    [SerializeField]
    private GameObject gwall;
    [SerializeField]
    private GameObject gwallcol;
    [SerializeField]
    private GameObject gpoint;
    [SerializeField]
    private GameObject gearth;
    [SerializeField]
    private GameObject ggate;

    public Vector3 pos;

    private Vector3 space = new Vector3(1.0f, 1.0f, 1.0f);

    private int iwidth;

    private AnaHori ah;
    private AnahoriDungeon ad;

    private int max;
    private int wall_height;

    void Start () {
        /*
         * 初期座標の設定
         */
        pos = Vector3.zero;

        wall_height = 10;
    }

    private string LoadStage(string StageFile)
    {
        /*
         * テキストの中身(ステージマップ)を読み込んで変数に保存
         */
        ReadWrite rw = GameObject.Find("Astage").GetComponent<ReadWrite>();
        string textdata = rw.Read(StageFile);

        return textdata;
    }


    public void Create(Vector3 pos, int ROOM_NUM) {

        max = 80;

        wall_height = 1;

        ah = this.GetComponent<AnaHori>();
        ad = this.GetComponent<AnahoriDungeon>();


        iwidth = 0;

        List<List<StageChip>> map = ad.Generate(ROOM_NUM, max);
        string textdata = "";

        for (int i = 0; i < max; i++)
        {
            for (int j = 0; j < max; j++)
            {
                textdata = textdata + map[i][j].type;
            }
            textdata = textdata + "\n";
        }

        GameObject obj = null;

        Vector3 init_pos;
        Vector3 floor_pos;

        GameMgr.stageCenterPos = Vector3.zero;

        int cnt = 0;
        int startPosInd = 0;
        int endPosInd = 0;

        int mapCount = 0;
        int stageCount = 0;
        int countX = 0;
        int countY = -1;

        /*
         * 変数に保存したステージマップを走査する
         * #ならCubeを生成し、Cubeの大きさだけx軸に右に移動
         * 改行文字ならz軸に下に移動して、x軸を初期化
         * 空白、-、ならそのままx軸に右に移動
         */
        foreach (char c in textdata)
        {
            countY++;
            if (c == '#' || c == 'S' || c == 'b' || c == 'G') {

                /*---------------------------------*/
                // Stage(床)を生成
                init_pos = new Vector3(pos.x, pos.y - 1, pos.z);
                obj = Instantiate(gstage, init_pos, Quaternion.identity, GameObject.Find("Astage").transform) as GameObject;
                obj.name = gstage.name;

                Stage s = new Stage();

                // ゲームオブジェクトの初期化
                s.obj = obj;
                // 確率の初期化
                s.prob = 0.0f;
                // 
                s.probIndex = stageCount;

                s.material = s.obj.GetComponent<Renderer>().material;

                s.target = new Vector3(s.obj.transform.position.x, s.obj.transform.position.y + 2, s.obj.transform.position.z);

                s.id = map[countX][countY].id;

                GameMgr.stageList.Add(s);

                stageCount++;

                GameMgr.stageCenterPos += init_pos;
                /*---------------------------------*/

                pos.x += obj.transform.lossyScale.x;
                iwidth++;
                cnt++;

                if (c == 'S')
                {
                    GameMgr.startPos = init_pos;
                }
                if (c == 'G')
                {
                    GameMgr.endPos = init_pos;
                }
            }
            else if (c == '*')
            {
                init_pos = new Vector3(pos.x, pos.y - 1, pos.z);
                floor_pos = new Vector3(pos.x, pos.y + 0.1f, pos.z);

                /*---------------------------------*/
                // Stage(床)を生成
                obj = Instantiate(gstage, init_pos, Quaternion.identity, GameObject.Find("Astage").transform) as GameObject;
                obj.name = gstage.name;

                Stage s = new Stage();

                // ゲームオブジェクトの初期化
                s.obj = obj;
                // 確率の初期化
                s.prob = 0.0f;
                // 
                s.probIndex = stageCount;

                s.material = s.obj.GetComponent<Renderer>().material;

                s.target = new Vector3(s.obj.transform.position.x, s.obj.transform.position.y + 2, s.obj.transform.position.z);

                s.id = map[countX][countY].id;

                GameMgr.stageList.Add(s);

                stageCount++;
                GameMgr.stageCenterPos += init_pos;
                /*---------------------------------*/

                // Stage(ウェイポイント)を生成
                obj = Instantiate(gpoint, floor_pos, Quaternion.identity, GameObject.Find("Apoint").transform) as GameObject;
                obj.name = gpoint.name;

                pos.x += obj.transform.lossyScale.x;
                iwidth++;
                cnt++;
            }
            else if (c == '\n') {

                obj = null;

                GameMgr.mapList.Add(new List<Map>());
                mapCount++;
                countY = -1;
                countX++;

                Vector3 origin = new Vector3((float)iwidth, 1.0f, 0f);
                pos.z -= space.z;
                pos.x -= origin.x;
                iwidth = 0;
            }

            else if (c == ' ')
            {
                pos.x += 1.0f;
                iwidth++;
            }
            else if (c == '+') {

                init_pos = new Vector3(pos.x, pos.y, pos.z);

                // Stage(障害物)を生成
                obj = Instantiate(gwall, init_pos, Quaternion.identity, GameObject.Find("Awall").transform) as GameObject;
                obj.name = gwall.name;

                Wall w = new Wall(obj, obj.GetComponent<Renderer>().material.color, false);
                GameMgr.wallList.Add(w);


                // Stage(障害物のコライダー)を生成
                obj = Instantiate(gwallcol, new Vector3(pos.x, pos.y + 1, pos.z), Quaternion.identity, GameObject.Find("Awall").transform) as GameObject;
                obj.name = gwallcol.name;

                pos.x += obj.transform.lossyScale.x;
                iwidth++;
                //pos.x += space.x;
                //iwidth++;
            }
            else if (c == '-')
            {
                for(int i = 0; i < wall_height; i++)
                {
                    init_pos = new Vector3(pos.x, pos.y, pos.z);

                    // Stage(壁)を生成
                    obj = Instantiate(gwall, init_pos, Quaternion.identity, GameObject.Find("Awall").transform) as GameObject;
                    obj.name = gwall.name;

                    Wall w = new Wall(obj, obj.GetComponent<Renderer>().material.color, false);
                    GameMgr.wallList.Add(w);

                    // Stage(壁のコライダー)を生成
                    obj = Instantiate(gwallcol, new Vector3(pos.x, pos.y + 1, pos.z), Quaternion.identity, GameObject.Find("Awall").transform) as GameObject;
                    obj.name = gwallcol.name;

                    
                }
                pos.x += obj.transform.lossyScale.x;
                iwidth++;
            }
            else if(c == 't')
            {
                init_pos = new Vector3(pos.x, pos.y - 1, pos.z);

                /*---------------------------------*/
                // Stage(床)を生成
                obj = Instantiate(gstage, init_pos, Quaternion.identity, GameObject.Find("Astage").transform) as GameObject;
                obj.name = gstage.name;

                Stage s = new Stage();

                // ゲームオブジェクトの初期化
                s.obj = obj;
                // 確率の初期化
                s.prob = 0.0f;
                // 
                s.probIndex = stageCount;

                s.material = s.obj.GetComponent<Renderer>().material;

                s.target = new Vector3(s.obj.transform.position.x, s.obj.transform.position.y + 2, s.obj.transform.position.z);

                s.id = map[countX][countY].id;

                GameMgr.stageList.Add(s);

                stageCount++;
                GameMgr.stageCenterPos += init_pos;
                /*---------------------------------*/

                init_pos = new Vector3(pos.x, pos.y, pos.z);

                // Stage(障害物)を生成
                obj = Instantiate(ggate, init_pos, Quaternion.identity, GameObject.Find("Awall").transform) as GameObject;
                obj.name = gwall.name;

                Wall w = new Wall(obj, obj.GetComponent<Renderer>().material.color, true);
                GameMgr.wallList.Add(w);

                pos.x += obj.transform.lossyScale.x;
                iwidth++;
            }

            if(c != '\n')
            {
                if (c == '*')
                {
                    GameMgr.mapList[mapCount].Add(new Map(obj, "#"));
                }
                else
                {
                    GameMgr.mapList[mapCount].Add(new Map(obj, c.ToString()));
                }
            }

        }

        GameMgr.stageCenterPos /= stageCount;

        string StageFile = Application.dataPath + "/" + "Resources" + "/" + "stage4.txt";
        ReadWrite.ListWrite(StageFile, GameMgr.mapList, max, max);

        Debug.Log("Genarate");
    }

    public void DeleteObj(string objName) {
        /*
         * ステージというタグのオブジェクトをすべて消去
         */
        //GameObject[] objs = GameObject.FindGameObjectsWithTag(objName);
        GameObject objs = GameObject.Find(objName);
        foreach (Transform obj in objs.transform)
        {
            DestroyImmediate(obj.gameObject);
        }
    }

    public void Delete()
    {
        DeleteObj("Awall");
        DeleteObj("Astage");
        DeleteObj("Apoint");
        /*
        DeleteObj("Stage");
        DeleteObj("Wall");
        DeleteObj("Point");
        DeleteObj("Earth");
        */
    }
	
}