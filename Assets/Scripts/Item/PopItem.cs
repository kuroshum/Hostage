using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopItem : MonoBehaviour
{
    private GameMgr gm;

    private GameObject item;

    private Enemy enemy;

    private bool popShotItemFlag;
    private bool popDecoyItemFlag;

    private float itemSize;
    public void SetItemSize(float itemSize) { this.itemSize = itemSize; }

    private float maxShotItemSize;
    public void SetMaxShotItemSize(float maxShotItemSize) { this.maxShotItemSize = maxShotItemSize; }

    private float maxDecoyItemSize;
    public void SetMaxDecoyItemSize(float maxDecoyItemSize) { this.maxDecoyItemSize = maxDecoyItemSize; }

    private float itemTime;
    private void SetItemTime(float itemTime) { this.itemTime = itemTime; }

    private bool followFlag;
    //public void SetFollowFlag(bool followFlag) { this.followFlag = followFlag; }

    public void Init(GameMgr gm, Enemy enemy)
    {
        this.gm = gm;

        if (enemy != null)
        {
            followFlag = true;
        }
        else
        {
            followFlag = false;
        }
        this.enemy = enemy;

        SetItemSize(0.1f);
        SetMaxShotItemSize(1.0f);
        SetMaxDecoyItemSize(0.4f);
        SetItemTime(0.0f);
        popShotItemFlag = false;
        popDecoyItemFlag = false;

    }

    private void PopShotItem()
    {
        item = gm.InstantiateShotItem(this.transform.position);
        popShotItemFlag = true;

    }

    private void PopDecoyItem()
    {
        item = gm.InstantiateDecoyItem(this.transform.position);
        popDecoyItemFlag = true;
    }

    private void Pop(float maxSize)
    {
        if (itemTime < 1f)
        {
            if (item.transform.localScale.x < maxSize)
            {
                itemSize += Time.deltaTime;
                item.transform.localScale = new Vector3(itemSize, itemSize, itemSize);
            }
            item.transform.position = new Vector3(item.transform.position.x, Mathf.Sin(itemTime * Mathf.PI) * 3, item.transform.position.z);
        }
        itemTime += Time.deltaTime;
    }

    private void FollowEnemy()
    {
        this.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y - 5f, enemy.transform.position.z);
    }

    void Update()
    {
        if(followFlag == true)
        {
            FollowEnemy();
        }

        if(popShotItemFlag == true)
        {
            Pop(maxShotItemSize);
        }
        else if(popDecoyItemFlag == true)
        {
            Pop(maxDecoyItemSize);
        }
    }

    public void DestroyItem()
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

    private IEnumerator DestroyItemBox()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10f, this.transform.position.z);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Shot" || col.tag == "Player")
        {
            DestroyItem();
        }
    }
}
