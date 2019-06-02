using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SideScrollerMovement : MonoBehaviour
{
    [Header("Set Up")]
    public CharacterController charC;

    [Header("Basic Movement Variables")]
    public float baseSpeed;
    public float jumpHeight;

    [Header("Dash Variables")]
    public float dashSpeed;
    public float currentDashTime;
    public float maxDashTime;
    public float dashCooldown;
    public float cooldownTimer;

    [Header("Movement")]
    public Vector2 moveDir;
    public float gravity;
    public float groundRayDistance = 1.1f;
    public bool FaceLeft;

    // Start is called before the first frame update
    void Start()
    {
        currentDashTime = maxDashTime;
        cooldownTimer = dashCooldown;
        charC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    bool IsGrounded()
    {
        Ray groundRay = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(groundRay, out hit, groundRayDistance))
        {
            return true;
        }
        return false;
    }

    void Move()
    {

        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.localEulerAngles = new Vector2(transform.localEulerAngles.x, 180);
            FaceLeft = true;
        }

        else if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localEulerAngles = new Vector2(transform.localEulerAngles.x, 0);
            FaceLeft = false;
        }

        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.x *= baseSpeed;
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y = jumpHeight;
            }
        }
        moveDir.y -= gravity * Time.deltaTime;
        Dash();
        charC.Move(moveDir*Time.deltaTime);
    }

    void Dash ()
    {
        if (cooldownTimer > dashCooldown)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                currentDashTime = 0f;
            }
        }

        else
        {
            cooldownTimer += Time.deltaTime;
        }

        if (currentDashTime < maxDashTime)
        {
            if (FaceLeft)
            {
                moveDir.x = -dashSpeed;
            }
            else
            {
                moveDir.x = dashSpeed;
            }
            moveDir.y = 0;
            moveDir.y += gravity * Time.deltaTime;
            currentDashTime += Time.deltaTime;
            cooldownTimer = 0f;
        }

    }
}
