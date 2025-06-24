using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public float maxSpeed;
    private Vector2 moveInputVector;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public void OnMove(InputValue value)
    {
        moveInputVector = value.Get<Vector2>(); 
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
}
