using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [SerializeField] private bool CombatEnable;
    private bool GotInput;
    private bool IsAttack;
    private bool IsFirstAttack;
    private float LastInputTime = Mathf.NegativeInfinity;
    [SerializeField] GameObject Attack1Eff_1;
    [SerializeField] GameObject Attack1Eff_2;
    [SerializeField] private float InputTime;
    [SerializeField] private Transform AttackHitBoxPos;
    [SerializeField] private LayerMask WhatIsDamgeEnable;
    [SerializeField] private float AtackRadius;
    [SerializeField] private float AttackDamage;
    [SerializeField] private float AttackTimeTran;
    [SerializeField] private float[] AttackDetail = new float[2];
    private Animator amt;
    private Player_Controller pc;


    // Start is called before the first frame update
    void Start()
    {
        amt = GetComponent<Animator>();
        pc = GetComponent<Player_Controller>();
        amt.SetBool("CanAttack", CombatEnable);
    }

    // Update is called once per frame
    void Update()
    {
        ResetAttack();
        CheckCombatInput();
        CheckAttack();
    }

    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(CombatEnable)
            {
                GotInput = true;
                LastInputTime = Time.time;
            }
        }
    }

    private void CheckAttack()
    {
        if(GotInput)
        {
            if (!IsAttack)
            {
                GotInput = false;
                IsAttack = true;
                IsFirstAttack = !IsFirstAttack;
                amt.SetBool("Attack_1", true);
                amt.SetBool("FirstAtack", IsFirstAttack);
                amt.SetBool("IsAttack", IsAttack);
            }
        }

        if(Time.time >= LastInputTime + InputTime)
        {
            GotInput = false;
        }
    }

    private void ResetAttack()
    {
        if (Time.time >= LastInputTime + AttackTimeTran)
        {
            IsFirstAttack = false;
        }
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBoxPos.position, AtackRadius, WhatIsDamgeEnable);

        AttackDetail[0] = AttackDamage;
        AttackDetail[1] = transform.position.x;
        
        foreach(Collider2D Colider in DetectObject)
        {
            Colider.transform.SendMessage("Damage", AttackDetail);
        }
    }

    private void FinishAttack()
    {
        IsAttack = false;
        amt.SetBool("IsAttack", IsAttack);
        amt.SetBool("Attack_1", false);
    }

    private void Attack1Effect_1()
    {
        Instantiate(Attack1Eff_1, transform);
    }
    private void Attack1Effect_2()
    {
        Instantiate(Attack1Eff_2, transform);
    }
    
    private void Damage(float[] attackDetails)
    {
        if (!pc.getIsDash())
        {
            int damageDirect;
            if (attackDetails[1] < transform.position.x) damageDirect = 1;
            else damageDirect = -1;
            
            pc.KnockBack(damageDirect);
        }
        
        pc.GetPs().DecreaseHealth(attackDetails[0]);
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackHitBoxPos.position, AtackRadius);
    }
}
