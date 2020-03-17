/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Node is a base abstract class that is used as a blueprint for all other nodes in the Behaviour Tree
 * Source: Unity 2017 Game AI Programming Third Edition published by Packt
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Nodes/Node.cs
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Node
{

    /* Delegate that returns the state of the node.*/
    public delegate NodeStates NodeReturn();

    /* The current state of the node */
    protected NodeStates m_nodeState;

    public NodeStates nodeState
    {
        get { return m_nodeState; }
    }

    /* The constructor for the node */
    public Node() { }

    /* Implementing classes use this method to valuate the desired set of conditions */
    public abstract NodeStates Evaluate();

}