using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol: Bullet
{
    //Bullet class's name will have to be redefined to encapsulate all weapons.
    [SerializeField] Bullet gunBaseClass;


    //Clip size will correspond to real gun in which is based.
    //Temporary clipSize value.
    [SerializeField]private int clipSize = 8;
    [SerializeField]private float damageOutput = .5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //gunBaseClass.Reload(clipSize);
    }
}
