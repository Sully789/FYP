/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Selector is a composite node that handles the calling of the child node in the Behaviour Tree
 * Source: Unity 2017 Game AI Programming Third Edition published by Packt
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Nodes/Selector.cs
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    /** The child nodes for this selector */
    protected List<Node> m_nodes = new List<Node>();


    /** The constructor requires a lsit of child nodes to be 
     * passed in*/
    public Selector(List<Node> nodes)
    {
        m_nodes = nodes;
    }

    /* If any of the children reports a success, the selector will
     * immediately report a success upwards. If all children fail,
     * it will report a failure instead.*/
    public override NodeStates Evaluate()
    {
        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    continue;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}
