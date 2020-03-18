/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * PlayerAI_BehaviourTree uses the Seeker class as well as a Behaviour Tree to Move, Jump and Shoot through the scene
 * Adapted from Unity 2017 Game AI Programming Third Edition published by Packt
 * Uses Aron Granberg's A* Pathfinding Project and Brackeys 2D Character Controller 
 * Sources:
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Samples/CardGame/BehaviorTrees/EnemyBehaviorTree.cs
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Samples/MathTree.cs
 * https://www.youtube.com/watch?v=dwcT-Dch0bA
 * https://www.youtube.com/watch?v=jvtFUfJ6CP8
 * https://arongranberg.com/astar/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class PlayerAI_BehaviourTree : MonoBehaviour
{
    public Selector navigateScene;          //Selector node that acts as the root node of the Behaviour Tree
    public Sequence navigateSceneSeq;       //Sequence node that acts as the root node of the Behaviour Tree
    public ActionNode moving;               //Leaf node on the Behaviour tree used for movement
    public ActionNode jumpingGap;              //Leaf node on the Behaviour tree used for jumping
    public ActionNode jumpingObstacle;              //Leaf node on the Behaviour tree used for jumping
    public ActionNode shooting;             //Leaf node on the Behaviour tree used for shooting

    public Transform player;            
    public Transform target;
    public CharacterController2D controller;
    public Animator animator;
    public Text movingText;
    public Text jumpingText;
    public Text shootingText;

    public delegate void TreeExecuted();
    public event TreeExecuted onTreeExecuted;

    private Rigidbody2D rb;
    private Grid grid;
    private Weapon fire;
    private RaycastHit2D jumpGapRay;
    private RaycastHit2D jumpBlockRay;
    private RaycastHit2D shootRay;

    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private float speed = 200f;
    private float jumpGapRayDist = 3f;
    private float jumpBlockRayDist = 1f;
    private float shootRayDist = 100f;
    private float nextWaypointDistance = 3;

    private float horizontalMove = 1.0f;
    private float runSpeed = 40f;
    private bool jump = false;
    private bool crouch = false;
    private bool shoot = false;


    private Path path;
    private Seeker seeker;

    private Vector2 velocity;
    private Vector2 jumpVelocity;

    void Start()
    {
        Physics2D.queriesStartInColliders = false; // Allow Raycast to not detect itself
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        grid = GetComponent<Grid>();
        fire = GetComponent<Weapon>();


        InvokeRepeating("UpdatePath", 0f, .5f);
        velocity = new Vector2(2f, 0f);
        jumpVelocity = new Vector2(1f, 100f);


        //First the AI checks for an emeny and will shoot it if it sees one
        //Next the AI will check for a gap, if there is no gap this node fails

        //If there is no enemy to shoot or gap to jump, the enemy will move towards the goal

        shooting = new ActionNode(ShootState);
        jumpingGap = new ActionNode(JumpState);
        jumpingObstacle = new ActionNode(JumpState);
        moving = new ActionNode(MoveState);

        navigateScene = new Selector(new List<Node> {
            shooting,
            jumpingObstacle,
            jumpingGap,
            moving,
        });

        navigateSceneSeq = new Sequence(new List<Node> {
            jumpingGap,
            shooting,
            jumpingObstacle,
            moving,
        });

        // navigateScene.Evaluate();
        //navigateSceneSeq.Evaluate();


        InvokeRepeating("Evaluate", 0f, .25f);
    }

    public void Evaluate()
    {
        navigateScene.Evaluate();
        Execute();
    }

    private void Update()
    {
        UpdateUI();
       // Raycast();
    }

    private void FixedUpdate()
    {
        PathfindMethod();
        Raycast();
    }

    void Raycast()
    {
        // Cast a ray straight down.
        //Raycast for gaps and enemies
        //  RaycastHit2D jumpGap = Physics2D.Raycast(transform.position, -Vector2.up);
        jumpGapRay = Physics2D.Raycast(transform.position, -Vector2.up, jumpGapRayDist);
        jumpBlockRay = Physics2D.Raycast(transform.position, Vector2.right, jumpBlockRayDist);
        shootRay = Physics2D.Raycast(transform.position, Vector2.right, shootRayDist);
       // Jump(jumpGap);
      //  ShootAtEnemy(target);
    }

    void UpdateUI()
    {
        if (moving.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(movingText);
        }
        else
        {
            TurnOffActionsUINoCo(movingText);
        }

        if(jumpingGap.nodeState == NodeStates.SUCCESS || jumpingObstacle.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(jumpingText);
        }
        else
        {
            TurnOffActionsUINoCo(jumpingText);
        }

        if(shooting.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(shootingText);
        }
        else
        {
            TurnOffActionsUINoCo(shootingText);
        }

    }

    private void TurnOnActionsUI(Text text)
    {
        text.gameObject.SetActive(true);
    }

    private void TurnOffActionsUINoCo(Text text)
    {
        text.gameObject.SetActive(false);
    }


    private void Execute()
    {
        Debug.Log("The AI is thinking...");

        if (jumpingGap.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI has jumped over a gap");
            JumpMethod(jumpGapRay);

        }
        else
        {
            Debug.Log("The AI has not detected a gap");
            JumpMethod(jumpGapRay);
        }

        if (shooting.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is shooting an Enemy");
            ShootAtEnemyMethod(shootRay);

        }
        else
        {
            Debug.Log("The AI has no target");
            ShootAtEnemyMethod(shootRay);

        }
        
        if (jumpingObstacle.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI has jumped over an obstacle");
            JumpBlockMethod(jumpBlockRay);

        }
        else
        {
            Debug.Log("The AI has not detected an obstacle");
            JumpBlockMethod(jumpBlockRay);
        }
        
        if (moving.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is moving towards the goal");

        }
        else
        {
            Debug.Log("The AI has not reached the goal");

        }

        if (onTreeExecuted != null)
        {
            onTreeExecuted();
        }
    }

    private NodeStates MoveState()
    {
        if (reachedEndOfPath == false)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates JumpState()
    {
        if (jump == true)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates ShootState()
    {
        if (shoot == true)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
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

    void JumpMethod(RaycastHit2D hit)
    {
        //if Raycast detects gap, jump over gap
        // If it hits something...
        if (hit.collider != null)
        {
            jump = false;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }
    }

    void JumpBlockMethod(RaycastHit2D hit)
    {
        //if Raycast detects gap, jump over gap
        // If it hits something...
        if (hit.collider != null)
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }
        else
        {  
            jump = false;
            animator.SetBool("IsJumping", false);
        }
    }

    void ShootAtEnemyMethod(RaycastHit2D hit)
    {
        //if Raycast detects enemy, shoot at enemy
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            if (hit.collider.tag == "Enemy")
            {
                fire.GetComponent<Weapon>().Shoot();
                shoot = true;
            }
            else
            {
                shoot = false;
            }

        }
        else
        {
            shoot = false;
        }
    }

    void PathfindMethod()
    {
        //if statements returns if no path has been assigned
        if (path == null)
        {
            return;
        }

        //if statement returns if the agent has reached the end of the path
        else if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        //if not, the reachedEndOfPath boolean is set to false
        else
        {
            reachedEndOfPath = false;

            //Vector 2 that gets the direction that the agent must travel towards
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

            //Vector 2 that assigns how fast the agent moves
            Vector2 force = direction * speed * Time.deltaTime;

            //AddForce method moves the Rigidbody of the agent
            rb.AddForce(force);

            //Vector 2 that updates the distance of the agent from the goal
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            //if statement updates the waypoint
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            //Changes direction of the agent depending on where they are moving
            if (force.x >= 0.01f)
            {
                controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
                animator.SetFloat("Speed", Mathf.Abs(force.x));  //Adds run animation for demo
            }
            else if (force.x <= 0.01f)
            {
                controller.Move(-(horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
                animator.SetFloat("Speed", Mathf.Abs(force.x)); //Adds run animation for demo
            }
            return;
        }
    }

   
}
