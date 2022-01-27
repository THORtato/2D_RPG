using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBuff : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    public GameObject buffTarget;
    public int buffPower;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (buffTarget == null)
        {
            print("Target Is NULL");
        }
        else
        {
            Debug.Log(buffTarget);
            direction = buffTarget.transform.position - transform.position;
        }
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.tag == "Player")
        {
            target.gameObject.GetComponent<PlayerController>().playerHealth += buffPower;
            Destroy(gameObject);
        }

        

    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
