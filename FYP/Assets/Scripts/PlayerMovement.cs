/* 
 * Sean O'Sullivan | K00180620 | Year 4 | Final Year Project | Pathfinding Algorithm that uses A* and a Behaviour Tree to navigate a Platformer level
 * PlayerMovement class was used in testing the scene, the algrorithm was built on these movements, class is unused 
 * Source: Brackeys 2D Platformer Game Tutorial
 * https://www.youtube.com/watch?v=dwcT-Dch0bA
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*
    public CharacterController2D controller;
    public Animator animator;
    float horizontalMove = 0f;
    float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

    }

    void PlayerInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    private void FixedUpdate()
    {
        //Move character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
    */
}
