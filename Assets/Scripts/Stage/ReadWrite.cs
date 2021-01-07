using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ReadWrite : MonoBehaviour {

    /*
     * ファイルの読み取り
     */ 
    public string  FileRead(string dataPath) {

        string readStream = "";
        try {
            //Debug.Log(dataPath);
            using (StreamReader sr = new StreamReader(dataPath)) {
                readStream = sr.ReadToEnd();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
        //Debug.Log(readStream);
        return readStream;
    }
    /*
     * 読み取りの実行
     */ 
    public string Read(string dataPath) {
        //Debug.Log(dataPath);
        string readData = FileRead(dataPath);
        return readData;
    }

    public static void FileWrite(string dataPath, string[,] writeData,int iheight,int iwidth) {

        //Debug.Log("セーブします");
        try {
            using (StreamWriter sw = new StreamWriter(dataPath,false)) {
                for(int i = 0; i < iwidth; i++) {
                    for(int j = 0; j < iheight; j++) {
                        sw.Write(writeData[i,j]);
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
    }

    public static void Write(string dataPath, string[,] writeData,int iheight,int iwidth) {
        FileWrite(dataPath, writeData,iheight,iwidth);
    }

    public static void ListFileWrite(string dataPath, List<List<StageChip>> writeData,int iheight,int iwidth) {

        //Debug.Log("セーブします");
        try {
            using (StreamWriter sw = new StreamWriter(dataPath,false)) {
                for(int i = 0; i < iwidth; i++) {
                    for(int j = 0; j < iheight; j++) {
                        //sw.Write(writeData[i][j].type);
                        sw.Write(writeData[i][j].id);
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
    }

    public static void ListWrite(string dataPath, List<List<StageChip>> writeData,int iheight,int iwidth) {
        ListFileWrite(dataPath, writeData,iheight,iwidth);
    }

    public static void ListFileWrite(string dataPath, List<List<Map>> writeData, int iheight, int iwidth)
    {

        //Debug.Log("セーブします");
        try
        {
            using (StreamWriter sw = new StreamWriter(dataPath, false))
            {
                for (int i = 0; i < iwidth; i++)
                {
                    for (int j = 0; j < iheight; j++)
                    {
                        sw.Write(writeData[i][j].mapChip);
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public static void ListWrite(string dataPath, List<List<Map>> writeData, int iheight, int iwidth)
    {
        ListFileWrite(dataPath, writeData, iheight, iwidth);
    }

}