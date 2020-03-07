/* Code from Unity 2017 Game AI Programming Book
 * https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/tree/master/Chapter06/Assets/Scripts/Nodes
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBinaryNode : Node
{
    public override NodeStates Evaluate()
    {
        var roll = Random.Range(0, 2);
        return (roll == 0 ? NodeStates.SUCCESS : NodeStates.FAILURE);
    }
}
