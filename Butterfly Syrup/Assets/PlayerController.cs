using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;

    [Space]

    [Header("Parkour")]
    public float slideSpeed;
    public float wallJumpLerp;
    public float wallJumpForce;
    public float wallJumpHeight;

    [Space]

    [Header("Booleans")]
    public bool wallJumped;
    public bool canMove;
    public bool canJump;

    private Rigidbody2D rb;
    CollisionState coll;
    Animator anim;

    void Start() // Some things are commented out as they will be reimplemented once we reach icicle claws
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CollisionState>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal"); //Project Settings Defined
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if ((Input.GetKeyDown(KeyCode.Space)) && canJump == true)
        {
            /*if (coll.onWall && !coll.onGround)
            {
                WallJump();
            }*/
            /*else if (coll.onGround && coll.onWall)
            {
                Jump(Vector2.up, false);
                StartCoroutine(DisableWallJump(1f));
                StopCoroutine(DisableWallJump(0f));
            }*/
            if (coll.onGround)
            {
                Jump(Vector2.up, false);
            }
        }

        /*if(coll.onWall && !coll.onGround)
        {
            WallSlide();
        }*/

        Vector3 curVelocity = rb.velocity;

        if (Input.GetKey(KeyCode.K))
        {
            Debug.Log(curVelocity);
        }

        Flip();

        if(rb.velocity.x > 1 || rb.velocity.x < -1)
        {
            anim.SetFloat("Speed", 2f);
        } else
        {
            anim.SetFloat("Speed", 0f);
        }
        
    }


    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;
        if (!wallJumped)
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        else
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
    }

    private void WallJump() // We can keep this, just don't call the method.
    {
        StartCoroutine(DisableMovement(.15f));

        rb.velocity = new Vector2(0f, 0f);

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        if (coll.onRightWall)
        {
            // Unity is weird, there's no transform.left, so you must do negative transform.right
            rb.AddForce(-transform.right * wallJumpForce, ForceMode2D.Impulse);
            //jumpWall((Vector2.left / 3f + wallDir / 1.75f), true);
        }
        else if (coll.onLeftWall)
        {
            rb.AddForce(transform.right * wallJumpForce, ForceMode2D.Impulse);
        }
        rb.AddForce(transform.up * wallJumpHeight, ForceMode2D.Impulse);
        StopCoroutine(DisableMovement(0f));
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        wallJumped = true;
        yield return new WaitForSeconds(time);
        canMove = true;
        wallJumped = false;
    }

    IEnumerator DisableWallJump(float time) // Not using Wall Jump
    {
        wallJumped = true;
        yield return new WaitForSeconds(time);
        wallJumped = false;
    }

    private void Jump(Vector2 dir, bool wall) //Using Jump.cs
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    private void jumpWall(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * wallJumpForce;
    }

    private void WallSlide()
    {
        if (wallJumped == false)
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);

        if (!canMove)
            return;
    }

    void Flip()
    {
        if (rb.velocity.x > .5f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (rb.velocity.x < -.5f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}