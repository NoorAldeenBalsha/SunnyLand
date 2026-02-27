using UnityEngine;

public class Bear : EnemyBase
{
    public float patrolSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    private Rigidbody2D rb;
    private Animator anim;

    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

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
        anim.SetBool("IsRunning", true);

        
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
}