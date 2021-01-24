using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnahoriDungeon : MonoBehaviour
{
    /// <summary>
    ///縦横のサイズ ※必ず奇数にすること
    /// </summary>
    private int max;

    /// <summary>
    /// マップの状態 0：壁 1：通路
    /// </summary>
    private List<List<StageChip>> map;

    /// <summary>
    /// スタートの座標
    /// </summary>
    private int[] startPos;

    /// <summary>
    /// ゴールの座標
    /// </summary>
    private int[] goalPos;

    private int Vert;
    private int Hori;

    private int minSize = 5;
    private int maxSize = 9;

    /// <summary>
    /// ステージ上の壁
    /// </summary>
    private List<Border> border;

    /// <summary>
    /// ステージに存在する部屋の情報を格納する変数
    /// </summary>
    private List<List<RoomChip>> room;

    /// <summary>
    /// 障害物生成用のクラス
    /// </summary>
    private AnaHori ah;

    int[] startRoomPos;
    Border startBorder;

    System.Random rnd;

    private string Obst = "+";

    /// <summary>
    /// 引数値を中心とした連番数列を作成
    /// 例：5が引数の場合、0～10の数列を作成
    /// </summary>
    /// <param name="randx"></param>
    /// <returns></returns>
    public int[] CreateRange(int randx)
    {
        int[] range = new int[randx * 2 + 1];
        for (int i = 0; i < randx * 2 + 1; i++)
        {
            range[i] = -randx + i;
        }

        return range;
    }

    /// <summary>
    /// 障害物を作成
    /// </summary>
    /// <param name="len"></param>
    /// <param name="tmp_room"></param>
    public void MakeObst(int len, List<RoomChip> tmp_room)
    {
        int size = len * 2 - 1;
        int cnt = 0;
        room = new List<List<RoomChip>>(size);
        for (int i = 0; i < size; i++)
        {
            room.Add(new List<RoomChip>(size));
            for (int j = 0; j < size; j++)
            {
                room[i].Add(new RoomChip(tmp_room[cnt].type, tmp_room[cnt].x, tmp_room[cnt].y));
                cnt++;
            }
        }

        room = ah.Generate(room, room.Count);

        for (int i = 0; i < room.Count; i++)
        {
            for (int j = 0; j < room.Count; j++)
            {
                map[room[i][j].x][room[i][j].y].type = room[i][j].type;
            }
        }
    }

    /// <summary>
    /// 部屋を作成する関数
    /// </summary>
    /// <param name="startPos"> 部屋を作成する座標（部屋の真ん中） </param>
    /// <param name="id"> 部屋のID </param>
    /// <param name="len"> 部屋の半径サイズ </param>
    /// <returns></returns>
    public List<RoomChip> MakeRoom(int[] startPos, int id, int len)
    {
        // 連番数列を作成
        int[] size = CreateRange(len);

        // 部屋のリストのインスタンス生成
        List<RoomChip> tmp_room = new List<RoomChip>();

        // 部屋に壁と床を設定
        for (int i = 0; i < size.Length; i++)
        {
            for (int j = 0; j < size.Length; j++)
            {
                // 1列・行目・最終列・行目には壁を設定
                if (i == 0 || j == 0 || i == size.Length - 1 || j == size.Length - 1)
                {
                    // 部屋の隅には壁を配置しない
                    if ((i != 0 || j != 0) && (i != size.Length - 1 || j != size.Length - 1) && (i != 0 || j != size.Length - 1) && (i != size.Length - 1 || j != 0))
                    {
                        // 床や通路が無い場合は壁を配置する
                        if (map[startPos[0] + size[i]][startPos[1] + size[j]].type != "#" && map[startPos[0] + size[i]][startPos[1] + size[j]].type != "b" && map[startPos[0] + size[i]][startPos[1] + size[j]].type != "t")
                        {
                            border.Add(new Border(startPos[0] + size[i], startPos[1] + size[j], id));
                            map[startPos[0] + size[i]][startPos[1] + size[j]].Set("-", id);
                        }
                    }

                }
                // それ以外は床を設定
                else
                {
                    map[startPos[0] + size[i]][startPos[1] + size[j]].Set("#", id);
                    tmp_room.Add(new RoomChip("#", startPos[0] + size[i], startPos[1] + size[j]));
                }
            }
        }

        return tmp_room;

    }

    /// <summary>
    /// 設定された座標を中心とした部屋を作成できるかを判定
    /// </summary>
    /// <param name="startRoomPos"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public bool CheckRoom(int[] startRoomPos, int len)
    {
        int[] size = CreateRange(len);

        for (int i = 0; i < size.Length; i++)
        {
            for (int j = 0; j < size.Length; j++)
            {
                // ステージを作成する際に使用する、サイズ[max, max]のステージの素からはみ出ていないかを判定
                if(startRoomPos[0] + size[i] > max-1 || startRoomPos[1] + size[j] > max-1 || startRoomPos[0] + size[i] < 0 || startRoomPos[1] + size[j] < 0)
                {
                    return false;
                }
                
                // 作成する部屋と既存の部屋が被っていないかを判定
                if (map[startRoomPos[0] + size[i]][startRoomPos[1] + size[j]].type == "#")
                {
                    return false;
                }
                
            }
        }
        return true;
    }

    /// <summary>
    /// 部屋を作成するための座標を検索
    /// Map上に部屋が存在しない場所を検索
    /// </summary>
    /// <param name="len"> 作成する部屋の半径サイズを指定 </param>
    /// <param name="id">  </param>
    /// <param name="init_len"></param>
    /// <returns></returns>
    public int[] SearchPos(int len, int id, int init_len, int ROOM_NUM)
    {
        // 部屋を作成する座標を計算
        while (true)
        {
            // 新しい部屋を作成する際に、既存のどの部屋の壁を起点にするかを設定
            int ind;
            if(id == 2)
            {
                // 二番目につくる部屋はランダム
                ind = rnd.Next(0, border.Count);
            }
            else
            {
                // 三番目以降に作る部屋の起点が二番目以降に作った部屋の壁になるように設定
                ind = rnd.Next((init_len * 2 - 1) * 4, border.Count);
            }
            startBorder = border[ind];


            for (int i = -1; i < 2; i += 2)
            {
                // ステージを作成する際に使用する、サイズ[max, max]のステージの素から部屋がはみ出ていないかを判定
                if(startBorder.x + i > max - 1 || startBorder.x + i < 0 || startBorder.y + i > max - 1 || startBorder.y + i < 0)
                {
                    Vert = 0;
                    Hori = 0;
                    break;
                }
                
                // 起点となる壁の左右と上下のどちらに部屋を作成するスペースがあるかを判定

                // 左右にある場合
                if (map[startBorder.x + i][startBorder.y].type == "#")
                {
                    Vert = -i;
                    Hori = 0;
                }

                // 上下にある場合
                if (map[startBorder.x][startBorder.y + i].type == "#")
                {
                    Vert = 0;
                    Hori = -i;
                }
            }

            // 部屋がはみ出ている場合はやり直し
            if (Vert == 0 && Hori == 0) continue;

            // 部屋を作成する起点となる座標を設定
            startRoomPos = new int[2] { startBorder.x + (len * Vert), startBorder.y + (len * Hori) };

            // 起点から部屋を作成できない場合はやり直し
            if (CheckRoom(startRoomPos, len)) break;
        }


        // 部屋と部屋を繋げる通路を作成
        for (int h = -1; h < 2; h++)
        {
            for (int w = -1; w < 2; w++)
            {
                if (w != 0 && h != 0) continue;
                if (map[startBorder.x + h][startBorder.y + w].type == "-")
                {
                    if(id == ROOM_NUM + 1)
                    {
                        map[startBorder.x + h][startBorder.y + w].type = "t";
                    }
                    else
                    {
                        map[startBorder.x + h][startBorder.y + w].type = "b";
                    }
                    //map[startBorder.x + h][startBorder.y + w].Set("b", id);

                }
            }
        }

        // どの部屋と部屋が繋がっているかを保存
        if(id != ROOM_NUM + 1)
        {
            GameMgr.stageIDList[map[startBorder.x][startBorder.y].id - 1].Add(id);
            GameMgr.stageIDList[id - 1].Add(map[startBorder.x][startBorder.y].id);
            //Debug.Log("preID : " + map[startBorder.x][startBorder.y].id);
            //Debug.Log("ID : " + id);
        }
        

        // 部屋の壁を作成
        for (int i = 0; i < border.Count; i++)
        {
            if ((border[i].x == startBorder.x || border[i].y == startBorder.y) && border[i].id == startBorder.id)
            {
                if (map[border[i].x][border[i].y].type != "#" && map[border[i].x][border[i].y].type != "b" && map[border[i].x][border[i].y].type != "t")
                {
                    map[border[i].x][border[i].y].Set("-", id);
                }
                border.RemoveAt(i);
                i--;
            }
        }
        
        // 生成する部屋の座標を返す
        return startRoomPos;
    }

    /// <summary>
    ///  Stageを生成する関数
    /// </summary>
    /// <param name="ROOM_NUM"> 作成するステージの部屋の数 </param>
    /// <param name="max"> ステージの最大サイズ max * max </param>
    /// <returns></returns>
    public List<List<StageChip>> Generate(int ROOM_NUM, int max)
    {
        // 乱数生成のインスタンス
        rnd = new System.Random(System.Environment.TickCount);

        // ステージの最大サイズを設定
        this.max = max;

        // ステージ上の壁のリストのインスタンス生成
        border = new List<Border>();

        // 障害物生成用のクラスのインスタンス生成
        ah = this.GetComponent<AnaHori>();

        //マップのリストのインスタンス生成
        map = new List<List<StageChip>>(max);

        // マップを空白で初期化
        for (int i = 0; i < max; i++)
        {
            map.Add(new List<StageChip>(max));
            for (int j = 0; j < max; j++)
            {
                map[i].Add(new StageChip(" ", 0));
            }
        }

        //スタート地点（真ん中）の取得
        startPos = new int[] { (max - 1) / 2, (max - 1) / 2 };
        int[] tmp = startPos;


        // 部屋の半径サイズをランダムで設定
        int len = rnd.Next(minSize, maxSize);
        
        // 初期の部屋の大きさを格納 
        int init_len = len;

        // 最初の部屋を作成
        List<RoomChip> tmp_room  = MakeRoom(startPos, 1, len);
        
        // 最初の部屋の障害物を作成
        MakeObst(len, tmp_room);
        

        // ROOMNUMの数だけ部屋と障害物を作成
        for (int i = 2; i < ROOM_NUM+2; i++)
        {
            // 最後に作成する部屋は人質のいる部屋なので
            // 障害物を作成しない
            if(i != ROOM_NUM + 1)
            {
                // 作成する部屋の半径サイズを設定
                len = rnd.Next(minSize, maxSize);

                // 部屋を作成する座標を設定
                startPos = SearchPos(len, i, init_len, ROOM_NUM);
                
                // 部屋を作成
                tmp_room = MakeRoom(startPos, i, len);
                
                // 部屋に障害物を作成
                MakeObst(len, tmp_room);
                //Debug.Log("MakeRoom");
            }
            else
            {
                // 部屋を作成する座標を設定
                startPos = SearchPos(minSize, i, init_len, ROOM_NUM);
                
                // 部屋を作成
                tmp_room = MakeRoom(startPos, i, minSize);
            }
        }

        // スタート地点を設定
        map[tmp[0]][tmp[1]].type = "S";

        // 部屋と部屋の間の通路をふさぐ障害物を除去
        int[] RemoveRange = CreateRange(3);
        for(int i =0; i < map.Count; i++)
        {
            for(int j = 0; j < map.Count; j++)
            {
                if (map[i][j].type == "b" || map[i][j].type == "t")
                {
                    for(int h = 0; h < RemoveRange.Length; h++)
                    {
                        for(int w = 0; w < RemoveRange.Length; w++)
                        {
                            if (map[i + RemoveRange[h]][j + RemoveRange[w]].type == "+")
                            {
                                map[i + RemoveRange[h]][j + RemoveRange[w]].type = "#";
                            }
                        }
                    }

                    for (int h = -1; h < 2; h++)
                    {
                        for (int w = -1; w < 2; w++)
                        {
                            if (h == -1 && w == -1) continue;
                            if (h == 1 && w == 1) continue;
                            if (h == -1 && w == 1) continue;
                            if (h == 1 && w == -1) continue;

                            if (map[i + h][j + w].type == "#" || map[i + h][j + w].type == "b")
                            {
                                map[i + h][j + w].type = "*";
                            }
                        }
                    }
                    
                } 
            }
        }

        // 人質の居る場所を設定
        map[startPos[0]][startPos[1]].type = "G";
        
        // 作成したステージをテキストファイルに書き込み
        string StageFile = Application.dataPath + "/" + "Resources" + "/" + "stage3.txt";
        ReadWrite.ListWrite(StageFile, map, max, max);


        return map;
    }

}
