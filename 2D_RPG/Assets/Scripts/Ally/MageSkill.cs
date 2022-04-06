using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill : MonoBehaviour
{
    public GameObject SkillSprite;
    public int SkillDamage;
    public Animator Anim;
    public float defaultSpeed;
    // Start is called before the first frame update

    void Start()
    {
        Invoke("DestroySelf", 5f);
        Destroy(gameObject, 5.5f);
    }

    private void OnTriggerEnter2D(Collider2D Enemy)
    {
        if (Enemy.tag == "Enemy")
        {
            defaultSpeed = Enemy.GetComponent<EnemyAI>().speed;
            Enemy.GetComponent<EnemyAI>().speed = .5f;
            Enemy.GetComponent<EnemyAI>().patrolSpeed *= .5f;
            Enemy.GetComponent<EnemyAI>().UnitDamage(SkillDamage);
        }
    }


    private void OnTriggerExit2D(Collider2D Enemy)
    {
        if (Enemy.tag == "Enemy")
        {
            Enemy.GetComponent<EnemyAI>().speed = defaultSpeed;
            Enemy.GetComponent<EnemyAI>().patrolSpeed *= 2;
        }
    }

    public void DestroySelf()
    {
        Anim.SetBool("durationFinished", true);
    }
}
