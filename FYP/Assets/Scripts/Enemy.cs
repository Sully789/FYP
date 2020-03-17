/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Enemy class contains the health of the enemy and the code to destroy it
 * Source: Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=wkKsl1Mfp5M
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 50;   //How much health the Enemy has

    //Take damage takes health off of the Enemy when it is called
    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    //Die destroys the Enemy Game Object
    void Die()
    {
        Destroy(gameObject);
    }
}
