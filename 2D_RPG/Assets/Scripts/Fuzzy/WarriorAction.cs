using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAction : MonoBehaviour
{
    public GameObject warriorUnit;
    public AllyAI Warrior;
    public AudioClip SlashSFX;
    AudioSource audioSource;

    public GameObject SkillProjectiles;
    public float delay;

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
        Warrior = warriorUnit.GetComponent<AllyAI>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        enemyCount = Warrior.targetCount;
        mageMana = Warrior.UnitMana;
        RuleSet();
        WarriorDecision();
    }

    public void WarriorDecision()
    {
        if (Vector2.Distance(transform.position, Warrior.target.transform.position) < Warrior.UnitAttackRange)
        {
            if (delay > 3f)
            {
                Warrior.speed = Warrior.defaultSpeed;
                actionTaken = Mathf.Max(rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9);
                if ( actionTaken == rule1 || actionTaken == rule2  || actionTaken == rule3 || actionTaken == rule4)
                {
                    if (Warrior.currentTarget != null)
                    {
                        WarriorAttack();
                        Debug.Log("Attack");
                    }
                }
                else if (actionTaken == rule7 && Warrior.speed > 0)
                {
                    Debug.Log("RUN!");
                    WarriorRun();
                }
                else
                {
                    WarriorSkill();
                        
                }
                Invoke("CancelAnimation", .2f);
                delay = 0;
                
            }
            delay += Time.deltaTime;
            
        }
    }

    public void WarriorAttack()
    {
        Warrior.Anim.SetBool("isAttacking", true);
        if(Warrior.target.tag == "Enemy")
        {
            Warrior.target.GetComponent<EnemyAI>().UnitDamage(Warrior.UnitAttack);
        }
    }

    public void WarriorSkill()
    {
        Warrior.Anim.SetBool("isAttacking", true);
        audioSource.Play();
        GameObject skill = GameObject.Instantiate(SkillProjectiles, Warrior.firepoint.position, Warrior.firepoint.rotation);
        if(Warrior.target.transform.position.x > skill.transform.position.x)
        {
            skill.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        skill.GetComponent<WarriorSkill>().SkillDamage = Warrior.UnitAttack;
        Destroy(skill, 1f);
        Warrior.UnitMana -= 10;
        
    }

    public void WarriorRun()
    {
        Warrior.speed *= -1;
    }

    public void CancelAnimation()
    {
        Warrior.Anim.SetBool("isAttacking", false);
    }


}
