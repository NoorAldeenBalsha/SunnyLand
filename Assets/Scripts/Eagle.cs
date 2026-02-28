using UnityEngine;

public class Eagle : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth = 1;

    private Animator animator;
    private Collider2D col;

    [Header("Movement")]
    public float flySpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    private Transform currentTarget;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>(); 
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        currentTarget = rightPoint;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (leftPoint == null || rightPoint == null) return;

        float dir = Mathf.Sign(currentTarget.position.x - transform.position.x);
        transform.position += new Vector3(dir * flySpeed * Time.deltaTime, 0f, 0f);

        Flip(dir);

        if (currentTarget == rightPoint && transform.position.x >= rightPoint.position.x)
            currentTarget = leftPoint;
        else if (currentTarget == leftPoint && transform.position.x <= leftPoint.position.x)
            currentTarget = rightPoint;
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