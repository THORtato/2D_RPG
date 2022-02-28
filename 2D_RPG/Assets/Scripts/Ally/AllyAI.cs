using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AllyAI : MonoBehaviour
{
    public GameObject target;
    public GameObject currentTarget;
    public int targetCount;
    public GameObject Player;
    public HealthBar healthBar;
    public ManaBar manaBar;
    [SerializeField]
    GameObject DamageEffect;

    //public EnemyAction enemyAction;
    public HealerAction healerAction;
    public MageAction mageAction;

    //Stats
    [Header("Unit Stats")]
    public float speed = 200f;
    public float defaultSpeed = 200f;
    public int UnitAttack;
    public int UnitMaxHealth;
    public int UnitHealth;
    public int UnitMaxMana;
    public int UnitMana;
    public float UnitAttackRange;
    public bool isReadyToAttack = true;
    

    //Movement
    [Header("Movement Settings")]
    Path path;
    int currentWaypoint = 0;
    public Vector3 Range;
    //how close enemy need to a waypoint(?) before it moves on to the next one
    public float nextWaypointDistance = 3f;
    bool reachEndofPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    public float MovementDelay;
    public Animator Anim;

    //Detection
    [Header("Detection Settings")]
    public bool IsInCombat;
    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;
    public Transform detectorOrigin;
    public float detectorSize;
    public Vector2 detectorOriginOffset = Vector2.zero;
    public float detectionDelay = .5f;
    public LayerMask detectorLayerMask;

    //Detection gizmo
    [Header("Detection Gizmos")]
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmo = true;

    [Header("Shooting")]
    public bool shouldShoot;
    public GameObject projectiles;
    public Transform firepoint;
    public float fireRate;
    private float fireCounter;

    // Start is called before the first frame update
    void Start()
    {
        UnitHealth = UnitMaxHealth;
        UnitMana = UnitMaxMana;
        MovementDelay = .5f;
        healthBar.setHealth(UnitHealth, UnitMaxHealth);
        manaBar.setMana(UnitMana, UnitMaxMana);
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        healerAction = GetComponent<HealerAction>();
        mageAction = GetComponent<MageAction>();

        InvokeRepeating("UpdatePath", 0f, MovementDelay);
        StartCoroutine(DetectionCoroutine());

        if (IsInCombat == false)
        {
            InvokeRepeating("UnitRegen", 1f, 2f);
        }



    }


    void Update()
    {
        healthBar.setHealth(UnitHealth, UnitMaxHealth);
        manaBar.setMana(UnitMana, UnitMaxMana);

        if (target != null)
        {
            //healerAction.HealerDecision();
            mageAction.MageDecision();
        } 

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowEnemy();
    }

    //follow enemy / Pathfinding
    void FollowEnemy()
    {
        //if there is no path, do nothing
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Anim.SetBool("isMoving", false);
            reachEndofPath = true;
            return;
        }
        else
        {
            reachEndofPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        Anim.SetBool("isMoving", true);

        rb.AddForce(force);

        //flip unit
        if (target.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }




        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
            
        }

    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            //generating path
            if (target != null)
            {
                seeker.StartPath(rb.position, target.transform.position - Range, OnPathComplete);
            }
            else
            {
                return;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Detection
    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, detectorLayerMask);
        targetCount = collider.Length;

        if (currentTarget != null)
        {
            return;
        }

        //if there is something in collider
        if (collider != null && collider.Length > 0)
        {
            target = collider[0].gameObject;
            currentTarget = target;
            Debug.Log(target.name + " Detected");
            IsInCombat = true;
        }
        else
        {
            target = Player;
            currentTarget = null;
            IsInCombat = false;
        }

        
    }

    private void OnDrawGizmos()
    {
        if (showGizmo && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (IsInCombat)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawSphere((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }

    public void UnitDamage(int UnitAttack)
    {
        Instantiate(DamageEffect, transform.position, transform.rotation);
        UnitHealth -= UnitAttack;
        if (UnitHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void UnitRegen()
    {
        if(UnitMana < UnitMaxMana)
        {
            Debug.Log("REGEN...");
            UnitMana += 5;

        }
        if (UnitHealth < UnitMaxHealth)
        {
            Debug.Log("REGEN...");
            UnitHealth += 5;

        }

    }
}
