using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //There will be different enemies with different health values (wearing bulletproof vests, helmets for headshots, etc.)
    [SerializeField] private float healthValue = 0, damageOutPut = 0;

    public String[] guns = { "Pistol", "Shotgun", "Assault Rifle" };
    public float[] gunProjectileDamage = { .2f, .6f, .3f };
    // Start is called before the first frame update
    void Start()
    {
        //Placeholder names.
        if(CompareTag("Enemy"))
        {
            healthValue = 8;
        }
        
        else if(CompareTag("Enemy1"))
        {
            healthValue = 10;
        }
        else if(CompareTag("Enemy2"))
        {
            healthValue = 12;
        }

    }

    // Update is called once per frame
    void Update()
    {}

    private void LogicBehavior()
    {

    }

    private void HealthState()
    {
        healthValue -= damageOutPut;
        if (healthValue <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ProjectileType(Collision collision)
    {
        for (int index = 0; index < guns.Length; index++)
        {
            if (collision.gameObject.tag == guns[index])
            {
                damageOutPut = gunProjectileDamage[index];
                HealthState();
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProjectileType(collision);
    }
}
