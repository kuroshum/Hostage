using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Reticle : MonoBehaviour
{

    [SerializeField]
    private Texture2D reticle;

    [SerializeField]
    private Texture2D reticleForcuse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void RayReticle()
    {
        // メインカメラからクリックした地点のベクトルでRayを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 
        RaycastHit hit = new RaycastHit();

        //レイを投げて何かのオブジェクトに当たった場合
        if (Physics.Raycast(ray, out hit))
        {
            // 衝突したオブジェクトがステージかウェイポイントの場合はデコイ配置
            if (hit.collider.tag == "EnemyReticle")
            {
                OnPointerEnter(reticleForcuse);
            }
            else
            {
                OnPointerEnter(reticle);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        RayReticle();
    }

    public void OnPointerEnter(Texture2D reticle)
    {
        Cursor.SetCursor(reticle, Vector2.zero, CursorMode.Auto);
    }

}
