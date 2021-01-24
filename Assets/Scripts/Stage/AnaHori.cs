using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnaHori : MonoBehaviour
{
    /*
    *設定する値
    */
    private int max;        //縦横のサイズ ※必ず奇数にすること

    /*
    *内部パラメータ
    */
    //private string[,] walls;      //マップの状態 0：壁 1：通路
    private int[] startPos;    //スタートの座標
    private int[] goalPos;     //ゴールの座標

    System.Random rnd;

    private string Obst = "+";

    public List<List<RoomChip>> Generate(List<List<RoomChip>> walls, int size)
    {
        this.max = size;

        rnd = new System.Random(System.Environment.TickCount);

        //スタート地点の取得
        startPos = GetStartPosition();

        //通路の生成
        //初回はゴール地点を設定する
        goalPos = MakeDungeonMap(startPos, ref walls);

        //通路生成を繰り返して袋小路を減らす
        int[] tmpStart = goalPos;
        for (int i = 0; i < size-8; i++)
        {
            MakeDungeonMap(tmpStart, ref walls);
            tmpStart = GetStartPosition();
        }

        //-----------------------------------------------------------------
        // WayPointの作成

        bool[] dir = new bool[4];

        for(int i = 0; i < max; i++)
        {
            for(int j = 0; j < max; j++)
            {
                if (i != 0 && j != 0 && i != max - 1 && j != max - 1)
                {
                    if (walls[i][j].type == Obst)
                    {
                        dir[0] = walls[i + 1][j].type == "#" ? true : false;
                        
                        dir[1] = walls[i - 1][j].type == "#" ? true : false;

                        dir[2] = walls[i][j + 1].type == "#" ? true : false;

                        dir[3] = walls[i][j - 1].type == "#" ? true : false;

                        if (walls[i + 1][j + 1].type == "#" && dir[0] == true && dir[2] == true && Check_dead_end(i, j, 1, 1, walls) == true) walls[i + 1][j + 1].type = "*";
                        if (walls[i - 1][j + 1].type == "#" && dir[1] == true && dir[2] == true && Check_dead_end(i, j, -1, 1, walls) == true) walls[i - 1][j + 1].type = "*";
                        if (walls[i + 1][j - 1].type == "#" && dir[0] == true && dir[3] == true && Check_dead_end(i, j, 1, -1, walls) == true) walls[i + 1][j - 1].type = "*";
                        if (walls[i - 1][j - 1].type == "#" && dir[1] == true && dir[3] == true && Check_dead_end(i, j, -1, -1, walls) == true) walls[i - 1][j - 1].type = "*";

                    }

                    if (walls[i][j].type == "b")
                    {
                        Debug.Log("*");
                        for(int k = -1; k < 2; k++)
                        {
                            for(int l = -1; l < 2; l++)
                            {
                                if (walls[i+k][j+l].type == "#")
                                {
                                    walls[i + k][j + l].type = "*";
                                }
                            }
                        }
                    }
                }
            }
        }

        return walls;
        //-----------------------------------------------------------------
    }

    bool Check_dead_end(int i, int j, int h, int v, List<List<RoomChip>> walls)
    {
        int cnt = 0;
        if (walls[i + h + 1][j + v + 1].type == Obst) cnt++;
        if (walls[i + h - 1][j + v + 1].type == Obst) cnt++;
        if (walls[i + h + 1][j + v - 1].type == Obst) cnt++;
        if (walls[i + h - 1][j + v - 1].type == Obst) cnt++;

        if (cnt >= 4) return false;
        else return true;
    }

    /*
    *スタート地点の取得
    */
    int[] GetStartPosition()
    {
        int i = 2;
        //ランダムでx,yを設定
        int randx = rnd.Next(i, max-i);
        int randy = rnd.Next(i, max-i);

        //x、yが両方共偶数になるまで繰り返す
        while (randx % 2 != 0 || randy % 2 != 0)
        {
            randx = Mathf.RoundToInt(rnd.Next(i, max-i));
            randy = Mathf.RoundToInt(rnd.Next(i, max-i));
        }

        return new int[] { randx, randy };
    }

    /*
    *マップ生成
    */
    int[] MakeDungeonMap(int[] _startPos, ref List<List<RoomChip>> walls)
    {
        //スタート位置配列を複製
        int[] tmpStartPos = new int[2];
        _startPos.CopyTo(tmpStartPos, 0);
        
        //移動可能な座標のリストを取得
        Dictionary<int, int[]> movePos = GetPosition(tmpStartPos, walls);

        int cnt = 0;

        //移動可能な座標がなくなるまで探索を繰り返す
        while (movePos != null)
        {
            //移動可能な座標からランダムで1つ取得し通路にする
            int[] tmpPos = movePos[rnd.Next(0, movePos.Count)];
            walls[tmpPos[0]][tmpPos[1]].type = Obst;

            //元の地点と通路にした座標の間を通路にする
            int xPos = tmpPos[0] + (tmpStartPos[0] - tmpPos[0]) / 2;
            int yPos = tmpPos[1] + (tmpStartPos[1] - tmpPos[1]) / 2;
            walls[xPos][yPos].type = Obst;

            //移動後の座標を一時変数に格納し、再度移動可能な座標を探索する
            tmpStartPos = tmpPos;
            movePos = GetPosition(tmpStartPos, walls);

            break;
        }
        //探索終了時の座標を返す
        return tmpStartPos;
    }

    /*
    *移動可能な座標のリストを取得する
    */
    Dictionary<int, int[]> GetPosition(int[] _startPos, List<List<RoomChip>> walls)
    {
        //可読性のため座標を変数に格納
        int x = _startPos[0];
        int y = _startPos[1];

        //移動方向毎に2つ先のx,y座標を仮計算
        List<int[]> position = new List<int[]> {
            new int[] {x, y + 2},
            new int[] {x, y - 2},
            new int[] {x + 2, y},
            new int[] {x - 2, y}
        };

        //移動方向毎に移動先の座標が範囲内かつ壁であるかを判定する
        //真であれば、返却用リストに追加する
        Dictionary<int, int[]> positions = position.Where(p => !isOutOfRange(p[0], p[1]) && walls[p[0]][p[1]].type == "#")
                                                   .Select((p, i) => new { p, i })
                                                   .ToDictionary(p => p.i, p => p.p);
        //移動可能な場所が存在しない場合nullを返す
        return positions.Count() != 0 ? positions : null;
    }

    //与えられたx、y座標が範囲外の場合真を返す
    bool isOutOfRange(int x, int y)
    {
        return (x < 0 || y < 0 || x >= max || y >= max);
    }

}
