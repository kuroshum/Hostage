using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneMgr : MonoBehaviour
{
    private Player player;

    private GameObject camera;

    private Vector3 startPos;

    [SerializeField]
    private float playerMoveSpeed = 3.0f;

    [SerializeField]
    private float playerApplySpeed = 0.2f;

    private GameMgr gm;

    // Start is called before the first frame update
    void Start()
    {
        //gm = this.GetComponent<GameMgr>();

        camera = GameObject.FindGameObjectWithTag("MainCamera");

        Player.parent = new TokenMgr<Player>("Player", 1);

        player = Player.Add(0, playerMoveSpeed, playerApplySpeed, null, startPos.x, startPos.y + 1, startPos.z, camera);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
