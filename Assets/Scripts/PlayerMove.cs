using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;

    bool isJumping;
    public GameManager gameManager; // Reference to the GameManager script
    public float jumpForce; // Force applied when jumping
    public float maxSpeed;
    private Vector2 moveInputVector;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        isJumping = false;
    }
    public void OnMove(InputValue value)
    {
        moveInputVector = value.Get<Vector2>();
    }
    public void OnJump()
    {
        if (!isJumping)
        {
            isJumping = true; // Set jumping state to true
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Add upward force for jump
            animator.SetBool("isJumping", true); // Trigger jump animation
        }
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.normalized.x * 0.5f, rb.linearVelocity.y); // Stop horizontal movement when input is canceled
    }
    void Update()
    {
        if (moveInputVector.x != 0)
        {
            spriteRenderer.flipX = moveInputVector.x == -1; // Flip sprite based on movement direction
        }

        if (moveInputVector.x == 0)
        {
            animator.SetBool("isWalking", false); // Stop running animation
        }
        else
        {
            animator.SetBool("isWalking", true); // Start running animation
        }
    }
    void FixedUpdate()
    {
        float moveHorizontal = moveInputVector.x;
        rb.AddForce(Vector2.right * moveHorizontal, ForceMode2D.Impulse);

        if (rb.linearVelocity.x > maxSpeed) // speed to right side
        {
            rb.linearVelocity = new Vector2(maxSpeed, rb.linearVelocity.y);
        }
        else if (rb.linearVelocity.x < -maxSpeed) // speed to left side
        {
            rb.linearVelocity = new Vector2(-maxSpeed, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false; // Reset jumping state when colliding with the floor
            animator.SetBool("isJumping", false); // Stop jump animation
        }

        if (collision.gameObject.CompareTag("enemy"))
        {
            if (rb.linearVelocity.y < 0 && transform.position.y > collision.contacts[0].point.y && collision.gameObject.name != "spike") // Check if player is falling
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.contacts[0].point); // Call method to handle player damage
            }
        }
    }

    void OnDamaged(Vector2 damageSourcePosition)
    {
        gameManager.HealthDown(); // Decrease player's health when damaged

        gameObject.layer = 8; // Change player layer to "PlayerDamaged"

        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // Change player color to red to indicate damage

        int direction = transform.position.x - damageSourcePosition.x > 0 ? 1 : -1; // Determine direction based on damage source position
        rb.AddForce(new Vector2(direction, 1) * 7, ForceMode2D.Impulse); // Apply force to the player when damaged

        animator.SetTrigger("Damaged"); // Trigger damage animation

        Invoke("OffDamaged", 2); // Call OffDamaged method after 2 seconds to reset player state
    }

    void OffDamaged()
    {
        gameObject.layer = 7; // Change player layer back to "Player"
        spriteRenderer.color = new Color(1, 1, 1, 1); // Reset player color to normal
    }

    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100; // Increase total score by 100 when attacking an enemy

        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();

        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // Apply upward force to the player when attacking an enemy

        enemyMove.OnDamaged(); // Call OnDamaged method on the enemy to handle enemy damage
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            bool isBronze = collision.gameObject.name.Contains("bronze");
            bool isSilver = collision.gameObject.name.Contains("silver");

            if (isBronze)
            {
                gameManager.stagePoint += 50; // Add 50 points for bronze item
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100; // Add 100 points for silver item
            }
            else
            {
                gameManager.stagePoint += 300; // Add 300 points for other items
            }

            gameManager.stagePoint += 100;

            collision.gameObject.SetActive(false); // Deactivate the item when player collides with it
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            gameManager.NextStage(); // Call NextStage method in GameManager when player reaches the finish line
        }
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.3f); // Change color to red when damaged
        spriteRenderer.flipY = true; // Flip sprite to indicate damage
        capsuleCollider.enabled = false; // Disable collider to prevent further interactions
        rb.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); // Apply upward force when damaged
    }

    public void VelocityZero()
    {
        rb.linearVelocity = Vector2.zero; // Reset player velocity to zero
    }

}
