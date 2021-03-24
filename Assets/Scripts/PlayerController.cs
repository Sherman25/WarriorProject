using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public int maxHealth = 100;
    public int currentHealth;
    public LayerMask enemyLayers;

    private Rigidbody2D rb2d;
    private int count;
    private bool canMove = true;
    private bool facingRight = true;

    Vector2 movement;
    UnityEvent attack_event = new UnityEvent();

    void Start()
    {
        currentHealth = maxHealth;
        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        winText.text = "";
        SetCountText();
        attack_event.AddListener(Attack);
    }

    void Update()
    {
        // Attack trigger
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //movement = new Vector2(0, 0);
            canMove = false;
            attack_event.Invoke();
        }
    }


    void FixedUpdate()
    {
        //rb2d.MovePosition(rb2d.position + movement * Time.fixedDeltaTime);  
    }

    public void Move(float horizontalMove, float verticalMove)
    {
        // Move only if not attacking and not attacked
        if (canMove)
        {
            movement = new Vector2(horizontalMove, verticalMove);
            animator.SetFloat("Horizontal", horizontalMove);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            rb2d.MovePosition(rb2d.position + movement * Time.fixedDeltaTime);
            if (horizontalMove > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontalMove < 0 && facingRight)
            {
                Flip();
            }
        }

    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void MoveAvailable()
    {
        canMove = true;
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count >= 4)
        {
            winText.text = "You win!";
        }
    }

    void Attack()
    {
        // Play an atack animation
        animator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            Debug.Log(enemy.GetComponent<EnemyAI>());
            enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        canMove = false;
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        // Play hearth animation

        // Play fail animation
        if (currentHealth <= 0)
        {
            Fail();
        }
    }

    void Fail()
    {
        canMove = false;
        animator.SetBool("Fail", true);
        Debug.Log("Player fail!");
        animator.SetTrigger("Killed");
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
