using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    public GameObject bulletTarget;
 
    // Start is called before the first frame update
    void Start()
    {
        if(bulletTarget == null)
        {
            print("Target Is NULL");
        } else
        {
            Debug.Log(bulletTarget);
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

    private void OnCollisionEnter2D(Collision2D target)
    {
        if(target.gameObject.tag == "Player")
        {
            print("Player Hit");
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
