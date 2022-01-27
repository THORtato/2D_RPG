using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    public GameObject bulletTarget;
    public int bulletPower;

    // Start is called before the first frame update
    void Start()
    {
        if (bulletTarget == null)
        {
            print("Target Is NULL");
        }
        else
        {
            direction = bulletTarget.transform.position - transform.position;
        }


        //direction = PlayerController.instance.transform.position - transform.position;
        direction.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.tag == "Player")
        {
            PlayerController Player = target.GetComponent<PlayerController>();
            Player.PlayerDamage(bulletPower);
            Destroy(gameObject);
        }



    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
