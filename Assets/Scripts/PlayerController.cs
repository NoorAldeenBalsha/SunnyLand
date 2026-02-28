using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator anim;
    public Transform gfx;


    [Header("Health")]
    public int MaxHealth=100;
    public int CurrentHealth;
    public Image healthBar;
    public float smoothSpeed = 10f;
    private float targetFill;
    public Text HealthTXT;


    [Header("Score")]
    int Score;
    public Text ScoreTXT;


    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private int jumpCount = 0;
    private int maxJumpCount = 2;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Climb")]
    public float climbSpeed = 4f;
    private bool isNearLadder;
    private bool isGrounded;
    private bool isClimbing;
    private float moveInput;

    [Header("Death")]
    public float deathJumpForce = 5f;
    public float deathGravityScale = 2f;
    public float deathDestroyTime = 2f;

    private bool isDead = false;
    private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
        CurrentHealth = MaxHealth;
        targetFill = 1f;
        if (healthBar != null)
        {
            healthBar.fillAmount = targetFill;
        }

        Score =0;
        ScoreTXT.text = "Score : " + Score ;
    }
    //====================================================================================================
    // Update is called once per frame
    void Update()
    {
        // Health UI
        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
            HealthTXT.text = CurrentHealth + " %";
        }
        //====================================================================================================
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundRadius,groundLayer);
        anim.SetBool("IsGrounded", isGrounded);
        //====================================================================================================
        // Movement Input
        moveInput = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        if (moveInput != 0) gfx.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        //====================================================================================================
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded &&!isClimbing && jumpCount < maxJumpCount){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetTrigger("Jump");
        }
        //====================================================================================================
        // Crouch
        anim.SetBool("IsCrouch",Input.GetKey(KeyCode.S) && isGrounded && !isClimbing);
        //====================================================================================================
        // Ladder Logic
        vertical = Input.GetAxisRaw("Vertical");
        if (isNearLadder)
        {
            if (vertical > 0 && !isClimbing)
            {
                isClimbing = true;
                rb.gravityScale = 0;
                anim.SetBool("IsClimb", true);
            }
            if (isClimbing)
            {
                rb.linearVelocity = new Vector2( moveInput * moveSpeed, vertical * climbSpeed);
                if (vertical == 0)
                {
                    rb.linearVelocity = new Vector2(moveInput * moveSpeed,0f );
                }
            }
        }
        //====================================================================================================
        //Death Logic
        if(isDead) return;
        //====================================================================================================
        // Escape to Setting Scene
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(LoadAfterDelay("Setting"));
        }
    }
    //====================================================================================================
    // This one for Movement
    void FixedUpdate()
    {
        if (!isClimbing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
        }
    }
    //====================================================================================================
    //This one for Start Climb for Ladder
    public void StartClimb()
    {
        isClimbing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsClimb", true);
    }
    //====================================================================================================
    //This one for Stop Climb for Ladder
    public void StopClimb()
    {
        isClimbing = false;
        rb.gravityScale = 1;
        anim.SetBool("IsClimb", false);
    }
    //====================================================================================================
    // This one for Die
    void Die()
    {
        isDead = true;
        anim.SetTrigger("Hurt");
        rb.linearVelocity = new Vector2(0f, deathJumpForce);
        rb.gravityScale = 0;
        col.enabled = false;
        StartCoroutine(DeathRoutin());
    }
    //====================================================================================================
    // This one for Take Damage
    public void TakeDamage(int damge)
    {
        CurrentHealth -= damge;
        CurrentHealth=Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        targetFill = (float)CurrentHealth / MaxHealth;
        anim.SetTrigger("Hurt");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    //====================================================================================================
    //This one for OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Health"))
        {
            if (CurrentHealth < 100)
            {
                CurrentHealth += 10;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
                targetFill = (float)CurrentHealth / MaxHealth;

            }
            
            Destroy(collision.gameObject);
        } else if (collision.CompareTag("Score"))
            {
                Score += 10;
            ScoreTXT.text = "Score : " + Score;
            Destroy(collision.gameObject);
            }

        if (collision.CompareTag("Climb"))
        {
           isNearLadder = true;
        }

        Eagle eagle = collision.GetComponent<Eagle>();
        if(eagle == null) return;
        
        if(rb.linearVelocity.y < 0 && transform.position.y>collision.transform.position.y+0.8f) 
        {
            Score += 10;
            ScoreTXT.text = "Score : " + Score;
            eagle.TakeDamage(1);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 2f); 
        }
        else
        {
            TakeDamage(10);
        }

    }
    //====================================================================================================
    // This one for OnTriggerExit2D
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            isNearLadder = false;
            StopClimb();
        }
    }
    //====================================================================================================
    // This one for OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            return; // ãåã ÌÏÇð
        }

        Bear bear = collision.gameObject.GetComponent<Bear>();
        Bunny bunny = collision.gameObject.GetComponent<Bunny>();

        if (bear == null && bunny == null)
            return; // áíÓ ÚÏæ

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.8f) // ÝæÞ ÇáÚÏæ
            {
                Score += 10;
                ScoreTXT.text = "Score : " + Score;

                if (bear != null) bear.TakeDamage(1);
                if (bunny != null) bunny.TakeDamage(1);

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
                break;
            }
            else
            {
                TakeDamage(10);
            }
        }
    }
    //====================================================================================================
    // This method is load scene after delay for 0.5 second
    IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    System.Collections.IEnumerator DeathRoutin()
    {
        yield return new WaitForSeconds(0.5f);
        gfx.rotation=Quaternion.Euler(0f, 0f, 180);
        rb.gravityScale= deathGravityScale;
        rb.linearVelocity = new Vector2(0f, deathJumpForce);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LoadAfterDelay("List"));
    }
}

