﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    /// <summary>
    /// ID :
    ///     0 : shotItem
    ///     1 : DecoyItem
    /// </summary>
    private int itemID;

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

        itemID = Random.Range(0, 2);
        if (itemID == 0)
        {
            this.transform.Find("Shot").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("Decoy").gameObject.SetActive(true);
        }

    }

    private void PopShotItem()
    {
        if (SceneManager.GetActiveScene().name != "Select")
        {
            item = gm.InstantiateShotItem(this.transform.position);
        }
        popShotItemFlag = true;

    }

    private void PopDecoyItem()
    {
        if (SceneManager.GetActiveScene().name != "Select")
        {
            item = gm.InstantiateDecoyItem(this.transform.position);
        }
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

    private IEnumerator LoadScene()
    {
        if(this.name == "EasyBox")
        {
            GameMgr.ROOM_NUM = 4;
            GameMgr.ENEMY_NUM = 4;
        }
        else if(this.name == "NormalBox")
        {
            GameMgr.ROOM_NUM = 6;
            GameMgr.ENEMY_NUM = 8;
        }
        else if (this.name == "HardBox")
        {
            GameMgr.ROOM_NUM = 8;
            GameMgr.ENEMY_NUM = 10;
        }

        
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
    }

    void Update()
    {
        if (followFlag == true)
        {
            FollowEnemy();
        }

        if (SceneManager.GetActiveScene().name != "Select")
        {
            if (popShotItemFlag == true)
            {
                Pop(maxShotItemSize);
            }
            else if (popDecoyItemFlag == true)
            {
                Pop(maxDecoyItemSize);
            }
        }
        else
        {
            if (popShotItemFlag == true || popDecoyItemFlag == true)
            {
                StartCoroutine(LoadScene());
            }
        }
        

        

    }

    public void DestroyItem()
    {
        if (itemID == 0)
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
        yield return new WaitForSeconds(2f);
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
