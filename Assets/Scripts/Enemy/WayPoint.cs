using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    /// <summary>
    /// ウェイポイント
    /// </summary>
    private List<GameObject> points;

    private int pointNum;

    private bool wallFlag;

    /// <summary>
    /// ウェイポイントの初期化
    /// </summary>
    public void Initialize()
    {
        points = new List<GameObject>(GameObject.FindGameObjectsWithTag("Point"));
        pointNum = points.Count;
        /*
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);

        }
        */
    }

    /// <summary>
    /// 障害物があった場合は一番近いwayointを選択
    /// </summary>
    /// <param name="wallDirection"></param>
    /// <param name="targetPos"></param>
    /// <param name="EnemyPos"></param>
    /// <returns></returns>
    public Vector3 SearchWayPoint(Vector3 wallDirection, Vector3 targetPos, Vector3 EnemyPos, List<Wall> walls, Enemy enemy)
    {
        // 到達していないウェイポイントが半分未満になったらウェイポイントを初期化
        if(points.Count < pointNum/2)
        {
            Initialize();
        }
        // インデックスが0のウェイポイントと敵の距離を計算し、最小距離に設定する。
        //float minPointDirection = (points[0].transform.position - EnemyPos).sqrMagnitude*2 + (points[0].transform.position - targetPos).sqrMagnitude;
        float minPointDirection = (points[0].transform.position - EnemyPos).sqrMagnitude + (targetPos - EnemyPos).sqrMagnitude + (points[0].transform.position - targetPos).sqrMagnitude;
        //float minPointDirection = (points[0].transform.position - EnemyPos).sqrMagnitude;
        //float minPointDirection = (points[0].transform.position - this.transform.position).sqrMagnitude;
        
        // 各々のウェイポイントと敵の距離を計算し、最小の距離を計算
        int minCnt = 0;
        for (int i = 1; i < points.Count; i++)
        {
            // ウェイポイントと敵の距離を計算
            float enemyToPointDirection = (points[i].transform.position - EnemyPos).sqrMagnitude;
            
            // 
            var pointAngle = Vector3.Angle(points[i].transform.position - EnemyPos, wallDirection);
            //if (pointAngle <= 10f || wallDirection.sqrMagnitude < enemyToPointDirection)
            if (pointAngle <= 30f && wallDirection.sqrMagnitude < enemyToPointDirection)
            {
                //Debug.Log("weipoint");
                continue;
            }


            // 目的地とウェイポイントの距離を計算
            float pointToTargetDirection = (points[i].transform.position - targetPos).sqrMagnitude;

            // 目的地と敵の距離を計算
            float enemyToTargetDirection = (targetPos - EnemyPos).sqrMagnitude;
            
            // 両方の距離を足したもの
            float direction = enemyToPointDirection + enemyToTargetDirection + pointToTargetDirection;
            //float direction = enemyToPointDirection;


            // 最短の距離にあるウェイポイントが合った場合
            if (minPointDirection > direction)
            {
                wallFlag = false;
                
                /*
                // ウェイポイントと現在地までのベクトルを計算し正規化
                Vector2 start_to_end = new Vector2(points[i].transform.position.x - this.transform.position.x, points[i].transform.position.z - this.transform.position.z);

                for (int j = 0; j < walls.Count; j++)
                {
                    if (enemy.obst(walls[j].obj, start_to_end) == true)
                    {
                        wallFlag = true;
                        break;
                    }
                }
                */
                if (wallFlag == false)
                {
                    minPointDirection = direction;
                    minCnt = i;
                }
                else
                {
                    wallFlag = false;
                }
            }
        }
        targetPos = new Vector3(points[minCnt].transform.position.x, points[minCnt].transform.position.y, points[minCnt].transform.position.z);
        //points[minCnt].GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        //points.Remove(points[minCnt]);
        points.RemoveAt(minCnt);

        return targetPos;
    }

}
