using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float runSpeed;
    public int attackDamage = 40;
    public int maxHealth = 100;
    public int currentHealth;
    public LayerMask enemyLayers;
    public AudioManager audioManager;

    private Rigidbody2D rb2d;
    private int count;
    private bool canMove = true;
    private bool facingRight = true;
    private int damageToDeal;
    private int currentDamage = 0;

    private float horizontalMove;
    private float verticalMove;

    Vector2 movement;
    UnityEvent attack_event = new UnityEvent();

    void Start()
    {
        SetDamageToDeal();
        currentHealth = maxHealth;
        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        winText.text = "";
        SetCurrentHealth();
        attack_event.AddListener(Attack);
    }

    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
        verticalMove = Input.GetAxis("Vertical") * runSpeed;
        // Attack trigger
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canMove = false;
            attack_event.Invoke();
        }

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
            SetCurrentHealth();
        }
    }
    void SetCurrentHealth()
    {
        countText.text = "Current Health: " + currentHealth.ToString();
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
            currentDamage += attackDamage;
            if (currentDamage >= damageToDeal)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        canMove = false;
        currentHealth -= damage;
        SetCurrentHealth();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void SetDamageToDeal()
    {
        int index = SceneManager.GetActiveScene().buildIndex / 3;
        damageToDeal = (index + 1) * 100;
        Debug.Log("damageToDeal: " + damageToDeal);
    }
}
