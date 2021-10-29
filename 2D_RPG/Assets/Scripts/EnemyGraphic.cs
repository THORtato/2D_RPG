using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyGraphic : MonoBehaviour
{
    public AIPath aipath;

    // Update is called once per frame
    void Update()
    {
        //flip enemy's graphic
        if(aipath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            Debug.Log("Turned Right");
        } else if(aipath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Debug.Log("Turned Left");
        }
    }
}
