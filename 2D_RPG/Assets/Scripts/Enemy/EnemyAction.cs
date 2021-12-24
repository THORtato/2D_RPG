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
    public GameObject impactEffect;
    public Transform impactPoint;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();

    }

    public void UnitAction()
    {
        if (Vector2.Distance(transform.position, enemyAI.target.transform.position) < enemyAI.UnitAttackRange )
        {
            if(delay > 2f)
            {
                switch (unitType)
                {
                    case UnitType.Melee:
                        enemyAI.MovementDelay = .5f;
                        enemyAI.Anim.SetBool("isAttacking", true);
                        if (enemyAI.target.tag == "Player")
                        {

                            enemyAI.target.GetComponent<PlayerController>().playerHealth -= enemyAI.UnitDamage;
                            Debug.Log("Attacking" + enemyAI.target.name);
                            
                        }
                        else if (enemyAI.target.tag == "Companion")
                        {
                            enemyAI.target.GetComponent<AllyAI>().UnitHealth -= enemyAI.UnitDamage;
                            Debug.Log("Attacking" + enemyAI.target.name);
                           
                        }
                        delay = 0;
                        Instantiate(impactEffect, impactPoint.position, impactPoint.rotation);
                        break;

                    case UnitType.Ranged:
                        enemyAI.MovementDelay = 3f;
                        enemyAI.shouldShoot = true;
                        if (enemyAI.shouldShoot)
                        {
                            GameObject bullet = GameObject.Instantiate(enemyAI.projectiles, enemyAI.firepoint.position, enemyAI.firepoint.rotation);
                            enemyAI.Range = new Vector2((Random.RandomRange(0, 2) * 2 - 1) * 3, (Random.RandomRange(0, 2) * 2 - 1) * 3);
                            bullet.GetComponent<EnemyBullet>().bulletTarget = enemyAI.target;
                            Debug.Log("Bullet Instantiated");
                            delay = 0;
                        }
                        break;

                }
                
            }
            else if(delay < 2f)
            {
                
                enemyAI.shouldShoot = false;
                enemyAI.Anim.SetBool("isAttacking", false);
            }
            
            delay += Time.deltaTime;
        }

    }
}
