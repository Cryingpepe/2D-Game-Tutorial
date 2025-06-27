using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public int nextMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(nextMove, rb.linearVelocity.y); // Move enemy to the left

        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.2f, rb.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
           Turn(); // If no platform below, turn around
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);

        animator.SetInteger("WalkSpeed", nextMove); // Set walking speed for animation

        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1; // Flip sprite based on movement direction
        }

        float nextTime = Random.Range(2, 5);
        Invoke("Think", nextTime);
    }

    void Turn()
    { 
        nextMove *= -1; // Reverse direction if no platform below
        spriteRenderer.flipX = nextMove == 1; // Flip sprite based on movement direction

        CancelInvoke();
        Invoke("Think", 2); // Recalculate next move after reversing direction
    }
}
