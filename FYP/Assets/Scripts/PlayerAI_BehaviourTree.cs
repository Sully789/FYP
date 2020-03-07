using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI_BehaviourTree : MonoBehaviour
{
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
            moveCheckNode,
            jumpCheckNode,
        });

        rootNode = new Selector(new List<Node> {
            moveCheckNode,
            shootCheckNode,
            jumpCheckNode,
            moveToGoalSequence,
        });
        
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
}
