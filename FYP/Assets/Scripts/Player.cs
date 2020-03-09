using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 200f;

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isShooting = false;
    private bool isAtGoal = false;

   
    public bool IsMoving
    {
        get { return isMoving; }
    }

    public bool IsJumping
    {
        get { return isJumping; }
    }

    public bool IsShooting
    {
        get { return isShooting; }
    }

    public bool IsAtGoal
    {
        get { return isAtGoal; }
    }

    public bool Moving()
    {
        isMoving = true;
        Debug.Log("Moving");
        return isMoving;
    }

    public bool Jumping()
    {
        isJumping = true;
        Debug.Log("Jump");
        return isJumping;
    }

    public bool Shooting()
    {
        isShooting = true;
        //code to shoot
        Debug.Log("Shoot");
        return isShooting;
    }

    public bool AtGoal()
    {
        isAtGoal = true;
        //code to reach goal flag
        Debug.Log("Reached Goal");
        return isAtGoal;
    }
}
