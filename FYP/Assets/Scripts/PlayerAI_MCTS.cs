/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * PlayerAI_MCTS intended to use a Monte Carlo Tree Search to handle the navigation of the scene but was not used, only pseudocode 
 * References include Monte Mario and Monster Carlo papers
 * https://adamsmith.as/papers/monster-carlo-cig18.pdf
 * http://julian.togelius.com/Jacobsen2014Monte.pdf
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI_MCTS : MonoBehaviour
{
    /*
    void MCTS()
    {
        //While game active... in Update method?
            //leaf = navigation(root)
            //simulation result = rollout(leaf)
            //backprogagate (leaf, simulation result)

        //return best child(root)
    }

    //Method for node traversal 
    void Navigate()
    {
        //While fully expanded(node)
            //node = best UCT(node)
        
        //if no children present, node is terminal
            //return unvisited node OR node
    }

    //Method for the result of the simulation 
    void Rollout()
    {
        //While not a terminal node
            //Node = rollout policy(node)

        //return result(node)
    }

    //Method for randomly selecting a child node 
    void RolloutPoilicy()
    {
        //return Random Node(children.node) 
    }

    //Method for backpropagation 
    void Backpropagation()
    {
        //if root node
            //return
        //node.stats = update_stats(node, result)
        //backpropagate(node.parent) 
    }

    //Method for selecting the best child 
    void BestChild()
    {
        //pick child with highest number of visits
    }
    */
}