using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float enemyHealth;
    public float enemyMaxHealth;


    public Transform player;
    private Vector2 direction;
    

    [Header("Enemy Health Bar")]
    public Slider enemyHealthBar;

    [Header("Enemy Speed")]
    private float speed;
    public float runSpeed;
    public float retreatSpeed;
    public float stopDistance;
    public float retreatDistance;

    [Header("Attack")]
    public float damage;
    private float shootCooldown;
    public float shootStartTimer;
    public GameObject[] projectiles;

    public Animator anim;


    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        enemyHealthBar = gameObject.transform.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        //EnemyHealth
        enemyHealthBar.value = Mathf.Clamp01(enemyHealth / enemyMaxHealth);

        if (Vector2.Distance(transform.position, player.position) > stopDistance)
        {
            Chase();

        }
        //Checking if Bear pos reach the stopDistance with player pos and retreatDistance is > then the bear will stop moving
        else if (Vector2.Distance(transform.position, player.position) < stopDistance && Vector2.Distance(transform.position, player.position) > retreatDistance)
        {
            transform.position = this.transform.position;
        }
        //if retreatDistance is < between player and bear position
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            Retreat();
        }
        if (shootCooldown <= 0)
        {
            Shoot();
        }
        else
        {
            shootCooldown -= Time.deltaTime;
        }
        EnemyAnimator();

    }
    void Chase()
    {
        speed = runSpeed;
        direction = (player.transform.position - transform.position).normalized; //storing facing direction to target.
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        
    }
    void Retreat()
    {
        speed = retreatSpeed;
        transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        
    }
    void Shoot()
    {
        int random = Random.Range(0, projectiles.Length);
        Instantiate(projectiles[random], transform.position, Quaternion.identity);
        shootCooldown = shootStartTimer;
    }
    void EnemyAnimator()
    {
        
        if (direction!= Vector2.zero)
        {
            
            anim.SetFloat("Horizontal", direction.x);
            anim.SetFloat("Vertical", direction.y);
            //anim.SetFloat("Magnitude",direction.magnitude); Still Not Working for Idle Animations

        }
        
    }
}
