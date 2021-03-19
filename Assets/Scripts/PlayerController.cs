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
    

    private Rigidbody2D rb2d;
    private int count;

    Vector2 movement;
    UnityEvent attack_event = new UnityEvent();

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        count = 0;
        winText.text = "";
        SetCountText();
        attack_event.AddListener(Attack);
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed;
        movement = new Vector2(moveHorizontal, moveVertical);
        animator.SetFloat("Horizontal", moveHorizontal);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Attack trigger
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement = new Vector2(0, 0);
            attack_event.Invoke();
        }
    }


    void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + movement * Time.fixedDeltaTime);
        
        
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
        animator.SetTrigger("Attack");
    }
}
