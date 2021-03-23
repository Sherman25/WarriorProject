using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyAI enemyai;
    public int maxHealth = 100;
    private int currentHealth;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    //public Transform attackPoint;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        // Play hearth animation

        // Play fail animation
        if(currentHealth <= 0)
        {
            Fail();
        }
    }

    void Fail()
    {
        Debug.Log("Enemy fail!");
        animator.SetTrigger("Fail");
    }

    void SetFailed()
    {
        animator.SetBool("Failed", true);
    }

    void SetCanMove()
    {
        enemyai.SetCanMove();
    }

 /*   void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }*/
}
