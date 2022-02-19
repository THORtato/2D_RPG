using System.Collections;
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
    public GameObject damageEffect;

    public Transform gunArm;

    public Animator Anim;

    public int playerHealth;
    public int playerMana;

    [Header("Attacking")]
    public GameObject Bullet;
    public Transform firePoint;
    [SerializeField]
    private float _timeBetweenAttack;
    public int playerDamage;
    [SerializeField]
    private float _attackSpeed;
    [SerializeField]
    private float _attackRange;
    public LayerMask enemyLayerMask;


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
        PlayerAttack();    

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
            transform.localScale = new Vector3(1f, 1f, 1f);
            gunArm.localScale = new Vector3(1f, 1f, -1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            gunArm.localScale = new Vector3(1f, 1f, 1f);
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
    void PlayerAttack()
    {
        if (Input.GetMouseButton(0) && playerMana != 0)
        {
            
            _timeBetweenAttack -= Time.deltaTime;
            if (_timeBetweenAttack <= 0)
            {
                Anim.SetBool("isShooting", true);
                Instantiate(Bullet, firePoint.position, firePoint.rotation);
                playerMana -= 1;
                _timeBetweenAttack = _attackSpeed;
            }
            else
            {
                Anim.SetBool("isShooting", false);
            }
        }

        if (Input.GetKey(KeyCode.F))
        {
            
            Debug.Log("Player ATTACK!");
            _timeBetweenAttack -= Time.deltaTime;
            if (_timeBetweenAttack <= 0)
            {
                Anim.SetBool("isMelee", true);
                Collider2D[] enemies = Physics2D.OverlapCircleAll(firePoint.position, _attackRange, enemyLayerMask);
                for(int i =0; i < enemies.Length; i++)
                {
                   
                    enemies[i].GetComponent<EnemyAI>().UnitDamage(playerDamage);
                }
                _timeBetweenAttack = _attackSpeed;
            }
            else
            {
                Anim.SetBool("isMelee", false);
            }
            
        }
    }

    public void PlayerDamage(int unitAttack)
    {
        Instantiate(damageEffect, transform.position, transform.rotation);
        playerHealth -= unitAttack;
        if (playerHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
