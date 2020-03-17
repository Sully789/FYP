/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Sequence is a composite node that works similarly to the Selector, but all child nodes need to return true for the sequence to return true
 * Source: Unity 2017 Game AI Programming Third Edition published by Packt
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Nodes/Sequence.cs
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    /** Chiildren nodes that belong to this sequence */
    private List<Node> m_nodes = new List<Node>();

    /** Must provide an initial set of children nodes to work */
    public Sequence(List<Node> nodes)
    {
        m_nodes = nodes;
    }

    /* If any child node returns a failure, the entire node fails. Whence all 
     * nodes return a success, the node reports a success. */
    public override NodeStates Evaluate()
    {
        bool anyChildRunning = false;

        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
            }
        }
        m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return m_nodeState;
    }
}
