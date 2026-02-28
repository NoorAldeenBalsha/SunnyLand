using UnityEngine;

public class Bear : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth = 1;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private Transform currentTarget;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
      
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = rightPoint; 
    }

    void FixedUpdate()
    {
        Patrol();
    }

    void Patrol()
    {
        if (leftPoint == null || rightPoint == null) return;

        float dir = Mathf.Sign(currentTarget.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

        Flip(dir);
        animator.SetBool("IsRunning", true);

        
        if (currentTarget == rightPoint && transform.position.x >= rightPoint.position.x)
        {
            currentTarget = leftPoint;
        }
        else if (currentTarget == leftPoint && transform.position.x <= leftPoint.position.x)
        {
            currentTarget = rightPoint;
        }
    }
    void Flip(float dir)
    {
        if (dir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (col != null)
            col.enabled = false;


        if (animator != null)
            animator.SetTrigger("Death");

    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}