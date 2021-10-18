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
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
