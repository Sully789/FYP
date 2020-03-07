/* Code from Unity 2017 Game AI Programming Book
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/tree/master/Chapter06/Assets/Scripts/Nodes
 * 
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

/** Enum containing the possible states a node can return */
public enum NodeStates
{
    SUCCESS,
    FAILURE,
    RUNNING,
}