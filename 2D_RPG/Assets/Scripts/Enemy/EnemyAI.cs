using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public GameObject target;
    public GameObject currentTarget;
    public HealthBar healthBar;
    public AudioClip damageSFX;
    public AudioClip deathSFX;
    AudioSource audioSource;

    public EnemyAction enemyAction;

    //Stats
    [Header("Unit Stats")]
    public float speed = 200f;
    public int UnitAttack;
    [SerializeField]
    private int UnitMaxHealth;
    public int UnitHealth;
    public float UnitAttackRange;
    public bool isReadyToAttack = true;
    [SerializeField]
    GameObject DamageEffect;
    public PlayerController player;
    

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

    [Header("Patrol")]
    public float patrolSpeed;
    public Transform[] patrolPoints;
    public int currentPointIndex;
    public float waitTime;
    bool once;


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
        enemyAction = GetComponent<EnemyAction>();
        audioSource = GetComponent<AudioSource>();
        //player = GetComponent<PlayerController>();

        InvokeRepeating("UpdatePath", 0f, MovementDelay);
        StartCoroutine(DetectionCoroutine());
    }


    void Update()
    {
        if (player.isCutscene)
        {
            return;
        }
        else
        {
            healthBar.setHealth(UnitHealth, UnitMaxHealth);
            if (target != null)
            {
                enemyAction.UnitAction();
            }
            else
            {
                enemyPatrol();
            }
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

        //flip unit
        if(target.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        } else if(target.transform.position.x < transform.position.x)
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

    //patrol
    void enemyPatrol()
    {
        if(transform.position != patrolPoints[currentPointIndex].position)
        {
            Anim.SetBool("isMoving", true);
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, patrolSpeed * Time.deltaTime);
        }
        else
        {
            if(once == false)
            {
                Anim.SetBool("isMoving", false);
                once = true;
                StartCoroutine(Wait());
            }
            
        }
        //flip
        if(transform.position.x > patrolPoints[currentPointIndex].position.x)
        {
            transform.localScale = Vector3.one;
        } else if(transform.position.x < patrolPoints[currentPointIndex].position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); ;
        }

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        if(currentPointIndex + 1 < patrolPoints.Length)
        {
            currentPointIndex++;
        }
        else
        {
            currentPointIndex = 0;
        }
        once = false;
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

        //prevent unit to be distracted by new unit that enter gizmo
        //but if implemented, unit cannot change target from the initial(stalking target unless target or it is dead)
        if (currentTarget != null)
        {
            return;
        }

        //if there is something in collider
        if (collider != null)
        {
            target = collider.gameObject;
            currentTarget = target;
            print(collider);
            PlayerDetected = true;

        }
        else
        {
            target = null;
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
        if(UnitHealth > 0)
        {
            audioSource.clip = damageSFX;
            Instantiate(DamageEffect, transform.position, transform.rotation);
            audioSource.Play();
            UnitHealth -= UnitAttack;
        }
        else if (UnitHealth <= 0)
        {
            audioSource.clip = deathSFX;
            audioSource.Play();
            Destroy(this.gameObject,.4f);
        }
    }
}
