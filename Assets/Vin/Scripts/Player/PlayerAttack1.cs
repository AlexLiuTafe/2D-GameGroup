using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PlayerAttack1 : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 20f;
    public float attackRadius = 0.8f;
    public float attackRange = 1.2f;

    [Header("Attack Movement")]
    public float attackMoveSpeed;
    public float attackMoveTime;
    private Vector2 attackMotion;

    [Header("Combo and Delay")]
    public float attackDelay = 0.3f;
    public float endComboDelay = 0.7f;
    private float comboTimer = 0f;
    public float maxComboTime = 0.6f;
    private int attackCounter;
    private int attackComboLimit = 2;

    [Header("Attack Bools")]
    public static bool IsAttacking = false;
    public static bool CanAttack = true;
    public static bool CanBeDamaged = true;
    public bool AttackBuffer = false;

    private Animator anim;
    private CharacterController2D charC;

    private Transform attackPos;
   
    
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    void Start()
    {
        //Get the position of the player
        attackPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
        charC = GetComponentInParent<CharacterController2D>();

        
    }

    void Update()
    {
        //Set position as the player + direction
        transform.position = new Vector2(attackPos.position.x,attackPos.position.y) + PlayerMovement.direction;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            Attack();

        }

        if (IsAttacking)
        {
            charC.Move(attackMotion * Time.deltaTime);
            
        }

        ComboTime();
    }

    void Attack()
    {
        if (!PlayerMovement.isDodging && CanAttack)
        {
            
            PlayerMovement.CanMove = false;
            CanAttack = false;
            StartCoroutine(AttackMove(attackMoveTime));

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)transform.position, attackRadius);

            foreach (var collider in hitColliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }

            if (attackCounter < attackComboLimit)
            {
                StartCoroutine(AttackDelay(attackDelay));
                attackCounter++;
                comboTimer = 0;
            }

            else if (attackCounter >= attackComboLimit)
            {
                StartCoroutine(AttackDelay(endComboDelay));
                attackCounter = 0;
            }
            anim.SetFloat("Horizontal",PlayerMovement.direction.x);
            anim.SetFloat("Vertical", PlayerMovement.direction.y);
            anim.SetTrigger("Attack");
        }
    }

    void ComboTime()
    {
        if (comboTimer < maxComboTime)
        {
            comboTimer += Time.deltaTime;
        }

        else
        {
            attackCounter = 0;
            comboTimer = maxComboTime;
        }
    }

    #region Enumerators
    public IEnumerator AttackMove(float time)
    {
        PlayerMovement.motion = Vector2.zero;
        IsAttacking = true;
        attackMotion = PlayerMovement.direction * attackMoveSpeed;
        yield return new WaitForSeconds(time);
        IsAttacking = false;
    }

    public IEnumerator AttackDelay(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);
        CanAttack = true;
        PlayerMovement.CanMove = true;
    }
    #endregion
}
