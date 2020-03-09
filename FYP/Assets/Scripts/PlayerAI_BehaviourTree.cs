using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerAI_BehaviourTree : MonoBehaviour
{
  //  public GameObject player;
    public GameObject enemy;
    public GameObject goal;
    
    public Selector navigateScene;
    public ActionNode moving;
    public ActionNode jumping;
    public ActionNode shooting;


    public Transform target;
    public CharacterController2D controller;
    public Animator animator;

    public float speed = 200f;
    public float nextWaypointDistance = 3;

    // public float floatHeight;     // Desired floating height.
    // public float liftForce;       // Force to apply when lifting the rigidbody.
    // public float damping;         // Force reduction proportional to speed (reduces bouncing).

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

    RaycastHit2D x;


    void Start()
    {
        //shooting = new ActionNode(Shoot());
        //jumping = new ActionNode(Jump);
        moving = new ActionNode(Move);

        List<Node> rootChildrenTest = new List<Node>();
        rootChildrenTest.Add(moving);
        rootChildrenTest.Add(jumping);
        rootChildrenTest.Add(shooting);

        navigateScene = new Selector(rootChildrenTest);

        navigateScene.Evaluate();

        
        Physics2D.queriesStartInColliders = false; // Allow Raycast to not detect itself
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        grid = GetComponent<Grid>();
        fire = GetComponent<Weapon>();

        InvokeRepeating("UpdatePath", 0f, .5f);


    }

    void FixedUpdate()
    {
        Pathfind();
    }

    public NodeStates Move()
    {
        if (path == null)
        {
            Pathfind();
            Debug.Log("Path is null");
            return NodeStates.SUCCESS;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("Reached end of path");
            reachedEndOfPath = true;
            return NodeStates.SUCCESS;
        }
        else
        {
            Debug.Log("Not at end of path");
            reachedEndOfPath = false;
            return NodeStates.FAILURE;
        }
    }

    private NodeStates Jump(RaycastHit2D hit)
    {
        //if Raycast detects gap, jump over gap
        // If it hits something...
        if (hit.collider != null)
        {
            Debug.Log("Jump Ray Hit: " + hit.transform.name);
            return NodeStates.FAILURE;
        }
        else
        {
            Debug.Log("No ground detected");
            jump = true;
            return NodeStates.SUCCESS;
        }
    }

    private NodeStates Shoot(RaycastHit2D hit) //if raycast detects enemy, shoot
    {
        //if Raycast detects enemy, shoot at enemy
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            Debug.Log("Inital Ray is hitting: " + hit.transform.name);
            if (hit.collider.tag == "Enemy")
            {
                fire.GetComponent<Weapon>().Shoot();
                Debug.Log("Ray After Check is hitting: " + hit.transform.name);
                return NodeStates.SUCCESS;
            }
            else
            {
                Debug.Log("No target detected in if");
                return NodeStates.FAILURE;
            }

        }
        else
        {
            Debug.Log("No target detected.");
            return NodeStates.FAILURE;
        }
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
            // player.localScale = new Vector3(-1f, 1f, 1f);
            controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
            // animator.SetFloat("Speed", Mathf.Abs(force.x));
        }
        else if (force.x <= 0.01f)
        {
            // player.localScale = new Vector3(1f, 1f, 1f);
            controller.Move(-(horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
            //animator.SetFloat("Speed", Mathf.Abs(force.x));
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

    /*
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
            // player.localScale = new Vector3(-1f, 1f, 1f);
            controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
            // animator.SetFloat("Speed", Mathf.Abs(force.x));
        }
        else if (force.x <= 0.01f)
        {
            // player.localScale = new Vector3(1f, 1f, 1f);
            controller.Move(-(horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
            //animator.SetFloat("Speed", Mathf.Abs(force.x));
        }
    }

    void Raycast()
    {
        // Cast a ray straight down.
        //Raycast for gaps and enemies
        //  RaycastHit2D jumpGap = Physics2D.Raycast(transform.position, -Vector2.up);
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
        }
        else
        {
            Debug.Log("No ground detected");
            jump = true;
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

    /*


    /*
    private Player data;

    public ActionNode moveCheckNode;
    public ActionNode jumpCheckNode;
    public ActionNode shootCheckNode;
    public ActionNode goalCheckNode;
    public Sequence moveToGoalSequence;
    public Selector rootNode;

    public delegate void TreeExecuted();
    public event TreeExecuted onTreeExecuted;

    public delegate void NodePassed(string trigger);
    
	void Start ()
    {
        moveCheckNode = new ActionNode(MoveCheck);

        jumpCheckNode = new ActionNode(JumpCheck);

        shootCheckNode = new ActionNode(ShootCheck);

        goalCheckNode = new ActionNode(GoalCheck);

        moveToGoalSequence = new Sequence(new List<Node> {
            shootCheckNode,
            //moveCheckNode,
            jumpCheckNode,
        });

        rootNode = new Selector(new List<Node> {
           // moveCheckNode,
            shootCheckNode,
            jumpCheckNode,
            moveToGoalSequence,
        });

        Evaluate();
	}

    public void SetPlayerData(Player ai) {
        data = ai;
    }
	
	public void Evaluate() {
        rootNode.Evaluate();
        StartCoroutine(Execute());
    }

    private IEnumerator Execute() {
        Debug.Log("The AI is thinking...");
        yield return new WaitForSeconds(2.5f);

        if (shootCheckNode.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI decided to shoot");
            data.Shooting();
        }
        else if (moveCheckNode.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is moving.");
            data.Moving();
        }
        else if (jumpCheckNode.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is jumping");
            data.Jumping();
        }
        else
        {
            Debug.Log("The AI has reached the goal");
            data.AtGoal();
        }

        if(onTreeExecuted != null) {
            onTreeExecuted();
        }
    }

    private NodeStates MoveCheck()
    {
        if (data.IsMoving)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates JumpCheck()
    {
        if (data.IsJumping)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates ShootCheck()
    {
        if (data.IsShooting)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates GoalCheck()
    {
        if (data.IsAtGoal)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    */
}
