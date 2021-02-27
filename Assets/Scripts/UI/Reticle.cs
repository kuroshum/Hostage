using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class Reticle : MonoBehaviour
{

    private Texture2D reticle;

    private Texture2D reticleForcuse;

    private LineRenderer orbit;
    private Material orbitMaterial;

    private Color normalColor = new Color(0f, 175f, 255f);
    private Color forcuseColor = new Color(255f, 0f, 0f);

    void LoadTexture(Texture2D texture, string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        texture.LoadImage(bytes);
    }

    // Start is called before the first frame update
    void Start()
    {
        orbit = this.GetComponent<LineRenderer>();

        orbitMaterial = orbit.material;
        orbitMaterial.SetColor("_Color", normalColor);


        Vector3[] orbitPos = new Vector3[]
        {
            this.transform.position,
            new Vector3(this.transform.position.x+5, this.transform.position.y,this.transform.position.z+5)
        };

        orbit.SetPositions(orbitPos);

        Color[] colors = new Color[]
        {
            new Color(0,0,1),
            new Color(0,0,1)
        };

        orbit.startWidth = 0.1f;
        orbit.endWidth = 0.1f;

        reticle = new Texture2D(2, 2);
        LoadTexture(reticle, "Assets/Resources/reticle.png");
        reticleForcuse = new Texture2D(2, 2);
        LoadTexture(reticleForcuse, "Assets/Resources/reticle_forcus.png");

    }

    private void ChangeReticle(Vector3 mousePos, Ray ray)
    {
        // メインカメラからクリックした地点のベクトルでRayを飛ばす
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 
        RaycastHit hit;

        //レイを投げて何かのオブジェクトに当たった場合
        if (Physics.Raycast(ray, out hit, Vector3.Distance(mousePos, this.transform.position), ~(1 << 14)))
        {
            // 衝突したオブジェクトがステージかウェイポイントの場合はデコイ配置
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "ItemBox")
            {
                OnPointerEnter(reticleForcuse, forcuseColor);
            }
            else
            {
                OnPointerEnter(reticle, normalColor);
            }
        }
        else
        {
            OnPointerEnter(reticle, normalColor);
        }
    }

    private void DrawOrbit(Vector3 mousePos, Ray ray)
    {
        RaycastHit hit;

        Vector3 targetPos = mousePos;
        if (Physics.Raycast(ray, out hit, Vector3.Distance(mousePos, this.transform.position), ~(1 << 14)))
        {
            if (hit.collider.tag == "Wall")
            {
                targetPos = hit.point;
                //Debug.Log("wall");
            }
        }

        Vector3[] orbitPos = new Vector3[]
        {
            this.transform.position,
            targetPos
        };

        Debug.DrawRay(ray.origin, (mousePos - this.transform.position), Color.red, 0.1f);
        orbit.SetPositions(orbitPos);
    }

    // Update is called once per frame
    void Update()
    {
        

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 15.0f;
        mousePos.x += 5f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos = new Vector3(mousePos.x, this.transform.position.y, mousePos.z);
        mousePos = mousePos + (mousePos - this.transform.position).normalized;

        Ray ray = new Ray(this.transform.position, (mousePos - this.transform.position).normalized);

        // カーソルが敵の上かそれ以外でレティクルを変更
        ChangeReticle(mousePos, ray);

        if (SceneManager.GetActiveScene().name != "Main" && SceneManager.GetActiveScene().name != "Select")
        {
            return;
        }
        // 
        DrawOrbit(mousePos, ray);

    }

    public void OnPointerEnter(Texture2D reticle, Color color)
    {
        Cursor.SetCursor(reticle, Vector2.zero, CursorMode.Auto);
        orbitMaterial.SetColor("_Color", color);
    }

}
