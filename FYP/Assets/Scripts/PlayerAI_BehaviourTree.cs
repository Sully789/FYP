using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Sequence navigateSceneSeq;

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
    bool shoot = false;

    public Transform player;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    Grid grid;

    Weapon fire;

    public RaycastHit2D hit;
    public Ray2D ray;
    public RaycastHit2D jumpRay;
    public RaycastHit2D shootRay;

    public Text movingText;
    public Text jumpingText;
    public Text shootingText;

    public delegate void TreeExecuted();
    public event TreeExecuted onTreeExecuted;

    void Start()
    {
        Physics2D.queriesStartInColliders = false; // Allow Raycast to not detect itself
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        grid = GetComponent<Grid>();
        fire = GetComponent<Weapon>();

       // hit = GetComponent<RaycastHit2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);


        //First the AI checks for an emeny and will shoot it if it sees one
        //Next the AI will check for a gap, if there is no gap this node fails

        //If there is no enemy to shoot or gap to jump, the enemy will move towards the goal

        shooting = new ActionNode(ShootState);
        jumping = new ActionNode(JumpState);
        moving = new ActionNode(MoveState);

        navigateScene = new Selector(new List<Node> {
            shooting,
            jumping,
            moving,
        });

        navigateSceneSeq = new Sequence(new List<Node> {
            shooting,
            jumping,
            moving,
        });

        // navigateScene.Evaluate();
        //navigateSceneSeq.Evaluate();

        InvokeRepeating("Evaluate", 0f, .5f);
    }

    public void Evaluate()
    {
        navigateSceneSeq.Evaluate();
        Execute();
    }

    private void Update()
    {
        UpdateUI();
        //PathfindMethod();
        Raycast();
        Debug.Log("Reached End of path:" + reachedEndOfPath);
    }

    void Raycast()
    {
        // Cast a ray straight down.
        //Raycast for gaps and enemies
        //  RaycastHit2D jumpGap = Physics2D.Raycast(transform.position, -Vector2.up);
        jumpRay = Physics2D.Raycast(transform.position, -Vector2.up);
        shootRay = Physics2D.Raycast(transform.position, Vector2.right);
       // Jump(jumpGap);
      //  ShootAtEnemy(target);
    }

    void UpdateUI()
    {
        if(moving.nodeState == NodeStates.RUNNING || moving.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(movingText);
        }
        else
        {
            TurnOffActionsUINoCo(movingText);
            //StartCoroutine(TurnOffActionsUI(movingText));
        }

        if (jumping.nodeState == NodeStates.RUNNING || jumping.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(jumpingText);
        }
        else
        {
            TurnOffActionsUINoCo(jumpingText);
            //StartCoroutine(TurnOffActionsUI(jumpingText));
        }

        if (shooting.nodeState == NodeStates.RUNNING || shooting.nodeState == NodeStates.SUCCESS)
        {
            TurnOnActionsUI(shootingText);
        }
        else
        {
            TurnOffActionsUINoCo(shootingText);
            //StartCoroutine(TurnOffActionsUI(shootingText));
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

    private IEnumerator TurnOffActionsUI(Text text)
    {
        yield return new WaitForSeconds(2f);
        text.gameObject.SetActive(false);
    }

    private void Execute()
    {
        Debug.Log("The AI is thinking...");

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

        if (jumping.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI has jumped over a gap");
            
        }
        else
        {
            Debug.Log("The AI has not detected a gap");
            JumpMethod(jumpRay);
        }

        if (moving.nodeState == NodeStates.SUCCESS)
        {
            Debug.Log("The AI is moving towards the goal");
            PathfindMethod();

        }
        else
        {
            Debug.Log("The AI has not reached the goal");
            PathfindMethod();
            // PathfindMethod();
        }

        

        
        //else
       // {
       //     Debug.Log("The AI has reached the Goal.");
       // }
        if (onTreeExecuted != null)
        {
            onTreeExecuted();
        }
    }

    private NodeStates MoveState()
    {
        if (reachedEndOfPath == true)
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
    /*
    public NodeStates Move()
    {
        if (path == null)
        {
            Debug.Log("Move Node Null");
            return NodeStates.RUNNING;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return NodeStates.SUCCESS;
        }
        else
        {
            reachedEndOfPath = false;
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
            return NodeStates.RUNNING;
        }
    }

    private NodeStates Jump()
    {
        if(hit.collider == null)
        {
            Debug.Log("No ray");
        }

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
            return NodeStates.RUNNING;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates Shoot() //if raycast detects enemy, shoot
    {
        //if Raycast detects enemy, shoot at enemy
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            Debug.Log("Inital Ray is hitting: " + hit.transform.name);
            if (hit.collider.tag == "Enemy")
            {
                fire.GetComponent<Weapon>().Shoot();
                Debug.Log("Ray After Check is hitting: " + hit.transform.name);
                return NodeStates.RUNNING;
            }
            else
            {
                Debug.Log("No target detected in if");
                return NodeStates.SUCCESS;
            }

        }
        else
        {
            Debug.Log("No target detected.");
            return NodeStates.FAILURE;
        }
    }
    */
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
    */
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
            Debug.Log("Jump Ray Hit: " + hit.transform.name);
        }
        else
        {
            Debug.Log("No ground detected");
            jump = true;
        }
    }

    void ShootAtEnemyMethod(RaycastHit2D hit)
    {
        //if Raycast detects enemy, shoot at enemy
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            Debug.Log("Inital Ray is hitting: " + hit.transform.name);
            if (hit.collider.tag == "Enemy")
            {
                fire.GetComponent<Weapon>().Shoot();
                shoot = true;
                Debug.Log("Ray After Check is hitting: " + hit.transform.name);
            }
            else
            {
                Debug.Log("No target detected in if");
                shoot = false;
            }

        }

        else
        {
            Debug.Log("No target detected.");
            shoot = false;
        }
    }

    void PathfindMethod()
    {
        if (path == null)
        {
            Debug.Log("Path is null");
            return;
        }

        else if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("Reached end of path is true");
            reachedEndOfPath = true;
            return;
        }
        else
        {
            Debug.Log("Reached end of path is false, far else");
            reachedEndOfPath = false;
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                Debug.Log("In if 1");
                currentWaypoint++;
            }

            if (force.x >= 0.01f)
            {
                Debug.Log("In if 2");
                // player.localScale = new Vector3(-1f, 1f, 1f);
                controller.Move((horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
                // animator.SetFloat("Speed", Mathf.Abs(force.x));
            }
            else if (force.x <= 0.01f)
            {
                Debug.Log("In if 3");
                //player.localScale = new Vector3(1f, 1f, 1f);
                controller.Move(-(horizontalMove * runSpeed) * Time.fixedDeltaTime, crouch, jump);
                //animator.SetFloat("Speed", Mathf.Abs(force.x));
            }
            return;
        }


        Debug.Log("End of pathfind method");
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
