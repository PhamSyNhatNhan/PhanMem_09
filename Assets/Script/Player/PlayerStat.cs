using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5;
    private LifeManager lm;
    private float curentHealth = 5;
    private Player_Controller pc;
    private GameManager gm;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = GetComponent<Player_Controller>();
        lm = GameObject.Find("GameManager").GetComponent<LifeManager>();
        curentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return curentHealth;
    }

    public void SetCurrentHealth()
    {
        curentHealth = maxHealth;
    }

    public void DecreaseHealth(float damage)
    {
        curentHealth -= damage;
        lm.ModifyLife();
        
        if (curentHealth <= 0.0f)
        {
            Death();
        }
    }

    private void Death()
    {
        pc.GetAmt().SetBool("Death", true);
        pc.SetPlayer(false);
        pc.Setknoback(false);
        pc.GetRb().velocity = new Vector2(0.0f, 0.0f);
    }

    private void SubDeath()
    {
        gm.Respawn_();
        Destroy(gameObject);
    }
}
