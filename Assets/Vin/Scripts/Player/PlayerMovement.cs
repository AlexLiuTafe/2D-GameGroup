
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Health")]
    public float playerHealth;
    public float playerMaxHealth;
    public Slider playerHealthBar;

    [Header("Component Set Up")]
    private CharacterController2D charC;
    private Rigidbody2D rigid;

    [Header("Movement Variables")]
    public float baseSpeed = 5f;
    public float moveSpeed;
    public static Vector2 motion;
    public static Vector2 direction = new Vector2(0, -1);
    public static bool CanMove = true;

    [Header("Dodge Variables")]
    public float dodgeSpeed;
    public float dodgeTimer, maxDodgeTime, cooldownTimer, dodgeCooldown, preDodgeDelay, afterDodgeDelay;
    private Vector2 dashDirection;
    public static bool isDodging;

    [Header("Animation")]
    private Animator anim;
    private SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        charC = GetComponent<CharacterController2D>();
        rigid = GetComponent<Rigidbody2D>();
        playerHealthBar = gameObject.transform.GetComponentInChildren<Slider>();
        dodgeTimer = maxDodgeTime;
        cooldownTimer = dodgeCooldown;
    }

    void Update()
    {
        if (CanMove)
        {
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");

            motion = new Vector2(inputH, inputV);

            // If we are currently sensing any input
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                // Set our Direction variable as Motion (i.e. the last direction we travelled in based on inputs)
                direction = motion.normalized;
            }

            Debug.DrawRay(transform.position, direction);

            // Multiply Motion by Move Speed
            motion.x *= moveSpeed;
            motion.y *= moveSpeed;

            // Run Dodge() which will Dash if Left Shift is pressed
            Dodge();

            //Move using Character Controller function
            charC.Move(motion * Time.deltaTime);
        }
        playerHealthBar.value = Mathf.Clamp01(playerHealth / playerMaxHealth);
        AnimateMove();
    }

    void Dodge()
    {
        // If Dash is off cooldown
        //if (cooldownTimer > dodgeCooldown)
        //{
            // And we press Left Shift
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDodging)
            {
                dashDirection = direction;
                StartCoroutine(PreDodgeDelay(preDodgeDelay));
            }
        //}

        // Else Dash is currently on cooldown
        //else
        //{
        //    // So count up the cooldown timer
        //    cooldownTimer += Time.deltaTime;
        //}

        // If Dash is active, i.e. our dash timer is on and counting up
        if (dodgeTimer < maxDodgeTime)
        {
            direction = dashDirection;
            // Motion becomes our last faced direction multiplied by our dash speed
            motion = direction * dodgeSpeed;
            // Count up the dash timer by Time.deltaTime
            dodgeTimer += Time.deltaTime;
            // Set IsDashing to true
            isDodging = true;
            // Keep the cooldown timer to 0 so Dash doesn't start cooling down until the Dash is completed
            cooldownTimer = 0;
        }

        else if (dodgeTimer > maxDodgeTime && isDodging)
        {
            motion = Vector2.zero;
            StartCoroutine(AfterDodgeDelay(afterDodgeDelay));
            isDodging = false;
        }
    }

    void AnimateMove()
    {
        if (!(direction.x == 0 && direction.y == 0))
        {
            anim.SetFloat("Horizontal", direction.x);
            anim.SetFloat("Vertical", direction.y);
            anim.SetFloat("Motion", motion.magnitude);

            if (direction.x < 0)
            {
                rend.flipX = true;
            }
            if (direction.x > 0)
            {
                rend.flipX = false;
            }
        }
    }
    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    #region Enumerators
    IEnumerator PreDodgeDelay(float delay)
    {
        CanMove = false;
        motion = Vector2.zero;
        anim.SetTrigger("DodgeRoll");
        yield return new WaitForSeconds(delay);
        CanMove = true;
        // Set our dash timer to 0
        dodgeTimer = 0;
    }

    IEnumerator AfterDodgeDelay(float delay)
    {
        direction = dashDirection;
        CanMove = false;
        yield return new WaitForSeconds(delay);
        CanMove = true;
    }
    #endregion
}