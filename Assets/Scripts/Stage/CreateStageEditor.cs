using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CS))]//拡張するクラスを指定
public class CreateStageEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Vector3 pos = Vector3.zero;
        //元のInspector部分を表示
        base.OnInspectorGUI();

        //targetを変換して対象を取得
        CS createStage = target as CS;

        //publicMethodを実行する用のボタン
        if (GUILayout.Button("CreateStage"))
        {
            createStage.Create(pos);
        }

        if (GUILayout.Button("DeleteStage"))
        {
            createStage.Delete();
        }
    }
}


