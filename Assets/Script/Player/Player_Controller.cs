using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private bool player = true;
    
    //Component trong object
    private Rigidbody2D rb;
    private Animator amt;

    //Cac bien
    [Header("Jump")]
    [SerializeField] private float JumpForce = 15f;
    private bool CanJump;
    [SerializeField] private int amountJump = 2;
    [SerializeField] private int jumpleft;
    [SerializeField] GameObject DoubleJumpEffect;
    [SerializeField] private float jumpvar;


    [Header("Run")]
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] private float InputDirect;
    private bool IsFacingRight = true;
    private bool IsRun;
    private bool CanFlip = true;


    [Header("Ground check")]
    private bool IsGrounded;
    public Transform GroundCheck;
    public float GroundCheckRadius;
    public LayerMask WhatisGround;

    [Header("Wall check")]
    [SerializeField] private float WallCheckDistance;
    private bool IsTouchWall;
    public Transform WallCheck;
    private bool IsWallSlide;
    [SerializeField] private float WallSildeSpeed;

    [Header("Air")]
    [SerializeField] private float Airforce;
    [SerializeField] private float AirDrag;

    [Header("Wall jump")]
    [SerializeField] private Vector2 WallJump;
    [SerializeField] private float WallJumpForce;
    private int FacingDirect = 1;
    private float VerticalDirect;

    [Header("Dash")]
    [SerializeField] private float DashTime;
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashCD;
    private float DashTimeLeft;
    private float LastDash = -100f;
    private bool IsDash;
    [SerializeField] GameObject DashEffect;
    [SerializeField] private int AmountDash = 2;
    [SerializeField] private int DashLeft;
    private bool AirDash = false;
    private float LastDashMulti;
    private bool SupLastDash = false;
    private float SupLastDashTime;

    [Header("Combat")] 
    [SerializeField] private bool Knockback;
    private float KnockbackStartTime;
    [SerializeField] private float KnockbackDuration;
    [SerializeField] private Vector2 KnockbackSpeed;
    private PlayerStat ps;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amt = GetComponent<Animator>();
        ps = GetComponent<PlayerStat>();
        jumpleft = amountJump;
        DashLeft = AmountDash;
        WallJump.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        if(!player) return;
        
        CheckCanJump();
        CheckInput();
        CheckFacing();
        AnimationControl();
        CheckWallSilde();
        SupDash();
        CheckDash();
        checkKnockback();
    }
    private void FixedUpdate()
    {
        if(!player) return;

        Movement();
        CheckSurrounding();
    }

    public Animator GetAmt()
    {
        return amt;
    }

    public PlayerStat GetPs()
    {
        return ps;
    }

    public void SetPlayer(bool Player)
    {
        player = Player;
    }
    public void Setknoback(bool KnockBack)
    {
        Knockback = KnockBack;
    }

    public Rigidbody2D GetRb()
    {
        return rb;
    }
    
    
    private void CheckSurrounding()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, WhatisGround);
        if (IsGrounded )
        {
            AirDash = false;
        }
        IsTouchWall = Physics2D.Raycast(WallCheck.position, transform.right, WallCheckDistance, WhatisGround);
    }
    private void CheckFacing()
    {
        if (IsDash) return;
        if (IsFacingRight && InputDirect < 0)
        {
            Facing();
        }
        else if(!IsFacingRight && InputDirect > 0)
        {
            Facing();
        }

        if(rb.velocity.x != 0)
        {
            IsRun = true;
        }
        else
        {
            IsRun = false;
        }
    }

    
    
    private void CheckCanJump()
    {
        if ((IsGrounded && rb.velocity.y <= 0) || IsWallSlide)
        {
            jumpleft = amountJump;
            amt.SetBool("IsDoubleJump", false);
        }

        if(jumpleft <= 0)
        {
            CanJump = false;
            amt.SetBool("IsDoubleJump", true);
        }
        else
        {
            CanJump = true;
            amt.SetBool("IsDoubleJump", false);
        }
    }
    private void CheckWallSilde()
    {
        if(IsTouchWall && !IsGrounded && rb.velocity.y < 0.01f)
        {
            IsWallSlide = true;
        }
        else {
            IsWallSlide = false; 
        }
    }

    public void StartAni()
    {
        CanFlip = false;
    }
    public void EndAni()
    {
        CanFlip = true;
    }

    private void Facing()
    {
        if(!IsWallSlide && CanFlip && !Knockback)
        {
            FacingDirect *= -1; 

            IsFacingRight = !IsFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
    public int GetFacingDirect()
    {
        return FacingDirect;
    }

    private void CheckInput()
    {
        InputDirect = Input.GetAxisRaw("Horizontal");
        VerticalDirect = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpvar);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (LastDash + DashCD) && !IsWallSlide && IsGrounded)
            {
                if(DashLeft <= 0 || LastDashMulti == LastDash)
                {
                    DashLeft = AmountDash;
                }

                AttempToDash();
            }
            else if((Time.time >= (LastDash + DashCD)) && !IsWallSlide && !IsGrounded && !AirDash)
            {
                if (DashLeft <= 0)
                {
                    DashLeft = AmountDash - 1;
                } 
                AirDash = true;
                AttempToDash();
            }  
        }
    }
    private void SupDash(){
        if (!SupLastDash) { return; }

        if(SupLastDashTime <= 0)
        {
            LastDash = LastDashMulti;
            SupLastDash = false;
        }
        else
        {
            SupLastDashTime -= Time.deltaTime;
        }
    }
    private void AttempToDash()
    {
        if(DashLeft == 1)
        {
            LastDash = Time.time;
            SupLastDash = false;
        }
        else if(DashLeft > 1)
        {
            SupLastDash = true;
            SupLastDashTime = DashCD;
            LastDashMulti = Time.time;
        }

        IsDash = true;
        DashTimeLeft = DashTime;
    }
    
    private void CheckDash()
    {
        if (IsDash)
        {
            if(DashTimeLeft > 0)
            {
                rb.velocity = new Vector2(DashSpeed * FacingDirect, 0);
                amt.SetBool("IsDash", IsDash);
                if(DashTimeLeft == DashTime)
                {
                    Instantiate(DashEffect, transform);
                    DashLeft--;
                }
                DashTimeLeft -= Time.deltaTime;
            }

            if(DashTimeLeft <= 0 || IsTouchWall)
            {
                IsDash = false;
                amt.SetBool("IsDash", IsDash);
            }
        }
    }
    

    private void Jump()
    {
        if (IsDash && IsGrounded) return;
        if(CanJump && !IsWallSlide)
        {
            IsDash = false;
            amt.SetBool("IsDash", IsDash);
            rb.velocity = new Vector2 (rb.velocity.x, JumpForce);
            jumpleft -= 1;

            LastDash = -100f;
            AirDash = false;

            if (jumpleft == 0)
            {
                Instantiate(DoubleJumpEffect, transform);
            }
            
        }
        else if (IsWallSlide && InputDirect == 0 && CanJump)
        {
            IsWallSlide = false;
            jumpleft--;
            Vector2 ForceAdd_ = new Vector2(2f * -FacingDirect, 5f);
            rb.AddForce(ForceAdd_, ForceMode2D.Impulse);
        }
        
        else if((IsWallSlide || IsTouchWall) && InputDirect != 0 && CanJump)
        {
            if(FacingDirect != InputDirect)
            {
                IsWallSlide = false;
                jumpleft--;
                Vector2 ForceAdd_ = new Vector2(WallJumpForce * WallJump.x * InputDirect, WallJumpForce * WallJump.y);
                rb.AddForce(ForceAdd_, ForceMode2D.Impulse);
            }
        }
    }
    private void Movement()
    {
        if (IsDash) return;
        if (IsGrounded && !Knockback)
        {
            rb.velocity = new Vector2 (MovementSpeed * InputDirect, rb.velocity.y);
        }
        else if(!IsGrounded && !IsWallSlide && InputDirect != 0f && !Knockback)
        {
            Vector2 forceAdd_ = new Vector2(Airforce * InputDirect, 0);

            rb.AddForce(forceAdd_);

            if(Mathf.Abs(rb.velocity.x) > MovementSpeed)
            {
                rb.velocity = new Vector2(MovementSpeed * InputDirect, rb.velocity.y);
            }
        }
        else if(!IsGrounded && !IsWallSlide && InputDirect == 0f && !Knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * AirDrag, rb.velocity.y);
        }
        

        if(IsWallSlide)
        {
            if(rb.velocity.y < -WallSildeSpeed)
            {
                if (VerticalDirect < -0.01f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSildeSpeed*5);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSildeSpeed);
                }
            }
        }
    }


    private void AnimationControl()
    {
        amt.SetBool("IsRun", IsRun);
        amt.SetBool("IsGrounded", IsGrounded);
        amt.SetFloat("yVelocity", rb.velocity.y);
        amt.SetBool("IsWallSlide", IsWallSlide);
        amt.SetFloat("yDirect", VerticalDirect);
    }



    private void checkKnockback()
    {
        if (Time.time >= KnockbackStartTime + KnockbackDuration && Knockback)
        {
            Knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            
        }
    }

    public bool getIsDash()
    {
        return IsDash;
    }
    
    public void KnockBack(int damageDirect)
    {
        Knockback = true;
        KnockbackStartTime = Time.time;
        rb.velocity = new Vector2(KnockbackSpeed.x * damageDirect, KnockbackSpeed.y);
    }

    

    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
        Gizmos.DrawLine(WallCheck.position, new Vector3(WallCheck.position.x + WallCheckDistance, WallCheck.position.y, WallCheck.position.z));
    }
}
