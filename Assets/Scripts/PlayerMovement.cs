using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask Ground;
    public LayerMask mouseAimMask;
    public GameObject bulletPrefab;
    public Transform targetTransform;
    public Transform muzzleTransform;


    //float JumpHeight = 3f;
    private Animator animationsPlayer;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float groundDistance = 0.2f;

    private bool aimState;
    private Rigidbody body;
    private Camera mainCamera;
    private bool isGrounded = true;
    private Transform groundChecker;
    private Vector3 inputs = Vector3.zero;

    [SerializeField] private AnimationCurve recoilCurve;
    public float recoilTimer, recoilDuration = .25f, recoilMaxrotation = 45f;
    [SerializeField] private Transform rightLowerArm, rightHand;

    [SerializeField] private int gunInPlace, ammoCount = 12;

    public String[] guns = { "Pistol", "Shotgun", "Assault Rifle" };
    public int[] gunAmmoCount = { 12, 6, 30 };


    void Start()
    {
        mainCamera = Camera.main;
        body = GetComponent<Rigidbody>();
        groundChecker = transform.GetChild(0);
        animationsPlayer = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        WeaponAim();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = hit.point;
        }

        ProjectileShooting();
        Reload();
    }


    void FixedUpdate()
    {
        body.MovePosition(body.position + inputs * speed * Time.fixedDeltaTime);
        //body.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
    }

    private void LateUpdate()
    {
        //
        if(recoilTimer < 0)
        {
            return;
        }
        float curveTime = (Time.time - recoilTimer) / (recoilDuration);
        if(curveTime > 1)
        {
            recoilTimer = -1;
        }
        else
        {
            rightLowerArm.Rotate(Vector3.forward, recoilCurve.Evaluate(curveTime) * recoilMaxrotation, Space.Self);
        }
    }

    void Movement()
    {
        HorizontalMovement();
        Dash();

    }

    void HorizontalMovement()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);

        inputs = Vector3.zero;
        inputs.x = Input.GetAxis("Horizontal");
        //_inputs.z = Input.GetAxis("Vertical");
        if (inputs != Vector3.zero)
        {
            //transform.forward = _inputs;
            animationsPlayer.SetFloat("playerMovement", .1f);
            transform.rotation = Quaternion.LookRotation(inputs);
        }
        else
        {
            animationsPlayer.SetFloat("playerMovement", 0);
        }
    }

   private void ProjectileShooting()
    {
        /*
         * 
         */
        if(aimState && Input.GetButtonDown("Fire1"))
        {
            Fire();
        }

        /*
         * Byllet should follow a random path between 75 and -75 degrees.
         */
        else if(!aimState && Input.GetButtonDown("Fire1"))
        {

        }
    }

    private void Fire()
    {
        if(ammoCount > 0)
        {
            recoilTimer = Time.time;
            var go = Instantiate(bulletPrefab);
            go.transform.position = muzzleTransform.position;
            var bullet = go.GetComponent<Bullet>();
            bullet.Fire(go.transform.position, muzzleTransform.eulerAngles, gameObject.layer);
            ammoCount -= 1;
        }
        else
        {
            Debug.Log("Press R to reload.");
        }
        
    }

    void WeaponAim()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            animationsPlayer.SetBool("aiming", true);
        }
        else if(Input.GetKeyUp(KeyCode.C))
        {
            animationsPlayer.SetBool("aiming", false);
        }
    }

    private void OnAnimatorIK()
    {
        if(AimState())
        {
            Aim();
        }
    }

    private bool AimState()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            aimState = true;
        }

        else if(Input.GetKeyUp(KeyCode.V))
        {
            aimState = false;
        }
        return aimState;
    }

    private void Aim()
    {
        //Arms path
        //targetTransform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, 0);

        animationsPlayer.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animationsPlayer.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
        animationsPlayer.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animationsPlayer.SetIKPosition(AvatarIKGoal.LeftHand, targetTransform.position);

        //Head/eye path.
        animationsPlayer.SetLookAtWeight(1);
        animationsPlayer.SetLookAtPosition(targetTransform.position);
    }

    /*
     * Decide whether to make sprint a set of if statements, or a separate method.
     * Make it so instead of a dash this function acts as a sprint.
     */
    void Dash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * body.drag + 1)) / 
                -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * body.drag + 1)) / -Time.deltaTime)));
            
            //Should be dashVelocity but it currently calculating as 0,0,0. Analyse and fix.
            body.AddForce(dashDistance, 0, 0, ForceMode.VelocityChange);
            Debug.Log("Dash pressed. - " + dashDistance);
        }

     /*       
         * Make player speed increase the longer the designated key is pressed,
         * or as long as there's stamina available (system yet to be immplemented).
         
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }

        
         * Player's speed decreases back to default speed after key is no longer pressed.
         * Stamina bar slowly 'refills' back to default state.
        
        else if(Input.GetKeyUp(KeyCode.Space))
        {

        }
    */
    }
    public void Reload()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (ammoCount == 0)
            {
                //animationsPlayer.SetBool("reloading", true);
                ammoCount = gunAmmoCount[0]; //Should be gunInPlace but will use default pistol setting as weapon system isn't yet developed.
            }
            if (ammoCount <= (12 / 2)) //12 is temporary value as weapon system is developed where Unity identifies what weapon is being held.
            {
                //animationsPlayer.SetBool("reloading", true);
                ammoCount = gunAmmoCount[0];
            }
            else
            {
                /* Reloading is denied and a voice-clip says how it'll be stupid to be wasteful when she still has more than hald a clip in.
                * This means denying the player a basic action which would be annoying, still needs more pondering whether to follow through this idea,
                * or let the player reload as much as they want.
                */
            }
        }

    }
}