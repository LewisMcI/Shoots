using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    float maxHealth;
    float currHealth;

    private void Awake()
    {
        currHealth = maxHealth;
    }

    void ResetHealth()
    {
        currHealth = maxHealth;
    }

    public void DealDamage(float damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Player Dead");
    }
}
