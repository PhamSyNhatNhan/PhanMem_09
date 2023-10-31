using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1_Controller : MonoBehaviour
{
    private enum State
    {
        Move,
        Death
    }

    private State curState;

    [Header("Stat")]
    [SerializeField] private float maxHealth;
    private float curHealth;
    
    [Header("System")] 
    [SerializeField] private Transform GroundCheck, WallCheck;
    [SerializeField] private LayerMask WhatIsGround, WhatIsWall;
    [SerializeField] private float GroundCheckDistance, WallCheckDistance;
    private bool GroundDetect, WallDetect;
    private int FacingDirect;
    private Rigidbody2D rb;
    [SerializeField] private GameObject HitParticle;
    private Animator amt;

    [Header("Walk")] 
    [SerializeField] private float moveSpeed;

    [Header("KnockBack")]
    [SerializeField] private float knockbackDuration;
    private float KnockBackStart;
    [SerializeField] private Vector2 KnockBackVector;
    private int damageDirect;
    private bool IsKnockBack = false;

    [Header("Combat")] 
    [SerializeField] private float lastCollisionDamageTime;
    [SerializeField] private float CollisionDamageCooldown;
    [SerializeField] private float CollisionDamage;
    [SerializeField] private float CollisionDamageWidth;
    [SerializeField] private float CollisionDamageHeight;
    [SerializeField] private LayerMask WhatIsPlayer;
    private Vector2 CollisionDamageBL;
    private Vector2 CollisionDamageTR;
    private float[] attackDerails = new float[2];
    [SerializeField] private Transform CollisionDamgeCheck; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amt = GetComponent<Animator>();
        curHealth = maxHealth;
        FacingDirect = 1;
    }

    // Update is called once per frame
    void Update()
    {
        ReloadKnockBack();
            
        if (curState == State.Move)
        {
            UpdateMove();
        }
        else if (curState == State.Death)
        {
            UpdateDeath();
        }
    }
    
    // walk
    private void EnterMove()
    {
        
    }

    private void UpdateMove()
    {
        if (IsKnockBack) return;
        GroundDetect = Physics2D.Raycast(GroundCheck.position, Vector2.down, GroundCheckDistance, WhatIsGround);
        WallDetect = Physics2D.Raycast(WallCheck.position, Vector2.right, WallCheckDistance, WhatIsWall);

        checkCollisionDamge();
        
        if (!GroundDetect || WallDetect)
        {
            Facing();
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed * FacingDirect, rb.velocity.y);
        }
    }

    private void ExitMove()
    {
        
    }
    
    // Death
    private void EnterDeath()
    {
        amt.SetBool("Death", true);
    }

    private void ExitDeath_Ani_End()
    {
        Destroy(gameObject);
    }

    private void UpdateDeath()
    {
        
    }

    private void ExitDeath()
    {
        
    }


    private void switchState(State state)
    {
        if (curState == State.Move) ExitMove();  
        else if (curState == State.Death) ExitDeath();

        if (state == State.Move) EnterMove(); 
        else if (state == State.Death) EnterDeath();

        curState = state;
    }

    private void Facing()
    {
        FacingDirect *= -1;
        
        amt.SetBool("Turn", true);
        transform.Rotate(0f, 180f, 0f);
    }

    private void Facing_Ani_End()
    {
        amt.SetBool("Turn", false);
    }
    
    public void Damage(float[] atkDetails)
    {
        curHealth -= atkDetails[0];

        if (atkDetails[1] > transform.position.x) damageDirect = -1;
        else damageDirect = 1;
        
        Instantiate(HitParticle, transform);

        if (curHealth > 0.0f)
        {
            KnockBack();
        }
        else if (curHealth <= 0.0f)
        {
            KnockBack_Death();
            switchState(State.Death);
        }
    }
    private void checkCollisionDamge(){
        if (Time.time >= lastCollisionDamageTime + CollisionDamageCooldown)
        {
            CollisionDamageBL.Set(CollisionDamgeCheck.position.x - (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y - (CollisionDamageHeight / 2));
            CollisionDamageTR.Set(CollisionDamgeCheck.position.x + (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y + (CollisionDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(CollisionDamageBL, CollisionDamageTR, WhatIsPlayer);

            if (hit != null)
            {
                lastCollisionDamageTime = Time.time;
                attackDerails[0] = CollisionDamage;
                attackDerails[1] = transform.position.x;
                hit.SendMessage("Damage", attackDerails);
            }
        }
    }

    private void KnockBack()
    {
        KnockBackStart = Time.time;
        IsKnockBack = true;
        KnockBackStart = Time.time;
        rb.velocity = new Vector2(KnockBackVector.x * damageDirect, KnockBackVector.y);
    }

    private void ReloadKnockBack()
    {
        if (Time.time > KnockBackStart + knockbackDuration)
        {
            IsKnockBack = false;
        }
    }
    private void KnockBack_Death()
    {
        KnockBackStart = Time.time;
        rb.velocity = new Vector2(KnockBackVector.x*2.0f * damageDirect, KnockBackVector.y*2.0f);
    }
    
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(GroundCheck.position, new Vector2(GroundCheck.position.x, GroundCheck.position.y - GroundCheckDistance));
        Gizmos.DrawLine(WallCheck.position, new Vector2(WallCheck.position.x + WallCheckDistance, WallCheck.position.y));

        Vector2 bl = new Vector2(CollisionDamgeCheck.position.x - (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y - (CollisionDamageHeight / 2));
        Vector2 br = new Vector2(CollisionDamgeCheck.position.x + (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y - (CollisionDamageHeight / 2));
        Vector2 tr = new Vector2(CollisionDamgeCheck.position.x + (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y + (CollisionDamageHeight / 2));
        Vector2 tl = new Vector2(CollisionDamgeCheck.position.x - (CollisionDamageWidth / 2), CollisionDamgeCheck.position.y + (CollisionDamageHeight / 2));
        
        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);
    }
}
