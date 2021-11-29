using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : MonoBehaviour
{
    public enum UnitType
    {
        Melee,
        Ranged
    }

    EnemyAI enemyAI;
    public float delay;
    public UnitType unitType;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    public void UnitAction()
    {
        if (Vector2.Distance(transform.position, enemyAI.target.transform.position) < enemyAI.UnitAttackRange && delay > 2f)
        {
                switch (unitType)
                {
                    case UnitType.Melee:
                        if (enemyAI.target.tag == "Player")
                        {
                            enemyAI.target.GetComponent<PlayerController>().playerHealth -= enemyAI.UnitDamage;
                            Debug.Log("Attacking" + enemyAI.target.name);
                            delay = 0;
                        }
                        else if (enemyAI.target.tag == "Companion")
                        {
                            enemyAI.target.GetComponent<AllyAI>().UnitHealth -= enemyAI.UnitDamage;
                            Debug.Log("Attacking" + enemyAI.target.name);
                            delay = 0;
                        }
                        break;

                    case UnitType.Ranged:
                        if (enemyAI.shouldShoot)
                        {
                            GameObject bullet = GameObject.Instantiate(enemyAI.projectiles, enemyAI.firepoint.position, enemyAI.firepoint.rotation);
                            bullet.GetComponent<EnemyBullet>().bulletTarget = enemyAI.target;
                            Debug.Log("Bullet Instantiated");
                            delay = 0;
                        }


                        break;

                }
            delay += Time.deltaTime;
        }

    }
}
