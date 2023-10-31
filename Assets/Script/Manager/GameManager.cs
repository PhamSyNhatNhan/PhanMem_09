using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform RespawnPoint;
    [SerializeField] private GameObject Player;
    [SerializeField] private float RespawnTime;
    [SerializeField] private float RespawnTimeStart;
    private bool Respawn = false;
    private CinemachineVirtualCamera cvc;
    private LifeManager lm;

    private void Start()
    {
        cvc = GameObject.Find("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        lm = GetComponent<LifeManager>();
    }
    private void Update()
    {
        checkRespawn_();
    }


    public void Respawn_()
    {
        RespawnTimeStart = Time.time;
        Respawn = true;
    }

    private void checkRespawn_()
    {
        if (Time.time > RespawnTimeStart + RespawnTime && Respawn)
        {
            var playertmp = Instantiate(Player, RespawnPoint);
            lm.SubStart();
            cvc.m_Follow = playertmp.transform;
            Respawn = false;
        }
    }
}
