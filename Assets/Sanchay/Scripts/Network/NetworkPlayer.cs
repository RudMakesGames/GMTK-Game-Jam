using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fusion.Addons.Physics;
using UnityEngine.InputSystem;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{

    public static NetworkPlayer Local { get; set; }
    [Header("References Auto")]
    [SerializeField] Rigidbody rb;
    [SerializeField] CinemachineVirtualCamera cineCamMain, cineCamAds,cineCamParachute;
    [SerializeField] NetworkRigidbody3D NRb;
    [SerializeField] CapsuleCollider playerCollider;
    public Transform mainCam;
    [SerializeField] Animator anim;
    [SerializeField] Respawner respawnerScript;
    //[SerializeField] TextMeshProUGUI referencesCheckText;
    [SerializeField] CinemachineBrain cineBrain;

    [Header("References Manual")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float maxSpeed, knockBackStrength;

    [Header("Parachute Settings")]
    public GameObject parachuteVisual;
    public float parachuteFallSpeed = 2f;
    public float deployHeight = 15f;
    private bool isParachuting = false;
    private bool parachuteRequested = false;

    public bool isAiming;
    public bool isFiring;
    public bool isHit;

    public bool shouldTp;

    #region Input
    Vector2 moveInputVector = Vector2.zero;
    bool isJumping = false;
    bool isGrounded = false;
    Mouse mouseInputScript;
    float smoothVel;
    float camRotTemp;
    #endregion

    [SerializeField] Vector3 TeleportPoint;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NRb = GetComponent<NetworkRigidbody3D>();
        playerCollider = GetComponent<CapsuleCollider>();
        mouseInputScript = GetComponent<Mouse>();
        anim = GetComponent<Animator>();
        respawnerScript = GetComponent<Respawner>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("You have spawned");

            transform.name = $"P_{Object.Id}";

            cineCamMain = GameObject.Find("Main Follow Camera").GetComponent<CinemachineVirtualCamera>();
            cineCamAds = GameObject.Find("ADS Follow Camera ").GetComponent<CinemachineVirtualCamera>();
            cineCamParachute = GameObject.Find("Parachute Follow Camera").GetComponent<CinemachineVirtualCamera>();
            cineBrain = FindObjectOfType<CinemachineBrain>();
            mainCam = GameObject.Find("Main Camera").transform;
            
            cineCamMain.m_Follow = transform.Find("Follow Target");
            cineCamAds.m_Follow = transform.Find("Follow Target");
            cineCamParachute.m_Follow = transform.Find("Follow Target");
            /*referencesCheckText = FindObjectOfType<TextMeshProUGUI>();
            referencesCheckText.text = cineCamMain.m_Follow.name + transform.name + "\n" + cineCamAds.m_Follow.name + transform.name + "\n" + mainCam.name;*/
            TeleportPoint = GameObject.Find("TpPoint").transform.position;
           // mouseInputScript.assignReferences();
            mouseInputScript.MainCamera = cineCamMain;
            mouseInputScript.ADSCamera = cineCamAds;
            mouseInputScript.mainCam = mainCam;

            respawnerScript.setReferences();

            /*transform.Find("PlayerMesh").GetComponent<Renderer>().material = CharacterSelector.selectedMat;
            transform.GetComponentInChildren<TextMeshProUGUI>().text = CharacterSelector.selectedName;*/

            if (parachuteVisual == null)
            {
                parachuteVisual = GameObject.Find("ParachuteVisual");
            }
            parachuteVisual?.SetActive(false);
        }
        else
        {
            transform.name = $"P_{Object.Id}";
        }

        transform.Find("PlayerMesh").GetComponent<Renderer>().material = CharacterSelector.selectedMat;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = CharacterSelector.selectedName;

    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData input = new NetworkInputData();
        input.moveInput = moveInputVector;
        input.mouseInput.x = mouseInputScript.rotationY;
        input.mouseInput.y = mouseInputScript.rotationX;
        input.isJumping = isJumping;
        input.camRotY = camRotTemp;
        input.parachuteRequested = parachuteRequested;
        input.isFireButtonPressed = isFiring;
        input.isAiming = isAiming;
        input.isHit = isHit;

        isFiring = false;
        isJumping = false;
        //isHit = false;
        parachuteRequested = false; // Reset after send
        return input;
    }

    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Object.HasInputAuthority)
        {
            camRotTemp = mainCam.eulerAngles.y;

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                isJumping = true;
            }

            // Manual parachute request
            if (Input.GetKeyDown(KeyCode.E) && !isGrounded)
            {
                parachuteRequested = true;
            }
        }
    }

    public override void Render()
    {
        if (Object.HasInputAuthority)
        {
            cineBrain.ManualUpdate();
            cineCamMain.UpdateCameraState(Vector3.up, Runner.LocalAlpha);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            //isGrounded = GroundCheck();


            // Auto deploy parachute from height
            if (!isGrounded)
            {
                if (!isParachuting && transform.position.y > deployHeight && rb.velocity.y < -0.1f)
                {
                    isParachuting = true;
                    if(Object.HasInputAuthority)
                    {
                        parachuteVisual?.SetActive(true);
                        cineCamParachute.Priority = 2;
                        cineCamMain.Priority = 0;
                        cineCamAds.Priority = 0;
                    }
                   
                }
            }
            else if (isParachuting)
            {
                isParachuting = false;
                if (Object.HasInputAuthority)
                {
                    parachuteVisual?.SetActive(false);
                    cineCamMain.Priority = 2;
                    cineCamParachute.Priority = 0;
                    cineCamAds.Priority = 1;
                }
               
            }

            // Clamp below world
            
        }

        if (GetInput(out NetworkInputData input))
        {

            if (transform.position.y < -20f)
            {
                Tp(TeleportPoint);
            }

            float inputMag = input.moveInput.magnitude;
            Vector3 moveDir = new Vector3(input.moveInput.x, 0, input.moveInput.y).normalized;

            transform.rotation = Quaternion.Euler(0f, input.mouseInput.x, 0f);
            if (mouseInputScript.orientation != null)
                mouseInputScript.orientation.rotation = Quaternion.Euler(0f, input.mouseInput.x, 0f);
            if (mouseInputScript.cameraHolder != null)
                mouseInputScript.cameraHolder.localRotation = Quaternion.Euler(input.mouseInput.y, 0f, 0f);

            if (inputMag > 0.2f && !input.isHit)
            {
                float desiredAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + input.camRotY;
                //float desiredAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + mainCam.eulerAngles.y;
                Vector3 rotatedDir = Quaternion.Euler(0f, desiredAngle, 0f) * Vector3.forward.normalized;
                rb.velocity = new Vector3(rotatedDir.x * maxSpeed, rb.velocity.y, rotatedDir.z * maxSpeed);
            }

            // Manual parachute deploy
            if (input.parachuteRequested && !isGrounded && !isParachuting)
            {
                isParachuting = true;
                 parachuteVisual?.SetActive(true);
                cineCamParachute.Priority = 2;
                cineCamMain.Priority = 0;
                cineCamAds.Priority = 0;
            }

            // Apply parachute fall limit
            if (isParachuting && rb.velocity.y < -parachuteFallSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, -parachuteFallSpeed, rb.velocity.z);
            }

            // Jump
            if (input.isJumping)
            {
                rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
                //isJumping = false;
            }

            if(input.isFireButtonPressed)
            {
                mouseInputScript.FireReal();
            }

            /*if(input.isHit)
            {
                anim.SetTrigger("isHit");
            }*/

            if (shouldTp)
            {
                shouldTp = false;
                Tp(TeleportPoint);
            }

        }

        if (Object.HasInputAuthority)
        {
            isGrounded = GroundCheck();
            mouseInputScript.adsReal(input.isAiming);

            
            // referencesCheckText.text = $"{mouseInputScript.rotationY} / {mouseInputScript.rotationX}\nGrounded: {isGrounded}\nParachuting: {isParachuting}";
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.InputAuthority == player)
        {
            Runner.Despawn(Object);
        }
    }

    bool GroundCheck()
    {
        return Physics.Raycast(transform.position + transform.up, Vector3.down, (playerCollider.height / 2) + 1.5f, groundLayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if(collision.gameObject.CompareTag("Bullet") && Object.HasInputAuthority)
        {
            anim.SetTrigger("isHit");
            isHit = true;
            Vector3 bulletVelDirn = collision.gameObject.GetComponent<ProjectileRicochet>().projectileVelocity.normalized;
            bulletVelDirn.y = 0;
            Debug.Log(bulletVelDirn * knockBackStrength);
            rb.AddForce(bulletVelDirn*knockBackStrength);
        }*/
    }


    public void Tp(Vector3 tpPoint)
    {
        NRb.Teleport(tpPoint, Quaternion.identity);
        rb.MovePosition(tpPoint);
        /*if (transform.position.y < -20f)
        {
            NRb.Teleport(TeleportPoint, Quaternion.identity);

        }*/
    }
}
