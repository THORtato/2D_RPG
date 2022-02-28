using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAction : MonoBehaviour
{
    public GameObject mageUnit;
    public AllyAI Mage;

    public GameObject AttackProjectiles;
    public GameObject SkillProjectiles;
    public float delay,skillDelay;

    float enemyCount, mageMana;
    float lowEnemy, medEnemy, highEnemy;
    float lowMana, medMana, highMana;
    public float rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9,rule10;
    public float actionTaken;

    //ruleset
    void enemyLow()
    {
        if(enemyCount <= 1)
        {
            lowEnemy = 1f;
        }
        else if(enemyCount > 1 && enemyCount < 3)
        {
            lowEnemy = (3 - enemyCount) / 2;
        }
        else if(enemyCount >= 3)
        {
            lowEnemy = 0f;
        }
    }

    void enemyMed()
    {
        if(enemyCount < 1)
        {
            medEnemy = 0f;
        }
        else if(enemyCount >= 1 && enemyCount < 3)
        {
            medEnemy = (enemyCount - 2) / 2;
        }
        else if(enemyCount >= 3 && enemyCount < 5)
        {
            medEnemy = (5 - enemyCount) / 2;
        }
        else if(enemyCount >= 5)
        {
            medEnemy = 0f;
        }
    }

    void enemyHigh()
    {
        if(enemyCount < 3)
        {
            highEnemy = 0f;
        }
        else if (enemyCount >= 3 && enemyCount < 5)
        {
            highEnemy = (enemyCount - 3) / 2;
        }
        else if (enemyCount >= 5)
        {
            highEnemy = 1f;
        }
    }

    void manaLow()
    {
        if (mageMana < 25)
        {
            lowMana = 1f;
        }
        else if (mageMana >= 25 && mageMana < 50)
        {
            lowMana = (50 - mageMana) / 20;
        }
        else if (mageMana >= 50)
        {
            lowMana = 0f;
        }
    }

    void manaMed()
    {
        if (mageMana < 25)
        {
            medMana = 0f;
        }
        else if (mageMana >= 25 && mageMana < 50)
        {
            medMana = (mageMana - 25) / 25;
        }
        else if (mageMana >= 50 && mageMana < 75)
        {
            medMana = (75 - mageMana) / 25;
        }
        else if (mageMana >= 75)
        {
            medMana = 0f;
        }
    }

    void manaHigh()
    {
        if (mageMana < 50)
        {
            highMana = 0f;
        }
        else if (mageMana >= 50 && mageMana < 75)
        {
            highMana = (mageMana - 50) / 25;
        }
        else if (mageMana >= 75)
        {
            highMana = 1f;
        }
    }

    void Fuzzification()
    {
        enemyLow();
        enemyMed();
        enemyHigh();
        manaLow();
        manaMed();
        manaHigh();
    }

    float findMin(float a, float b)
    {
        return Mathf.Min(a, b);
    }

    void RuleSet()
    {
        Fuzzification();
        //if LOW ENEMY and LOW MANA then ATTACK
        rule1 = findMin(lowEnemy, lowMana);
        //if LOW ENEMY and MED MANA then ATTACK
        rule2 = findMin(lowEnemy, medMana);
        //if LOW ENEMY and HIGH MANA then ATTACK
        rule3 = findMin(lowEnemy, highMana);
        //if MED ENEMY and LOW MANA then ATTACK
        rule4 = findMin(medEnemy, lowMana);
        //if MED ENEMY and MED MANA then SKILL
        rule5 = findMin(medEnemy, medMana);
        //if MED ENEMY and HIGH MANA then SKILL
        rule6 = findMin(medEnemy, highMana);
        //if HIGH ENEMY and LOW MANA then RUN
        rule7 = findMin(highEnemy, lowMana);
        //if HIGH ENEMY and MED MANA then SKILL
        rule8 = findMin(highEnemy, medMana);
        //if HIGH ENEMY and HIGH MANA then SKILL
        rule9 = findMin(highEnemy, highMana);
        

    }

    private void Start()
    {
        Mage = mageUnit.GetComponent<AllyAI>();
    }

    private void Update()
    {
        enemyCount = Mage.targetCount;
        mageMana = Mage.UnitMana;
        skillDelay += Time.deltaTime;
        RuleSet();
        MageDecision();
    }

    public void MageDecision()
    {
        if (Vector2.Distance(transform.position, Mage.target.transform.position) < Mage.UnitAttackRange)
        {
            if (delay > 3f)
            {
                Mage.speed = Mage.defaultSpeed;
                actionTaken = Mathf.Max(rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9);
                if ( actionTaken == rule1 || actionTaken == rule2  || actionTaken == rule3 || actionTaken == rule4)
                {
                    if (Mage.currentTarget != null)
                    {
                        MageAttack();
                        Debug.Log("Attack");
                    }
                    else
                    {
                        Mage.Anim.SetBool("isAttacking", false);
                        return;
                    }
                }
                else if (actionTaken == rule7)
                {
                    Debug.Log("RUN!");
                    MageRun();
                }
                else
                {
                    if(skillDelay > 5f)
                    {
                        Mage.Anim.SetBool("isAttacking", true);
                        MageSkill();
                        skillDelay = 0;
                    }
                    
                }
                delay = 0;
                
            }
            delay += Time.deltaTime;
            
        }
    }

    public void MageAttack()
    {
        Mage.Anim.SetBool("isAttacking", true);
        GameObject bullet = GameObject.Instantiate(AttackProjectiles, Mage.firepoint.position, Mage.firepoint.rotation);
        bullet.GetComponent<AllyBullet>().bulletTarget = Mage.target;
        bullet.GetComponent<AllyBullet>().bulletDamage = Mage.UnitAttack;
    }

    public void MageSkill()
    {
        GameObject skill = GameObject.Instantiate(SkillProjectiles, Mage.firepoint.position, Mage.firepoint.rotation);
        skill.GetComponent<MageSkill>().SkillDamage = Mage.UnitAttack;
        Mage.Anim.SetBool("isAttacking", true);
        Mage.UnitMana -= 10;
        
    }

    public void MageRun()
    {
        Mage.speed *= -1;
    }
    
}
