using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb2d;
    private Camera theCamera;

    public Transform gunArm;

    public Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        theCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        //transform.position += new Vector3(moveInput.x, moveInput.y,0f) * Time.deltaTime * moveSpeed;
        rb2d.velocity = moveInput * moveSpeed;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenPoint = theCamera.WorldToScreenPoint(transform.localPosition);

        Vector2 offset = new Vector2(mousePosition.x - screenPoint.x, mousePosition.y - screenPoint.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        gunArm.rotation = Quaternion.Euler(0, 0, angle);    

        if(mousePosition.x < screenPoint.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            gunArm.localScale = new Vector3(-1f, -1f, -1f);
        }
        else
        {
            transform.localScale = Vector3.one;
            gunArm.localScale = Vector3.one;
        }

        if(moveInput != Vector2.zero)
        {
            Anim.SetBool("isMoving", true);
        }
        else
        {
            Anim.SetBool("isMoving", false);
        }


    }
}
