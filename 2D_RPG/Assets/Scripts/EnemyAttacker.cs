using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    
    //[Header("Runtime")]

    [Header("Setting")]
    private bool isAttacking = false;
    public float rangeToAttack;
    public int enemyHealth = 100;
    [Space(10)]
    [Header("Reference")]
    private Rigidbody2D rb2d;
    public float speed;
    private Vector3 MoveDirection;
    public GameObject Player;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAttack();
    }

    private void EnemyAttack()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= rangeToAttack)
        {
            Vector2 AttackDistance = transform.position;
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

    }

    /*
    public void EnemyMovementAnimation()
    {
        if (isMoving)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }*/

    public void DamageEnemy(int damage)
    {
        enemyHealth -= damage;
        if(enemyHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    //enemyattack
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isAttacking = true && collision.gameObject.tag == "Player")
        {
            animator.SetBool("isAttacking", true);
            collision.GetComponent<PlayerController>().DamagePlayer(10);
            
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.SetBool("isAttacking", false);
    }
}
