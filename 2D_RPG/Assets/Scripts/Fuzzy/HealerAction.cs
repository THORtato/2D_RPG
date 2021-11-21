using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAction : MonoBehaviour
{
    float playerHealth;
    float lowHealth, medHealth, highHealth;
    float rule1, rule2, rule3, rule4, rule5, rule6;

    //ruleset
    void healLow()
    {
        if(playerHealth < 30)
        {
            lowHealth = 1f;
        }
        else if(playerHealth >= 30 && playerHealth < 50)
        {
            lowHealth = (50 - playerHealth) / 20;
        }
        else if(playerHealth > 50)
        {
            lowHealth = 0f;
        }
    }

    void healMed()
    {
        if(playerHealth < 30)
        {
            medHealth = 0f;
        }
        else if(playerHealth >= 30 && playerHealth < 50)
        {
            medHealth = (playerHealth - 30) / 20;
        }
        else if(playerHealth >= 50 && playerHealth < 70)
        {
            medHealth = (70 - playerHealth) / 20;
        }
        else if(playerHealth >= 70)
        {
            medHealth = 0f;
        }
    }

    void healHigh()
    {
        if(playerHealth < 90)
        {
            highHealth = 0f;
        }
        else if (playerHealth >= 90 && playerHealth < 100)
        {
            highHealth = (playerHealth - 90) / 10;
        }
        else if (playerHealth >= 100)
        {
            highHealth = 1f;
        }
    }

    void Fuzzification()
    {
        healLow();
        healMed();
        healHigh();
    }

    float findMin(float a, float b)
    {
        return Mathf.Min(a, b);
    }

    void RuleSet()
    {
        rule1 = findMin(lowHealth, medHealth);
    }


    private void Start()
    {
        playerHealth = GetComponent<PlayerController>().playerHealth;
        Fuzzification();

        
    }

    private void Update()
    {

    }

}
