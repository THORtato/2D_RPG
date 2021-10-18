using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;

    public float rangeToChase;
    private Vector3 MoveDirection;
    public GameObject Player;
    private bool isMoving = false;

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
        EnemyMovement();
        EnemyMovementAnimation();
    }

    private void EnemyMovement()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChase)
        {
            isMoving = true;
            MoveDirection = PlayerController.instance.transform.position - transform.position;
        }
        else
        {
            isMoving = false;
            MoveDirection = Vector2.zero;
        }

        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;
    }

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
    }
}
