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
        Debug.Log(bulletTarget);
        direction = bulletTarget.transform.position - transform.position;

        //direction = PlayerController.instance.transform.position - transform.position;
        direction.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(bulletTarget);
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if(target.tag == "Player")
        {

            Destroy(gameObject);
        }
        
        
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
