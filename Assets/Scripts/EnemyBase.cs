using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Health")]
    public int MaxHealth = 1;
    protected int CurrentHealth;
    protected Animator animator;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {

        Destroy(gameObject, 0.5f);

    }
}
