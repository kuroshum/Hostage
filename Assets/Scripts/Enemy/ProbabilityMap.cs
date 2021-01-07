using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}

public class ProbabilityMap : MonoBehaviour
{

    // 敵とステージ間のベクトル
    private Vector3 stageDirection;


    // 色の変化量
    private float delta_prob = 0.8f;

    private List<int> targetStage;

    //public List<Stage> stageList;

    /*
    public void Initialize()
    {
        // ステージを取得
        GameObject[] stageObjs = GameObject.FindGameObjectsWithTag("Stage");

        // ステージ構造体の初期化
        stageList = new List<Stage>();

        // ステージ構造体のパラメータの初期化
        for (int i = 0; i < stageObjs.Length; i++)
        {
            Stage s = new Stage();

            // ゲームオブジェクトの初期化
            s.StageObj = stageObjs[i];
            // 確率の初期化
            s.StageProb = 0.0f;
            // 
            s.StageProbIndex = i;

            stageList.Add(s);
        }

        deltaColor = 0.2f;

        //Debug.Log(stageList[21].StageObj.transform.position);

    }
    */

    public int MinInds(List<Stage> stageList)
    {
        List<int> inds = new List<int>();
        for (int i = 0; i < stageList.Count; i++)
        {
            if (stageList[0].prob == stageList[i].prob)
            {
                inds.Add(i);
            }
            else
            {
                break;
            }
        }
        return inds[Random.Range(0,inds.Count)];
    }

    public Stage Sort_Prob(List<Stage> stageList, Enemy enemy)
    {
        //float minStage = Mathf.Pow(stageList[0].prob, 3) + (stageList[0].obj.transform.position - enemy.transform.position).sqrMagnitude;
        float minStage = 0; ;
        int cnt = -1;
        int stageIDCnt = 0; ;
        bool stageIDflag = false;
        List<int> minCnt = new List<int>();
        

        Extensions.Shuffle(stageList);

        foreach (Stage stage in stageList)
        {
            cnt++;
            foreach (int id in GameMgr.stageIDList[enemy.GetStageID()-1])
            {
                //Debug.Log("stageID :" + stage.id + " " + "ID :" + id);

                if (stage.id != id)
                {
                    stageIDflag = true;
                }
                else
                {
                    stageIDflag = false;
                    break;
                }
            }

            if(stage.id == enemy.GetStageID())
            {
                stageIDflag = false;
            }

            if (stageIDflag)
            {
                continue;
            }

            float valueStage = Mathf.Pow(stage.prob, 3) + (stage.obj.transform.position - enemy.transform.position).sqrMagnitude;
            if(stageIDCnt == 0)
            {
                minStage = valueStage;
                stageIDCnt++;
                minCnt.Add(cnt);
            }
            if (minStage > valueStage)
            {
                minStage = valueStage;
                minCnt.Add(cnt);
            }
        }
        
        return stageList[minCnt[Random.Range(0,minCnt.Count)]];
        
        //stageList.Sort((a, b) => a.prob.CompareTo(b.prob));

        //return stageList[MinInds(stageList)];

    }

    public void UpdateProbabilityMap(ref List<Stage> stageList, Vector3 pforward, Vector3 enemyPos)
    {
        // 扇形の視界に入ったら確率マップを更新する
        for (int i = 0; i < stageList.Count; i++)
        {
            // ステージと敵の間のベクトルを計算
            stageDirection = stageList[i].obj.transform.position - enemyPos;

            // ベクトルから角度を計算
            var angle = Vector3.Angle(pforward, stageDirection);

            // 扇形の視界に入っているステージの
            //if (angle <= 60f && stageDirection.sqrMagnitude < 30f)
            if (stageDirection.sqrMagnitude < 30f)
            {
                if (stageList[i].prob <= 1.0f)
                {
                    Stage tmpData = stageList[i];
                    tmpData.prob += delta_prob * Time.deltaTime;
                    stageList[i] = tmpData;
                }

            }
            else
            {
                if (stageList[i].prob > 0.0f)
                {
                    Stage tmpData = stageList[i];
                    tmpData.prob -= (delta_prob / (stageDirection.sqrMagnitude * 6)) * Time.deltaTime;
                    stageList[i] = tmpData;


                    //Debug.Log(stageColors[cnt]);
                }
            }

            //stageList[i].material.SetColor("_Color", new Color(stageList[i].prob, stageList[i].material.color.g, stageList[i].material.color.b, 0.0f));

            //stageList[i].obj.GetComponent<Renderer>().material.color = new Color(stageList[i].prob, 0.0f, 0.0f, 0.0f);
        }
    }
}
