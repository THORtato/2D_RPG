﻿using System.Collections;
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
                            enemyAI.target.GetComponent<PlayerController>().PlayerDamage(enemyAI.UnitAttack);
                            Debug.Log("Attacking" + enemyAI.target.name);
                        }
                        else if (enemyAI.target.tag == "Companion")
                        {
                            enemyAI.target.GetComponent<AllyAI>().UnitDamage(enemyAI.UnitAttack);
                            Debug.Log("Attacking" + enemyAI.target.name);
                           
                        }
                        delay = 0;
                        break;

                    case UnitType.Ranged:
                        enemyAI.MovementDelay = 3f;
                        enemyAI.Range = new Vector2((Random.RandomRange(0, 2) * 2 - 1) * 3, (Random.RandomRange(0, 2) * 2 - 1) * 3);
                        enemyAI.shouldShoot = true;
                        if (enemyAI.shouldShoot)
                        {
                            GameObject bullet = GameObject.Instantiate(enemyAI.projectiles, enemyAI.firepoint.position, enemyAI.firepoint.rotation);
                            bullet.GetComponent<EnemyBullet>().bulletTarget = enemyAI.target;
                            bullet.GetComponent<EnemyBullet>().bulletPower = enemyAI.UnitAttack;
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
