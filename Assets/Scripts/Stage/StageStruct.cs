using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Map
{
    public GameObject obj;

    public string mapChip;

    public Map(GameObject obj, string mapChip)
    {
        this.obj = obj;
        this.mapChip = mapChip;
    }
}


public struct Stage
{
    public GameObject obj;

    public float prob;

    public float probIndex;

    public Material material;

    public Vector3 target;

    public int id;

    /*
    public Stage(GameObject obj, float prob, float prob_index)
    {
        StageObj = obj;
        StageProb = prob;
        tmpStageProb = prob;
        StageProbIndex = prob_index;
        tmpStageProbIndex = prob_index;
    }
    */
}

public struct Wall
{
    public GameObject obj;

    public Color color;

    public bool gateFlag;

    // public Vector3 target;

    //public Wall(GameObject obj, Color color, Vector3 target)
    public Wall(GameObject obj, Color color, bool gateFlag)
    {
        this.obj = obj;
        this.color = color;
        this.gateFlag = gateFlag;
        //this.target = target;
    }
}

public struct WeiPoint
{
    public GameObject StageObj;
}

// 部屋と外との境界 (ダンジョン生成用)
public struct Border
{
    public int x;
    public int y;

    public int id;

    public Border(int x, int y, int id)
    {
        this.x = x;
        this.y = y;
        this.id = id;
    }
}


// 部屋に用いる床 (ダンジョン生成用)
public class StageChip
{
    public string type;

    public int id;

    public StageChip(string type, int id)
    {
        this.type = type;
        this.id = id;
    }

    public void Set(string type, int id)
    {
        this.type = type;
        this.id = id;
    }
}

public class RoomChip
{
    public string type;
    public int x;
    public int y;

    public RoomChip(string type, int x, int y)
    {
        this.type = type;
        this.x = x;
        this.y = y;
    }

}

public enum StateType
{
    Normal,
    Caution,
    Danger
}

