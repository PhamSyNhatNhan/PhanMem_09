using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    private PlayerStat ps;
    public Image[] life;

    private void Start()
    {
        ps = GameObject.Find("Player").GetComponent<PlayerStat>();
        SubModifyLife();
    }
    public void SubStart()
    {
        ps = GameObject.Find("Player(Clone)").GetComponent<PlayerStat>();
        SubModifyLife();
    }

    public void ModifyLife()
    {
        try
        {
            int lifeCount = (int)(ps.GetCurrentHealth());
            Debug.Log("Life" + lifeCount);
        
            for (int i = 0; i < 5; i++)
            {
                if (i >= lifeCount)
                {
                    life[i].enabled = false;
                }
                else
                {
                    life[i].enabled = true;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public void SubModifyLife()
    {
        for (int i = 0; i < 5; i++)
        {
            life[i].enabled = true;
        }
    }
}
