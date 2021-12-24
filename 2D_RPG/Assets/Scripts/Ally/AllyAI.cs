using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AllyAI : MonoBehaviour
{
    public GameObject target;
    public GameObject Player;
    public GameObject currentTarget;
    public HealthBar healthBar;

    //public EnemyAction enemyAction;
    public HealerAction healerAction;

    //Stats
    [Header("Unit Stats")]
    public float speed = 200f;
    public int UnitDamage;
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
            FollowEnemy();
            healerAction.HealerDecision();
        }
        else
        {
            //enemyPatrol();
        }
        UnitDeath();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowEnemy();
    }

    //follow enemy / Pathfinding
    void FollowEnemy()
    {
        Debug.Log("Following " + target);
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
        
        //animation
        if(rb.velocity != Vector2.zero)
        {
            Anim.SetBool("isMoving", true);
        }
        else
        {
            Anim.SetBool("isMoving", false);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance <= nextWaypointDistance)
        {
            //Debug.Log(distance);
            currentWaypoint++;
        }


        //flip Unit
        if (rb.velocity.x >= 0.1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            //Debug.Log("Turned Right");
        }
        else if (rb.velocity.x <= -0.1f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            //Debug.Log("Turned Left");
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
            target = null;
            currentTarget = null;
            PlayerDetected = false;
        }
    }

    void FollowPlayer()
    {
        target = Player;
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

    void UnitDeath()
    {
        if (UnitHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
