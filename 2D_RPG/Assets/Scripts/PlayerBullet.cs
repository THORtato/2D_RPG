using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 7.5f;
    private Rigidbody2D rb2d;
    public GameObject impactEffect;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            Instantiate(impactEffect,transform.position,transform.rotation);
            Destroy(this.gameObject);
        }
        
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
