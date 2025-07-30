using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fusion.Addons.Physics;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{

    public static NetworkPlayer Local { get; set; }

    [Header("References Auto")]
    [SerializeField] 
        Rigidbody rb;
    [SerializeField]
        CinemachineFreeLook cineCam;
    [SerializeField]
        NetworkRigidbody3D NRb;
    [SerializeField]
        CapsuleCollider playerCollider;
    [SerializeField] Camera mainCam;

    [Header("References Manual")]
    [SerializeField]
        LayerMask groundLayer;
    [SerializeField]
        float maxSpeed;

    #region Input
    Vector2 moveInputVector = Vector2.zero;
    public bool isJumping = false; public bool isGrounded = false;
    bool canAttack = true;

    float smoothVel;
    #endregion



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NRb = GetComponent<NetworkRigidbody3D>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if(Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("You have spawnned");
            transform.name = "YOU";

            cineCam = FindObjectOfType<CinemachineFreeLook>();

            cineCam.m_Follow = transform;
            cineCam.m_LookAt = transform;
            mainCam = FindObjectOfType<Camera>();
        }
        else
        {
            transform.name = $"P_{Object.Id}";
        }

        
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //movement
        networkInputData.moveInput = moveInputVector;

        //jump
        if (isJumping)
        {
            networkInputData.isJumping = true;
        }


        return networkInputData;
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            Debug.Log("jumping");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority)
        {
            isGrounded = GroundCheck();

            if (transform.position.y < -20f)
            {
                NRb.Teleport(Vector3.zero, Quaternion.identity);
            }

            /*if (!isGrounded)
                rb.AddForce(Vector3.down * 8f);*/ //fix floaty players    
        }

        if(GetInput(out NetworkInputData networkInputData))
        {
            float inputMag = networkInputData.moveInput.magnitude;
            /*Vector3 localVeclocityVsFwd = transform.forward*Vector3.Dot(transform.forward, rb.velocity);
            float localFwdVelocity = localVeclocityVsFwd.magnitude;*/

            Vector3 moveDirn = new Vector3(networkInputData.moveInput.x, 0, networkInputData.moveInput.y).normalized;

            if(inputMag>0.02f)
            {
                float desiredAngle = Mathf.Atan2(moveDirn.x, moveDirn.z)*Mathf.Rad2Deg + mainCam.transform.rotation.y;
                float currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, desiredAngle, ref smoothVel, 0.2f);
                transform.rotation = Quaternion.Euler(0, currentAngle, 0);

                //transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, Runner.DeltaTime*250f);

                /*if(rb.velocity.magnitude<maxSpeed)
                {
                    rb.AddForce(transform.forward*inputMag*30);
                }*/

                Vector3 moveDirn2 = Quaternion.Euler(0f,desiredAngle, 0f)*Vector3.forward.normalized;
                rb.velocity = new Vector3(moveDirn2.x*maxSpeed, rb.velocity.y, moveDirn2.z*maxSpeed);
            }

            if (isJumping)
            {
                rb.AddForce(Vector3.up*20, ForceMode.Impulse);
                Debug.Log("jumping");
                isJumping = false;
            }
        }

    }

    public void PlayerLeft(PlayerRef player)
    {

    }

    bool GroundCheck()
    {
        //Ray groundCheckRay;
        if (Physics.Raycast(transform.position + transform.up, Vector3.down, (playerCollider.height / 2) + 1.5f, groundLayer))
        {
            //Debug.Log("hit ground");
            //Debug.DrawRay(transform.position + transform.up, Vector3.down * ((playerCollider.height / 2) + 1.8f), Color.red);
            return true;
        }
        return false;
    }
}
