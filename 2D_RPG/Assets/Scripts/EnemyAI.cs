using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public GameObject target;

    public float speed = 200f;

    //how close enemy need to a waypoint(?) before it moves on to the next one
    public float nextWaypointDistance = 3f;
    public int UnitDamage;
    public float UnitAttackRange;
    public int EnemyHealth;
    public bool isReadyToAttack = true;
    public float delay;

    //current path following
    Path path;
    int currentWaypoint = 0;
    bool reachEndofPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    //enemy detection
    public bool TargetDetected;
    public Vector2 DirectionToTarget => target.transform.position - detectorOrigin.position;
    public Transform detectorOrigin;
    public float detectorSize;
    public Vector2 detectorOriginOffset = Vector2.zero;

    public float detectionDelay = .5f;
    public LayerMask detectorLayerMask;

    //enemy detection gizmo
    [Header("Gizmos")]
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmo = true;



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        StartCoroutine(DetectionCoroutine());
        

    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            Vector3 range = new Vector3(.2f, .2f);
            //generating path
            if(target != null)
            {
                seeker.StartPath(rb.position, target.transform.position - range, OnPathComplete);
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
    private void Update()
    {
        if (target != null)
        {
            if (Vector2.Distance(transform.position, target.transform.position) < UnitAttackRange)
            {
                UnitAction();
            }
        }
        
        /* 
        Check if something is in radius
	        if is in radius, then
		        pathfinding to somthing
			        if distance between this and something < actionDistance
				        Action(); / fuzzy logic
	        if not in radius, then
		        Patrol();
         */

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowEnemy();
    }

    //follow enemy
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
            Debug.Log(distance);
            currentWaypoint++;
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
        Collider2D collider = Physics2D.OverlapCircle((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize,detectorLayerMask);
        Debug.Log("Detecting....");

        if(collider != null)
        {
            target = collider.gameObject;
            TargetDetected = true;
        }
        else
        {
            target = null;
            TargetDetected = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(showGizmo && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (TargetDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawSphere((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }

    //Action
    void UnitAction()
    {
        
        if(delay > 2f)
        {
            if (target.tag == "Player")
            {
                target.GetComponent<PlayerController>().health -= UnitDamage;
            }
            else if (target.tag == "Companion")
            {
                target.GetComponent<UnitAI>().UnitHealth -= UnitDamage;
            }
            delay = 0;
        }
        delay += Time.deltaTime;


    }

    /*
    IEnumerator AttackEnemy()
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(2f);
        isReadyToAttack = true;
        UnitAction();
    }*/
}
