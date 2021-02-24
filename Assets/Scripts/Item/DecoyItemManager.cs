using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyItemManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(this.transform.position, Vector3.up, Time.deltaTime*90);
    }
}
