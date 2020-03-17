/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Enemy AI uses Aron Granberg's A* Pathfinding Project to find the Player
 * Source: Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=jvtFUfJ6CP8
 * https://arongranberg.com/astar/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float speed = 200f;                  //speed at which the enemy moves
    public float nextWaypointDistance = 3;      //Distance between each Waypoint

    public Transform target;                    //target to Pathfind to, assigned in the Inspector
    public Transform enemyGFX;                  //location of Enemy

    private int currentWaypoint = 0;            //current waypoint that the agent is at
    private bool reachedEndOfPath = false;      //boolean value that tracks when the agent has reached the end of the path

    private Path path;                          //Path class from Aron Granberg's A* Pathfinding Project, used as a reference to the path
    private Seeker seeker;                      //Seeker class Aron Granberg's A* Pathfinding Project, used to handle the pathfinding
    private Rigidbody2D rb;                     //Rigidbody of the Enemy class used for collisions

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        
    }

    // FixedUpdate is called a fixed number of times per second
    void FixedUpdate()
    {
        //if statements returns if no path has been assigned
        if(path == null)
        {
            return;
        }

        //if statement returns if the agent has reached the end of the path
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        //if not, the reachedEndOfPath boolean is set to false
        else
        {
            reachedEndOfPath = false;
        }
        //Vector 2 that gets the direction that the agent must travel towards
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        //Vector 2 that assigns how fast the agent moves
        Vector2 force = direction * speed * Time.deltaTime;

        //AddForce method moves the Rigidbody of the agent
        rb.AddForce(force);

        //Vector 2 that updates the distance of the agent from the goal
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        //if statement updates the waypoint
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        //Changes direction of the agent depending on where they are moving
        if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= 0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    //Sets the next waypoint to 0 when the path is complete
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Updates the path
    void UpdatePath()
    {
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        } 
    }
}
