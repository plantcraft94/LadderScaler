using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody2D rb;
    public float movement;
    public float speed;
    bool isGrounded;
    public LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    private bool jumpInput;
    [SerializeField] private float jumpForce = 5f;
    Animator anim;
    public bool JumpLadder;
    float vertical;
    public bool isnearladder = false;
    public bool isonladder = false;
    Collider2D Laddercollision;
    public bool isDead = false;

    [Header("Assist")]
    [SerializeField] private float jumpBufferLength = 0.2f;
    [SerializeField] private float jumpBufferTimer;
    bool jumpBuffer;
    public bool isClimbing = false;
    public bool isChoosingLevels = false;

    [SerializeField] private float cayoteTimeLength = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Dash")]
    TrailRenderer tr;
    public bool hasDash = false;
    public bool canDash = true;
    public bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Climb();
        Animation();
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.6f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer) || (isClimbing == true);
        if (isnearladder && isClimbing)
        {
            isonladder = true;
            rb.gravityScale = 0f;
            transform.position = new Vector2(Laddercollision.transform.position.x, transform.position.y);
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        
        Flip();

        
        if (isGrounded == true)
        {
            canDash = true;
            cayoteTimeLength = 0.1f;
        }
        else
        {
            cayoteTimeLength -= Time.deltaTime;
        }
        Physics2D.IgnoreLayerCollision(3, 7, isDashing);
        if (isDashing)
        {
            return;
        }
        if (isonladder == true && isnearladder == true)
        {
            return;
        }
        isonladder = false;
        if (rb.velocity.y > 0f)
        {
            cayoteTimeLength = 0f;
            rb.gravityScale = gravityScale;
        }
        else if (rb.velocity.y < 0f)
        {
            rb.gravityScale = gravityScale * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -15f));
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);

        // Process jump in the FixedUpdate method
        if (jumpBuffer == true)
        {
            jumpBufferTimer -= Time.deltaTime;
            if (jumpBufferTimer > 0 && (cayoteTimeLength > 0 || (isGrounded && jumpInput)))
            {
                jumpBuffer = false;
                rb.gravityScale = gravityScale;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                // Reset the jump buffer and cayote time states
                jumpInput = false;

            }
            else if (jumpBufferTimer <= 0)
            {
                jumpBuffer = false;
            }
            
        }

        if (jumpBuffer == false)
        {
            jumpBufferTimer = 0;
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        isonladder = false;
        isClimbing = false;
        JumpLadder = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            JumpLadder = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder") && (!JumpLadder))
        {
            isnearladder = true;
            Laddercollision = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            JumpLadder = false;
            isnearladder = false;
        }
    }





    // Input Actions
    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<float>();
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        print("Jump");
        if (ctx.started)
        {
            isonladder = false;
            isClimbing = false;
            JumpLadder = true;
            jumpBuffer = true;
            jumpBufferTimer = jumpBufferLength;
            jumpInput = true;
        }
        if (ctx.canceled)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }
    public void LadderMovement(InputAction.CallbackContext ctx)
    {
        vertical = ctx.ReadValue<float>();
    }
    public void Climb()
    {
        if (Keyboard.current[Key.UpArrow].isPressed || Keyboard.current[Key.DownArrow].isPressed || Keyboard.current[Key.W].isPressed || Keyboard.current[Key.S].isPressed)
        {
            print("Climb");
            if (isnearladder)
            {

                isClimbing = true;
            }
        }
        if (Keyboard.current[Key.UpArrow].wasReleasedThisFrame || Keyboard.current[Key.DownArrow].wasReleasedThisFrame || Keyboard.current[Key.W].wasReleasedThisFrame || Keyboard.current[Key.S].wasReleasedThisFrame)
        {
            if (!isnearladder)
            {

                isClimbing = false;
            }
        }
    }
    public void Dash(InputAction.CallbackContext ctx)
    {
        if (!hasDash)
        {
            return;
        }
        if (ctx.started && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    void Flip()
    {
        if (movement > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (movement < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    void Animation()
    {
        anim.SetBool("isJump", !isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isMove", movement != 0f);
        anim.SetBool("isClimb", isonladder);
        anim.SetBool("Climbing", vertical != 0f);
        anim.SetBool("isDead", isDead);
    }
}
