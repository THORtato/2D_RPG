using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAction : MonoBehaviour
{
    public GameObject healerUnit;
    public GameObject playerUnit;
    public PlayerController Player;
    public AllyAI Healer;
    public AudioClip SkillSFX;
    AudioSource audioSource;

    public GameObject AttackProjectiles;
    public GameObject BuffProjectiles;
    public float delay;

    float playerHealth, healerMana;
    float lowHealth, medHealth, highHealth,fullHealth;
    float lowMana, medMana, highMana;
    public float rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9,rule10;

    public float actionTaken;

    //ruleset
    void healthLow()
    {
        if(playerHealth < 25)
        {
            lowHealth = 1f;
        }
        else if(playerHealth >= 25 && playerHealth < 50)
        {
            lowHealth = (50 - playerHealth) / 20;
        }
        else if(playerHealth >= 50)
        {
            lowHealth = 0f;
        }
    }

    void healthMed()
    {
        if(playerHealth < 25)
        {
            medHealth = 0f;
        }
        else if(playerHealth >= 25 && playerHealth < 50)
        {
            medHealth = (playerHealth - 25) / 25;
        }
        else if(playerHealth >= 50 && playerHealth < 75)
        {
            medHealth = (75 - playerHealth) / 25;
        }
        else if(playerHealth >= 75)
        {
            medHealth = 0f;
        }
    }

    void healthHigh()
    {
        if(playerHealth < 50)
        {
            highHealth = 0f;
        }
        else if (playerHealth >= 50 && playerHealth < 75)
        {
            highHealth = (playerHealth - 50) / 25;
        }
        else if (playerHealth >= 75)
        {
            highHealth = 1f;
        }
    }

    void healthFull()
    {
        if(playerHealth < 75)
        {
            fullHealth = 0f;
        } 
        else if ( playerHealth >= 75 && playerHealth < 100 )
        {
            fullHealth = (playerHealth - 75) / 25;
        } 
        else if(playerHealth>= 95)
        {
            fullHealth = 1f;
        }
    }

    void manaLow()
    {
        if (healerMana < 25)
        {
            lowMana = 1f;
        }
        else if (healerMana >= 25 && healerMana < 50)
        {
            lowMana = (50 - healerMana) / 20;
        }
        else if (healerMana >= 50)
        {
            lowMana = 0f;
        }
    }

    void manaMed()
    {
        if (healerMana < 25)
        {
            medMana = 0f;
        }
        else if (healerMana >= 25 && healerMana < 50)
        {
            medMana = (healerMana - 25) / 25;
        }
        else if (healerMana >= 50 && healerMana < 75)
        {
            medMana = (75 - healerMana) / 25;
        }
        else if (healerMana >= 75)
        {
            medMana = 0f;
        }
    }

    void manaHigh()
    {
        if (healerMana < 50)
        {
            highMana = 0f;
        }
        else if (healerMana >= 50 && healerMana < 75)
        {
            highMana = (healerMana - 50) / 25;
        }
        else if (healerMana >= 75)
        {
            highMana = 1f;
        }
    }

    void Fuzzification()
    {
        healthLow();
        healthMed();
        healthHigh();
        healthFull();
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
        //if LOW HEALTH and LOW MANA then RUN
        rule1 = findMin(lowHealth, lowMana);
        //if LOW HEALTH and MED MANA then HEAL
        rule2 = findMin(lowHealth, medMana);
        //if LOW HEALTH and HIGH MANA then HEAL
        rule3 = findMin(lowHealth, highMana);
        //if MED HEALTH and LOW MANA then ATTACK
        rule4 = findMin(medHealth, lowMana);
        //if MED HEALTH and MED MANA then HEAL
        rule5 = findMin(medHealth, medMana);
        //if MED HEALTH and HIGH MANA then HEAL
        rule6 = findMin(medHealth, highMana);
        //if HIGH HEALTH and LOW MANA then ATTACK
        rule7 = findMin(highHealth, lowMana);
        //if HIGH HEALTH and MED MANA then HEAL
        rule8 = findMin(highHealth, medMana);
        //if HIGH HEALTH and HIGH MANA then HEAL
        rule9 = findMin(highHealth, highMana);
        //if FULL HEALTH, then ATTACK
        rule10 = findMin(fullHealth, 2);
        

    }

    private void Start()
    {
        Player = playerUnit.GetComponent<PlayerController>();
        Healer = healerUnit.GetComponent<AllyAI>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        playerHealth = Player.playerHealth;
        healerMana = Healer.UnitMana;
        RuleSet();
        HealerDecision();
    }

    public void HealerDecision()
    {
        
        if(Healer.target == null)
        {
            return;
        }

        //if enemy is in ally distance
        if (Vector2.Distance(transform.position, Healer.target.transform.position) < Healer.UnitAttackRange)
        {
            if (delay > 3f)
            {
                Healer.Anim.SetBool("isAttacking", false);
                Healer.speed = Healer.defaultSpeed;
                actionTaken = Mathf.Max(rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8, rule9);
                if ( actionTaken == rule4 || actionTaken == rule7  || actionTaken == rule10)
                {
                    if (Healer.currentTarget != null)
                    {
                        HealerAttack();
                        Debug.Log("Attack");
                    }
                    
                }
                else if (actionTaken == rule1 && Healer.speed > 0)
                {
                    Debug.Log("RUN!");
                    HealerRun();
                }
                else
                {
                    HealerHeal();
                    /*
                    if(Healer.currentTarget == null)
                    {
                        HealerHeal();
                        Debug.Log("Heal");
                    }
                    else if(Healer.target.tag == "Enemy")
                    {
                        HealerAttack();
                    }
                    */

                    
                    
                }
                Healer.currentTarget = null;
                Invoke("CancelAnimation", .2f);
                delay = 0;
            }
            delay += Time.deltaTime;
        }
        
    }

    public void HealerAttack()
    {
        Healer.Anim.SetBool("isAttacking", true);
        GameObject bullet = GameObject.Instantiate(AttackProjectiles, Healer.firepoint.position, Healer.firepoint.rotation);
        bullet.GetComponent<AllyBullet>().bulletTarget = Healer.target;
        bullet.GetComponent<AllyBullet>().bulletDamage = Healer.UnitAttack;
        
    }

    public void HealerHeal()
    {
        GameObject buff = GameObject.Instantiate(BuffProjectiles, Healer.firepoint.position, Healer.firepoint.rotation);
        audioSource.Play();
        Healer.Anim.SetBool("isAttacking", true);
        buff.GetComponent<AllyBuff>().buffTarget = playerUnit;
        buff.GetComponent<AllyBuff>().buffPower = Healer.UnitAttack;
        Healer.UnitMana -= 5;
    }

    public void HealerRun()
    {
        Healer.speed *= -1;
    }

    public void CancelAnimation()
    {
        Healer.Anim.SetBool("isAttacking", false);
    }
    
}
