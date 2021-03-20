using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
    }
}
