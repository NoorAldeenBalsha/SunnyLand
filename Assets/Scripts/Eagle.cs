using UnityEngine;

public class Eagle : EnemyBase
{
    public float flySpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    
    private Transform currentTarget;

    void Start()
    {
        currentTarget = rightPoint;
    }

    void Update()
    {
        Patrol();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    { 
    
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


}