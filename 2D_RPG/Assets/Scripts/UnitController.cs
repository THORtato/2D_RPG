using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;


    public float rangeToChase;
    private Vector3 MoveDirection;
    public GameObject Player;

    public bool engagingEnemy;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!engagingEnemy)
        {
            FollowPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        //if on range there is enemy
        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("Enemy detected");
            engagingEnemy = true;
            GameObject Enemy = other.gameObject;
            if (Vector3.Distance(transform.position, Enemy.transform.position) < rangeToChase)
            {
                MoveDirection = other.gameObject.transform.position - transform.position;
                Destroy(Enemy);
                engagingEnemy = false;
            }
            else
            {
                MoveDirection = Vector2.zero;
            }
        }

        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;
    }

    private void FollowPlayer()
    {
        Vector3 rangeBetweenAlly = new Vector3(1, 1);

        Debug.Log("Following Player");
        MoveDirection = PlayerController.instance.transform.position - transform.position - rangeBetweenAlly;
        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;
    }

    /*
    if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChase)
        {
            MoveDirection = PlayerController.instance.transform.position - transform.position;
        }
        else
        {
            MoveDirection = Vector2.zero;
        }

        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;
    */
}
