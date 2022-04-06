using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> enemy = new List<GameObject>();
    public int enemyCount;
    public Collider2D nextstage;


    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < enemyCount; i++)
        {
            if (enemy[i] == null)
            {
                enemy.RemoveAt(i);
                enemyCount--;
                i--;
            }
        }
        enableNextStage();
    }

    public void enableNextStage()
    {
        if (enemyCount == 0)
        {
            nextstage.isTrigger = true;
        }
    }
}
