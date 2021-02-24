using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopItem : MonoBehaviour
{
    private GameMgr gm;

    private GameObject item;

    private bool popFlag;

    private float itemSize;
    public void SetItemSize(float itemSize) { this.itemSize = itemSize; }

    public void Init(GameMgr gm)
    {
        this.gm = gm;
        SetItemSize(0.1f);
        popFlag = false;
    }

    private void PopShotItem()
    {
        item = gm.InstantiateShotItem(this.transform.position);
        popFlag = true;

    }

    private void PopDecoyItem()
    {
        item = gm.InstantiateDecoyItem(this.transform.position);
        popFlag = true;
    }

    void Update()
    {
        Debug.Log(popFlag);
        if(popFlag == true)
        {
            if(item.transform.localScale.x < 1f)
            {
                itemSize += Time.deltaTime;
                item.transform.localScale = new Vector3(itemSize, itemSize, itemSize);
                item.transform.position = new Vector3(item.transform.position.x, Mathf.Sin(itemSize*Mathf.PI)*3, item.transform.position.z);
            }
            
        }
    }

    private IEnumerator DestroyItemBox()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10f, this.transform.position.z);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Shot" || col.tag == "EnemyShot" || col.tag == "Player")
        {
            if (Random.Range(0, 2) == 0)
            {
                PopShotItem();
            }
            else
            {
                PopDecoyItem();
            }
            StartCoroutine(DestroyItemBox());
        }
    }
}
