using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkill : MonoBehaviour
{
    public GameObject SkillSprite;
    public Animator Anim;
    public int skillDamage;


    private void OnTriggerEnter2D(Collider2D Enemy)
    {
        if (Enemy.tag == "Enemy")
        {
            Enemy.GetComponent<EnemyAI>().UnitDamage(skillDamage);
            Anim.SetBool("hitEnemy", true);
            Destroy(gameObject, .5f);

        }
    }


}
