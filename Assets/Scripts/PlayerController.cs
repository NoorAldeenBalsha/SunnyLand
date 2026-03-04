using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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
    public float smoothSpeedHealth = 10f;
    private float targetFillHealth;
    public Text HealthTXT;


    [Header("Score")]
    public int MaxScore = 100;
    public int CurrentScore;
    public Image ScoreBar;
    public float smoothSpeedScore = 10f;
    public Text ScoreTXT;
    private float targetFillScore;


    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private int jumpCount = 0;
    private int maxJumpCount = 2;
    private float verticalInput;


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
        targetFillHealth = 1f;
        if (healthBar != null)
        {
            healthBar.fillAmount = targetFillHealth;
        }
        targetFillScore = 0f;
        if (ScoreBar != null)
        {
            ScoreBar.fillAmount = targetFillScore;
        }
    }
    //====================================================================================================
    // Update is called once per frame
    void Update()

    {   // Health UI
        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFillHealth, Time.deltaTime * smoothSpeedHealth);
            HealthTXT.text = CurrentHealth + " %";
        }
        //====================================================================================================
        // Score UI
        if (ScoreBar != null)
        {
            ScoreBar.fillAmount = Mathf.Lerp(ScoreBar.fillAmount, targetFillScore, Time.deltaTime * smoothSpeedScore);
            ScoreTXT.text = CurrentScore + " %";
        }
        //====================================================================================================
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundRadius,groundLayer);
        anim.SetBool("IsGrounded", isGrounded);
        if (isGrounded && isClimbing) StopClimb();
        //====================================================================================================
        // Movement Input
        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        if (moveInput != 0) gfx.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        //====================================================================================================
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded &&!isClimbing && jumpCount < maxJumpCount){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSound);
            anim.SetTrigger("Jump");
        }
        //====================================================================================================
        // Crouch
        anim.SetBool("IsCrouch",Input.GetKey(KeyCode.S) && isGrounded && !isClimbing);
        //====================================================================================================
        // Ladder Logic
        if (isNearLadder && Mathf.Abs(verticalInput)>0 && !isClimbing)
        {
                isClimbing = true;
                rb.gravityScale = 0;
                anim.SetBool("IsClimb", true);
  
        }
        //====================================================================================================
        //Death Logic
        if (isDead) return;
        //====================================================================================================
        //Score Check
        if (CurrentScore >= MaxScore)
        {
         StartCoroutine(   LoadAfterDelay("List"));
        }
    }
    //====================================================================================================
    // This one for Movement
    void FixedUpdate()
    {

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2( moveInput * moveSpeed,  verticalInput * climbSpeed );

            if (Mathf.Abs(verticalInput) > 0)
                anim.speed = 1f;
            else
                anim.speed = 0f;

            if (!isNearLadder || (isGrounded && verticalInput <= 0))
            {
                StopClimb();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2( moveInput * moveSpeed, rb.linearVelocity.y );
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
        targetFillHealth = (float)CurrentHealth / MaxHealth;
        anim.SetTrigger("Hurt");
        if (CurrentHealth <= 0)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSound);
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
                AudioManager.Instance.PlaySFX(AudioManager.Instance.healthPickupSound);
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
                targetFillHealth = (float)CurrentHealth / MaxHealth;

            }
            
            Destroy(collision.gameObject);
        } else if (collision.CompareTag("Score"))
            {
            CurrentScore += 10;
            CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
            targetFillScore = (float)CurrentScore / MaxScore;
            ScoreTXT.text = CurrentScore + " %";
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gemSound);
            Destroy(collision.gameObject);
            }

        if (collision.CompareTag("Climb"))
        {
           isNearLadder = true;
        }

        Eagle eagle = collision.GetComponent<Eagle>();
        Bat bat = collision.GetComponent<Bat>();
        Battle battle = collision.GetComponent<Battle>();
        if (eagle == null && bat==null && battle==null) return;
        
        if(rb.linearVelocity.y < 0 && transform.position.y>collision.transform.position.y+0.8f) 
        {
            CurrentScore += 10;
            CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
            targetFillScore = (float)CurrentScore / MaxScore;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSound);
            if(bat!=null) bat.TakeDamage(1);
            if (eagle != null) eagle.TakeDamage(1);
            if (battle != null) battle.TakeDamage(1);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 2f); 
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHitSound);
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
            return; 
        }

        Bear bear = collision.gameObject.GetComponent<Bear>();
        Bunny bunny = collision.gameObject.GetComponent<Bunny>();
        Dog dog = collision.gameObject.GetComponent<Dog>();
        Dino dino = collision.gameObject.GetComponent<Dino>();
        Slimer slimer = collision.gameObject.GetComponent<Slimer>();

        if (bear == null && bunny == null && dog == null && dino == null && slimer == null)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.8f)
            {
                
                AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSound);
                if (bear != null)
                {
                    bear.TakeDamage(1);
                    CurrentScore += 20;
                    CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
                    targetFillScore = (float)CurrentScore / MaxScore;
                    ScoreTXT.text = CurrentScore + " %";
                }
                if (bunny != null)
                {
                    bunny.TakeDamage(1);
                    CurrentScore += 20;
                    CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
                    targetFillScore = (float)CurrentScore / MaxScore;
                    ScoreTXT.text = CurrentScore + " %";
                }
                if (dino != null)
                {
                    dino.TakeDamage(1);
                    CurrentScore += 15;
                    CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
                    targetFillScore = (float)CurrentScore / MaxScore;
                    ScoreTXT.text = CurrentScore + " %";
                }
                if (dog != null)
                {
                    dog.TakeDamage(1);
                    CurrentScore += 15;
                    CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
                    targetFillScore = (float)CurrentScore / MaxScore;
                    ScoreTXT.text = CurrentScore + " %";
                }
                if (slimer != null)
                {
                    slimer.TakeDamage(1);
                    CurrentScore += 15;
                    CurrentScore = Mathf.Clamp(CurrentScore, 0, MaxScore);
                    targetFillScore = (float)CurrentScore / MaxScore;
                    ScoreTXT.text = CurrentScore + " %";
                }

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
                break;
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSound);
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

