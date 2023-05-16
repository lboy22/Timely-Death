using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask Ground;
    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public GameObject bulletPrefab;
    public Transform muzzleTransform;

    private Animator animationsPlayer;
    [SerializeField] float speed = 2f;
    //float JumpHeight = 3f;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] float dashDistance = 5f;
    

    private Rigidbody body;
    private Vector3 inputs = Vector3.zero;
    private bool isGrounded = true;
    private Transform groundChecker;
    private Camera mainCamera;
    private bool aimState;
    
    


    void Start()
    {
        body = GetComponent<Rigidbody>();
        groundChecker = transform.GetChild(0);
        animationsPlayer = GetComponent<Animator>();
        mainCamera = Camera.main;
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
    }


    void FixedUpdate()
    {
        body.MovePosition(body.position + inputs * speed * Time.fixedDeltaTime);
        //body.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
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
         * 
         */
        else if(!aimState && Input.GetButtonDown("Fire1"))
        {

        }
    }

    private void Fire()
    {
        var go = Instantiate(bulletPrefab);
        go.transform.position = muzzleTransform.position;
        var bullet = go.GetComponent<Bullet>();
        bullet.Fire(go.transform.position, muzzleTransform.eulerAngles, gameObject.layer);
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

    void checkAnimation()
    {
        
    }
}