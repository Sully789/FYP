/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * Weapon is used to shoot a Bullet from the firepoint that is assigned to the PlayerAI
 * Source: Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=wkKsl1Mfp5M
*/
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        bullet.layer = 2;
    }

    // Update is called once per frame
    void Update()
    {
       // if (Input.GetButtonDown("Fire1"))
       // {
      //      Shoot();
      //  }
    }

    public void Shoot()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation); 
    }
}
