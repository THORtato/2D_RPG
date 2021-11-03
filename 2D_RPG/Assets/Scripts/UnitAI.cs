﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class UnitAI : MonoBehaviour
{
    public GameObject target;

    //Stats
    [Header("Unit Stats")]
    public float speed = 200f;
    public int UnitDamage;
    public int UnitHealth;
    public float UnitAttackRange;
    public bool isReadyToAttack = true;
    public float delay;

    //Movement
    [Header("Movement Settings")]
    Path path;
    int currentWaypoint = 0;
    public Vector3 Range;
    //how close enemy need to a waypoint(?) before it moves on to the next one
    public float nextWaypointDistance = 3f;
    public GameObject Player;
    bool reachEndofPath = false;
    Seeker seeker;
    Rigidbody2D rb;

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



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        StartCoroutine(DetectionCoroutine());
        

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
        UnitDeath();
        
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
            Debug.Log(distance);
            currentWaypoint++;
        }


        //flip Unit
        if (rb.velocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            Debug.Log("Turned Right");
        }
        else if (rb.velocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Debug.Log("Turned Left");
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
        Collider2D collider = Physics2D.OverlapCircle((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize,detectorLayerMask);
        Debug.Log("Detecting....");

        if(collider != null)
        {
            
            target = collider.gameObject;
            Debug.Log(target.name + " Detected");
            PlayerDetected = true;
        }
        else
        {
            target = null;
            PlayerDetected = false;
            FollowPlayer();
        }
    }

    private void OnDrawGizmos()
    {
        if(showGizmo && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawSphere((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }

    void FollowPlayer()
    {
        target = Player;
    }


    //Action
    void UnitAction()
    {
        if (delay > 2f)
        {
            if(this.tag == "Enemy"  && target.tag == "Player"  )
            {
                target.GetComponent<PlayerController>().playerHealth -= UnitDamage;
                Debug.Log("Attacking" + target.name);
                delay = 0;
            } else
            {
                target.GetComponent<UnitAI>().UnitHealth -= UnitDamage;
                Debug.Log("Attacking" + target.name);
                delay = 0;
            }
            
        }
        delay += Time.deltaTime;

        

       /*
        *if enemy is in attack distance, do action
         */

    }

    void UnitDeath()
    {
        if(UnitHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
