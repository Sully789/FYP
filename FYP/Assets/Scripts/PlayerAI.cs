/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * PlayerAI was the original class that was used to navigate the scene, PlayerAI_BehaviourTree expanded on this class and replaced it
 * Apdated from Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=dwcT-Dch0bA
 * https://www.youtube.com/watch?v=jvtFUfJ6CP8
 * https://arongranberg.com/astar/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerAI : MonoBehaviour
{
    
    public Transform target;
    public CharacterController2D controller;
    public Animator animator;

    public float speed = 200f;
    public float nextWaypointDistance = 3;


    float horizontalMove = 1.0f;
    float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    public Transform player;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    Grid grid;

    Weapon fire;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesStartInColliders = false; // Allow Raycast to not detect itself
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        grid = GetComponent<Grid>();
        fire = GetComponent<Weapon>();

        InvokeRepeating("UpdatePath", 0f, .5f);

    }

    // FixedUpdate is called a fixed number of times per second
    void FixedUpdate()
    {
        Pathfind();           
        Raycast();
    }

    void Pathfind()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (force.x >= 0.01f)
        {
            controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
             animator.SetFloat("Speed", Mathf.Abs(force.x));
        }
        else if (force.x <= 0.01f)
        {
            controller.Move(-(horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
            animator.SetFloat("Speed", Mathf.Abs(force.x));
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

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void Movement()
    {
        controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
    }

    void Raycast()
    {
        // Cast a ray straight down.
        //Raycast for gaps and enemies
        RaycastHit2D jumpGap = Physics2D.Raycast(transform.position, -Vector2.up);
        RaycastHit2D target = Physics2D.Raycast(transform.position, Vector2.right);
        Jump(jumpGap);
        ShootAtEnemy(target);
    }

    void Jump(RaycastHit2D hit)
    {
        //if Raycast detects gap, jump over gap
        // If it hits something...
        if (hit.collider != null)
        {
            Debug.Log("Jump Ray Hit: " + hit.transform.name);
            jump = false;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            Debug.Log("No ground detected");
            jump = true;
            animator.SetBool("IsJumping", true);
        }

    }

    void Crouch()
    {
        //if Raycast detects overhead obstacle, duck under osbtacle
    }

    void ShootAtEnemy(RaycastHit2D hit)
    {
        //if Raycast detects enemy, shoot at enemy
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            Debug.Log("Inital Ray is hitting: " + hit.transform.name);
            if (hit.collider.tag == "Enemy")
            {
                fire.GetComponent<Weapon>().Shoot();
                Debug.Log("Ray After Check is hitting: " + hit.transform.name);
            }
            else
            {
                Debug.Log("No target detected in if");
               
            }
                       
        }

        else
        {
            Debug.Log("No target detected.");
        }
    }
    
}
