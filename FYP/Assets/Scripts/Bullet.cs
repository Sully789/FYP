/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Bullet class handles the Bullets that are spawned when a shot is fired by the PlayerAI
 * Source: Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=wkKsl1Mfp5M
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 20f;      // speed at which the bullet travels out
    private int damage = 40;        // how much health to take away from an Enemy

    private Rigidbody2D rb;         //Rigidbody of the bullet used to test collisions

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }
 
    //OnTriggerEnter2D checks if the collision is from an Enemy and the Enemy takes damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        
    }
    
}
