// ============================================================
// Player.cs (Updated — integrasi PlayerAudio)
//
// Perubahan dari versi sebelumnya:
//   - Tambah referensi PlayerAudio di Awake
//   - PlayJump() dipanggil saat jump pertama
//   - PlayRoll() dipanggil saat air roll / double jump
//   - SetGrounded() dipanggil di LateUpdate agar footstep
//     tahu kapan Bera di tanah
//
// Semua logika gameplay tidak berubah.
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float airControl = 0.5f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Air Roll / Dash")]
    public float dashForceX = 9f;
    public float dashForceY = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Death")]
    public float fallLimit = -10f;
    public GameOverManager gameOverManager;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerAudio playerAudio; // ← referensi audio

    private float moveInput;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isDead    = false;
    private bool isRolling = false;

    void Start()
    {
        rb        = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();

        // Cache PlayerAudio — boleh null jika script belum dipasang
        playerAudio = GetComponent<PlayerAudio>();

        if (groundCheck == null)
            Debug.LogError("GroundCheck belum diisi!");
    }

    void Update()
    {
        if (isDead) return;

        moveInput = Input.GetAxis("Horizontal");

        // =========================
        // ANIMATOR
        // =========================

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        animator.SetBool(
            "IsJumping",
            !isGrounded && rb.linearVelocity.y > 0.1f
        );

        animator.SetBool(
            "IsFalling",
            rb.linearVelocity.y < -0.1f
        );

        // =========================
        // FLIP KARAKTER
        // =========================

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // =========================
        // JUMP + AIR ROLL
        // =========================

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Jump pertama dari tanah
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                // ← Suara jump
                playerAudio?.PlayJump();
            }

            // Double jump = air roll
            else if (canDoubleJump)
            {
                canDoubleJump = false;
                isRolling     = true;

                animator.SetTrigger("Roll");

                rb.gravityScale = 1f;

                float dashDirection = transform.localScale.x;

                rb.linearVelocity = new Vector2(
                    dashDirection * dashForceX,
                    dashForceY
                );

                // ← Suara air roll
                playerAudio?.PlayRoll();

                Invoke("StopRoll", 0.35f);
            }
        }

        // =========================
        // CEK JATUH
        // =========================

        if (transform.position.y < fallLimit)
            Die();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isRolling) return;

        float control = isGrounded ? 1f : airControl;

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed * control,
            rb.linearVelocity.y
        );

        // =========================
        // GRAVITY TAMBAHAN
        // =========================

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up *
                                 Physics2D.gravity.y *
                                 (fallMultiplier - 1) *
                                 Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up *
                                 Physics2D.gravity.y *
                                 (lowJumpMultiplier - 1) *
                                 Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            checkRadius,
            groundLayer
        );

        // Reset double jump saat landing
        if (isGrounded)
        {
            canDoubleJump   = true;
            rb.gravityScale = 3f;
        }

        // ← Beritahu PlayerAudio status grounded
        // agar footstep tahu kapan harus berbunyi
        playerAudio?.SetGrounded(isGrounded);
    }

    void StopRoll()
    {
        isRolling       = false;
        rb.gravityScale = 3f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("LEVEL SELESAI!");
            rb.linearVelocity = Vector2.zero;
            this.enabled      = false;
        }
    }

    void Die()
    {
        Debug.Log("PLAYER MATI");
        isDead            = true;
        rb.linearVelocity = Vector2.zero;
        gameOverManager.GameOver();
    }

    void Restart()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}