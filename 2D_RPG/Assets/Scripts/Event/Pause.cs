using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject actor,canvas;
    // Start is called before the first frame update

    private void Start()
    {
        PauseProtag();
    }

    private void Update()
    {
        Invoke("Resume", 6f);  
    }

    void PauseProtag()
    {
        actor.GetComponent<PlayerController>().enabled = false;
        canvas.SetActive(false);

    }

    void Resume()
    {
        actor.GetComponent<PlayerController>().enabled = true;
        canvas.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Companion")
        {

        }
    }

}
