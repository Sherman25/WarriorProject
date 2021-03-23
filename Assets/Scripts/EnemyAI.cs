using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemy;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator animator;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float attackRange = 0.5f;

    public int attackDamage = 40;
    
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    bool canMove = true;

    Seeker seeker;
    Rigidbody2D rb;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone()) 
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (canMove)
        {
            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            animator.SetFloat("Force", Mathf.Abs(force.x));
            if (force.x > 0)
            {
                enemy.localScale = new Vector3(5f, 5f, 1f);
            }
            else if (force.x < 0)
            {
                enemy.localScale = new Vector3(-5f, 5f, 1f);
            }
        }
        

        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);
        if (hitEnemy != null)
        {
            canMove = false;
            Attack(hitEnemy);
        }
    }

    void Attack(Collider2D hitEnemy)
    {
        // Play an atack animation
        animator.SetFloat("Force", Mathf.Abs(0));
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
        // Detect enemy in range of attack
        //Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);

        // Damage
        Debug.Log("We hit " + hitEnemy.name);
        //hitEnemy.GetComponent<Enemy>().TakeDamage(attackDamage);
    }

    void CanMove()
    {
        canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
