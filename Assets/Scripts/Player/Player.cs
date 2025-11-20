using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;

    private float inputX;

    private float inputY;

    private Vector2 movementInput;

    private Animator[] animators;

    private bool isMoving;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators=GetComponentsInChildren<Animator>();
    }
    
    private void Update()
    {
        PlayerInput();
        SwitchAnimation();
    }

    //物理
    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {

        //if(intputY == 0)
        inputX = Input.GetAxisRaw("Horizontal");
        //if(intputX == 0)
        inputY = Input.GetAxisRaw("Vertical");


        if (inputX != 0 && inputY != 0) 
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }

        movementInput = new Vector3(inputX, inputY);

        isMoving = movementInput != Vector2.zero;

    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);

            if(isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
}
