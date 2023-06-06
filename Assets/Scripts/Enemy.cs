using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //There will be different enemies with different health values (wearing bulletproof vests, helmets for headshots, etc.)
    private int healthValue;

    // Start is called before the first frame update
    void Start()
    {
        //Placeholder names.
        if(this.name == "Enemy0")
        {
            healthValue = 8;
        }
        if (this.name == "Enemy1")
        {
            healthValue = 10;
        }
        if (this.name == "Enemy2")
        {
            healthValue = 12;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
