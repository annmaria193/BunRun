using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Controls { mobile, pc }

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGroundedBool = false;

    public Animator playeranim;
    public Controls controlmode;

    private float moveX;
    public bool isPaused = false;

    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmissions;

    public ParticleSystem ImpactEffect;
    private bool wasonGround;
    private bool isSliding = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footEmissions = footsteps.emission;

        if (controlmode == Controls.mobile)
        {
            UIManager.instance.EnableMobileControls();
        }
    }

    private void Update()
    {
        isGroundedBool = IsGrounded();

        SetAnimations();

        if (moveX != 0)
        {
            FlipSprite(moveX);
        }

        // Impact effect
        if (!wasonGround && isGroundedBool)
        {
            ImpactEffect.gameObject.SetActive(true);
            ImpactEffect.Stop();
            ImpactEffect.transform.position = new Vector2(footsteps.transform.position.x, footsteps.transform.position.y - 0.2f);
            ImpactEffect.Play();
        }

        wasonGround = isGroundedBool;
    }

    public void SetAnimations()
    {
        if (isSliding)
        {
            playeranim.SetBool("slide", true);
            footEmissions.rateOverTime = 0f;
        }
        else if (moveX != 0 && isGroundedBool)
        {
            playeranim.SetBool("run", true);
            footEmissions.rateOverTime = 35f;
        }
        else
        {
            playeranim.SetBool("run", false);
            playeranim.SetBool("slide", false);
            footEmissions.rateOverTime = 0f;
        }

        playeranim.SetBool("isGrounded", isGroundedBool);
    }

    private void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }

    public void MoveRight()
    {
        moveX = 1f;
        SetAnimations();
    }

    public void MoveLeft()
    {
        moveX = -1f;
        SetAnimations();
    }

    public void StopMoving()
    {
        moveX = 0f;
        isSliding = false; // Reset sliding state when stopping
        SetAnimations();
    }


    public void Jump(float jumpForce)
    {
        if (isGroundedBool)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            playeranim.SetTrigger("jump");
        }
    }

    public void Slide()
    {
        if (isGroundedBool)
        {
            isSliding = true;
            playeranim.SetBool("slide", true);
            moveX = transform.localScale.x > 0 ? 1f : -1f;
            SetAnimations();
        }
    }


    public void StopSlide()
    {
        isSliding = false;
        playeranim.SetBool("slide", false);
    }

    private bool IsGrounded()
    {
        float rayLength = 0.25f;
        Vector2 rayOrigin = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "killzone")
        {
            GameManager.instance.Death();
        }
    }
}