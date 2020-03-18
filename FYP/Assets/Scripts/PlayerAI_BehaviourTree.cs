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
    public Selector navigateScene;              //Selector node that acts as the root node of the Behaviour Tree
    public Sequence navigateSceneSeq;           //Sequence node that acts as the root node of the Behaviour Tree
    public ActionNode moving;                   //Leaf node on the Behaviour tree used for movement
    public ActionNode jumpingGap;               //Leaf node on the Behaviour tree used for jumping
    public ActionNode jumpingObstacle;          //Leaf node on the Behaviour tree used for jumping
    public ActionNode shooting;                 //Leaf node on the Behaviour tree used for shooting

    public Transform player;                    //Transform to hold the players location
    public Transform target;                    //Transform to hold the Goals location, used by the Seeker class
    public CharacterController2D controller;    //Character controller used to move the agent
    public Animator animator;                   //Animation used for demoing purposes
    public Text movingText;                     //Text objects to show what Action the agent is taking
    public Text jumpingText;
    public Text shootingText;

    private Rigidbody2D rb;                     //Rigidbody used for moving the agent and collisons
    private Grid grid;                          //grid used by Seeker class
    private Weapon fire;                        //fire used by Weapin class
    private RaycastHit2D jumpGapRay;            //Raycast used for agent to detect gaps
    private RaycastHit2D jumpBlockRay;          //Raycast used for agent to detect Obstacles
    private RaycastHit2D shootRay;              //Raycast used to detect Enemies

    private int currentWaypoint = 0;            //Waypoints used by Seeker class
    private bool reachedEndOfPath = false;      //boolean variable used to track if agent has reached the goal

    private float speed = 200f;                 //speed at which the agent moves
    private float jumpGapRayDist = 3f;          //distance of Raycast that is used to detect gaps
    private float jumpBlockRayDist = 1f;        //distance of Raycast that is used to detect obstacles
    private float shootRayDist = 100f;          //distance of Raycast that is used to detect enemies
    private float nextWaypointDistance = 3;     //distance to the next waypoint used by Seeker class

    private float horizontalMove = 1.0f;        //used by Character Controller to move
    private float runSpeed = 40f;               //speed used by Character Controller
    private bool jump = false;                  //boolean variable used by tree to track if agent has jumped
    private bool crouch = false;                //boolean variable used by tree to track if agent has crouched, unused
    private bool shoot = false;                 //boolean variable used by tree to track if agent has fired at enemy

    private Path path;                          //path is used by Seeker class and Pathfind method to layout path for agent
    private Seeker seeker;                      //Imported library that maps out the path for the agent

    void Start()
    {
        Physics2D.queriesStartInColliders = false;  //Allows Raycast to not detect itself
        seeker = GetComponent<Seeker>();            //Instantiates the Seeker
        rb = GetComponent<Rigidbody2D>();           //Instantiates the Rigidbody
        grid = GetComponent<Grid>();                //Instantiates the Grid
        fire = GetComponent<Weapon>();              //Instantiates the Weapon

        InvokeRepeating("UpdatePath", 0f, .5f);     //Updates the path dynamically

        //Instantiating the Behaviour Tree
        //First the AI checks for an enemy and will shoot it if it sees one
        shooting = new ActionNode(ShootState);
        //Next the AI will check for a gap, if there is no gap this node fails
        jumpingGap = new ActionNode(JumpState);
        //Next the AI will check for a obstacle, if there is no obstacle this node fails
        jumpingObstacle = new ActionNode(JumpState);
        //If there is no enemy to shoot or gap to jump, the enemy will move towards the goal
        moving = new ActionNode(MoveState);
        //Setting up the Action Nodes on the Selector Composite node
        navigateScene = new Selector(new List<Node> {
            shooting,
            jumpingObstacle,
            jumpingGap,
            moving,
        });
        //Setting up the Action Nodes on the Sequence Composite node, this is unused
        navigateSceneSeq = new Sequence(new List<Node> {
            jumpingGap,
            shooting,
            jumpingObstacle,
            moving,
        });

        InvokeRepeating("Evaluate", 0f, .001f);      //Updates the AI's next decision
    }
    
    //Behaviour Tree is called here and its nodes are executed
    public void Evaluate()
    {
        navigateScene.Evaluate();
        Execute();
    }

    //The UI that displays the current task the agent is taken is shown each frame
    private void Update()
    {
        UpdateUI();
        Raycast();
    }

    //The Pathfinding and the Raycasts are done in the FixedUpdate method as they include Physics calculations
    private void FixedUpdate()
    {
        PathfindMethod();
        Raycast();
    }

    //Raycast method handles the raycasts that are used to detect enemies, obstacles and gaps
    void Raycast()
    {
        //Cast a ray straight down, if it does not detect anything there is a gap
        jumpGapRay = Physics2D.Raycast(transform.position, -Vector2.up, jumpGapRayDist);
        //Cast a ray straight ahead, if it detects something, there is an obstacle
        jumpBlockRay = Physics2D.Raycast(transform.position, Vector2.right, jumpBlockRayDist);
        //Cast a ray straight ahead, longer than the previous ray, if it detects something, there is an enemy
        shootRay = Physics2D.Raycast(transform.position, Vector2.right, shootRayDist);
    }

    //Turns on UI elements to indicate what action the agent is currently taking
    void UpdateUI()
    {
        if (moving.nodeState == NodeStates.SUCCESS) //Turns on if the agent is moving
        {
            TurnOnActionsUI(movingText);
        }
        else
        {
            TurnOffActionsUINoCo(movingText);
        }

        if(jumpingGap.nodeState == NodeStates.SUCCESS || jumpingObstacle.nodeState == NodeStates.SUCCESS)   //Turns on if the agent is jumping
        {
            TurnOnActionsUI(jumpingText);
        }
        else
        {
            StartCoroutine(TurnOffActionsUI(jumpingText));
        }

        if(shooting.nodeState == NodeStates.SUCCESS)    //Turns on if the agent is shooting
        {
            TurnOnActionsUI(shootingText);
        }
        else
        {
            TurnOffActionsUINoCo(shootingText);
        }

    }

    //Method turns on the required UI
    private void TurnOnActionsUI(Text text)
    {
        text.gameObject.SetActive(true);
    }

    //Method turns off the required UI
    private void TurnOffActionsUINoCo(Text text)
    {
        text.gameObject.SetActive(false);
    }

    //Coroutine that leaves UI elements on screen for a few seconds before turning off
    private IEnumerator TurnOffActionsUI(Text text)
    {
        yield return new WaitForSeconds(3f);
        text.gameObject.SetActive(false);
    }

    //Main method of the Behaviour Tree, Debug messages display info on what the AI is currently evaluating
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
        
        //Could not get a sufficent movement working within tree, Movement is instead in PathfindingMethod
        if (moving.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is moving towards the goal");

        }
        else
        {
            Debug.Log("The AI has reached the goal");

        }
    }

    //Returns a Successful node to the Behaviour Tree if the agent has reached the end of the path
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

    //Returns a Successful node to the Behaviour Tree if the agent has jumped
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

    //Returns a Successful node to the Behaviour Tree if the agent has fired at an enemy
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

    //Used by Seeker class to clear waypoints
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    //Used by Seeker class to update the path the agent will take
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    //Returns a boolean to the Controller that is used when the agent needs to jump over a gap
    void JumpMethod(RaycastHit2D hit)
    {
        //if Raycast detects gap, jump over gap
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            if (hit.collider.tag != "Tilemap")
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
        else
        {
            jump = false;
            animator.SetBool("IsJumping", false);
        }
    }

    //Returns a boolean to the Controller that is used when the agent needs to jump over an obstacle
    void JumpBlockMethod(RaycastHit2D hit)
    {
        //if Raycast detects obstacle, jump over obstacle
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            if (hit.collider.tag == "Tilemap")
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
        else
        {
            jump = false;
            animator.SetBool("IsJumping", false);
        }
    }

    //Spawns a Bullet object if the raycast detects an enemy
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

    //Main pathfinding method built on from PlayerAI class, could not get implementation working within Tree, runs in FixedUpdate
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
