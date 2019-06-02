using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TopDownMovement : MonoBehaviour
{
    [Header("Move Direction")]
    public Vector3 moveDir;
    public Vector3 tempDir;
    public CharacterController charC;

    [Header("Character Variables")]
    public float maxSpeed;
    public float speed, dashSpeed, acceleration;

    // Start is called before the first frame update
    void Start()
    {
        charC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Move();
        
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
    }

    void Move()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
                moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (moveDir.magnitude > 0)
                {
                    tempDir = moveDir;
                }
                
                moveDir *= speed * Time.deltaTime;
                charC.Move(moveDir);
            }

            else
            {
                moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                moveDir *= speed * Time.deltaTime;
                charC.Move(moveDir);
            } 
        }

        else
        {
            speed = 0;
        }
    }

    void Dash()
    {
        //charC.Move(tempDir * dashSpeed);
        Vector3 dashPos = transform.position + tempDir * dashSpeed;
        transform.position = Vector3.Lerp(transform.position, dashPos, 0.7f*Time.deltaTime);
        print(tempDir.magnitude);
    }
}
