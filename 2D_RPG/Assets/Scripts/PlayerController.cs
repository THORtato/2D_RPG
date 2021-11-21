﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement")]
    [SerializeField]
    float moveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb2d;
    private Camera theCamera;

    public Transform gunArm;

    public Animator Anim;

    public int playerHealth;
    public int playerMana;

    [Header("Attacking")]
    public GameObject Bullet;
    public Transform firePoint;
    private float shotCounter;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        theCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerRotate();
        if(playerMana > 0)
        {
            PlayerShooting();
        }
        PlayerDeath();        

    }

    void PlayerRotate()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenPoint = theCamera.WorldToScreenPoint(transform.localPosition);

        Vector2 offset = new Vector2(mousePosition.x - screenPoint.x, mousePosition.y - screenPoint.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        gunArm.rotation = Quaternion.Euler(0, 0, angle);

        if (mousePosition.x < screenPoint.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            gunArm.localScale = new Vector3(-1f, -1f, -1f);
        }
        else
        {
            transform.localScale = Vector3.one;
            gunArm.localScale = Vector3.one;
        }
    }

    void PlayerMove()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        //transform.position += new Vector3(moveInput.x, moveInput.y,0f) * Time.deltaTime * moveSpeed;
        rb2d.velocity = moveInput * moveSpeed;

        if (moveInput != Vector2.zero)
        {
            Anim.SetBool("isMoving", true);
        }
        else
        {
            Anim.SetBool("isMoving", false);
        }
    }

    //shooting
    void PlayerShooting()
    {
        if (Input.GetMouseButton(0))
        {
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                Instantiate(Bullet, firePoint.position, firePoint.rotation);
                playerMana -= 1;
                shotCounter = _fireRate;
            }
        } 
    }

    public void PlayerDeath()
    {
        if (playerHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
