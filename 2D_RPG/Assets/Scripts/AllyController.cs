using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Player,
    Enemy,
    None
}

public class AllyController : MonoBehaviour
{
    [Header("Runtime")]
    public EnemyController targetedEnemy;
    public CharacterType currentCharacterInRadius;

    [Header("Settings")]
    [Space(10)]
    public float rangeToPlayer;
    public float speed;

    [Header("References")]
    [Space(10)]
    public GameObject Player;
    public EnemyManager enemyManager;

    private Rigidbody2D rb2d;
    private Vector3 MoveDirection;
    [SerializeField]
    private bool isInCombat = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInCombat)
        {
            FollowEnemy();
        }
        else
        {
            FollowPlayer();
        }
       
        
    }

    void FollowPlayer()
    {
        //check if player is within range, if it does do nothing. If it isn't, go to player
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > rangeToPlayer)
        {
            //go to player location
            MoveDirection = PlayerController.instance.transform.position - transform.position;
            isInCombat = false;

            Debug.Log("Following Player");
        }
        else
        {
            MoveDirection = Vector2.zero;
            Debug.Log("Stop moving");
        }

        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;
    }

    void FollowEnemy()
    {
        /*
        for (int i = 0, count = enemyManager.Enemies.Count; i < count;i++) 
        {
        }
        */
        if(enemyManager.Enemies.Count < 1) return;
        targetedEnemy = enemyManager.Enemies[0];

        if (!targetedEnemy.gameObject.activeInHierarchy) return;

        //if enemy is in range
        if (Vector3.Distance(transform.position, targetedEnemy.transform.position) > rangeToPlayer)
        {
            //go to enemy location
            MoveDirection = targetedEnemy.transform.position - transform.position;
            Debug.Log("Following Enemy");
            //if enemy is destroyed, remove the enemy from the list in enemymanager list
            isInCombat = true;
        }
        else
        {
            MoveDirection = Vector2.zero;
            Debug.Log("Stop moving");
        }

        MoveDirection.Normalize();
        rb2d.velocity = MoveDirection * speed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if there is CURRENTLY enemy in radius, dont change character type(focus on enemy)
        if (currentCharacterInRadius == CharacterType.Enemy) 
            return; //stop here
        
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            currentCharacterInRadius = CharacterType.Enemy;
            isInCombat = true;
            
        }
        else if (collision.gameObject.GetComponent<PlayerController>())
        {
            currentCharacterInRadius = CharacterType.Player;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyController>() || collision.gameObject.GetComponent<PlayerController>())
        {
            currentCharacterInRadius = CharacterType.None;
            isInCombat = false;
        }
    }


}
