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
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        PlayerInput();
    }

    //ŒÔ¿Ì
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
        movementInput = new Vector3(inputX, inputY);

    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
}
