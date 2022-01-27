using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AllyAI : MonoBehaviour
{
    public GameObject target;
    public GameObject currentTarget;
    public GameObject Player;
    public HealthBar healthBar;
    [SerializeField]
    GameObject DamageEffect;

    //public EnemyAction enemyAction;
    public HealerAction healerAction;

    //Stats
    [Header("Unit Stats")]
    public float speed = 200f;
    public int UnitAttack;
    [SerializeField]
    private int UnitMaxHealth;
    public int UnitHealth;
    private int UnitMaxMana;
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
    public bool PlayerDetected;
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
        MovementDelay = .5f;
        UnitMana = UnitMaxMana;
        healthBar.setHealth(UnitHealth, UnitMaxHealth);
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        healerAction = GetComponent<HealerAction>();

        InvokeRepeating("UpdatePath", 0f, MovementDelay);
        StartCoroutine(DetectionCoroutine());
    }


    void Update()
    {
        healthBar.setHealth(UnitHealth, UnitMaxHealth);
        if (target != null)
        {
            //print("Do Nuthin");
            healerAction.HealerDecision();
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
            reachEndofPath = true;
            return;
        }
        else
        {
            reachEndofPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        /*
        //animation
        if(rb.velocity != Vector2.zero)
        {
            Anim.SetBool("isMoving", true);
        }
        else
        {
            Anim.SetBool("isMoving", false);
        }
        */

        if (target.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
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
        Collider2D collider = Physics2D.OverlapCircle((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, detectorLayerMask);
        Debug.Log("Detecting....");

        if (currentTarget != null)
        {
            return;
        }

        //if there is something in collider
        if (collider != null)
        {
            target = collider.gameObject;
            currentTarget = target;
            Debug.Log(target.name + " Detected");
            PlayerDetected = true;
        }
        else
        {
            target = Player;
            currentTarget = null;
            PlayerDetected = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmo && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected)
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
}
