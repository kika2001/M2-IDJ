using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movespeed = 6;
    [SerializeField] float currentspeed;
    [SerializeField] float runspeed = 9;
    [SerializeField] float jumpheight = 2;
    public float gravity;

    [Range(0, 10), SerializeField] float airControl = 5;

    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    Rigidbody rb;
    Vector3 input;
    public bool canRun;
    public bool isgrounded;
    private LayerMask layer;
    void Start()
    {
        currentspeed = movespeed;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        layer = 9;
        layer = ~layer;
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //------------Checker-------------
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hit, 1.2f,layer))
        {
            isgrounded = true;
            Debug.Log("Is Grounded");
        }
        else
        {
            isgrounded = false;
        }
        if (input.z >0 && input.x <=0)
        {
            canRun = true;
        }
        else
        {
            canRun = false;
        }

       
        //------------Checker-------------

        //-------------Run---------------
        if (Input.GetKey(KeyCode.LeftShift) && isgrounded == true && canRun==true)
        {

            currentspeed = runspeed;
        }
        else if (!Input.GetKeyDown(KeyCode.LeftShift) && isgrounded == true)
        {
            currentspeed = movespeed;
        }
        //-------------Run---------------


        
        //if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
        //{
        //    input = new Vector3(input.x / 1.3f, 0, input.z / 1.3f);
        //}
        input = Vector3.ClampMagnitude(input,1f);
        //Debug.Log(input);
        input *= currentspeed;
        input = transform.TransformDirection(input);
        if (isgrounded)
        {
            moveDirection = input;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(8 * gravity * jumpheight);
                Debug.Log("Saltou");
            }
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}

