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
    [SerializeField] 
        Rigidbody rb;
    [SerializeField]
        CinemachineVirtualCamera cineCamMain, cineCamAds;
    [SerializeField]
        NetworkRigidbody3D NRb;
    [SerializeField]
        CapsuleCollider playerCollider;
    [SerializeField] Transform mainCam;
    [SerializeField] TextMeshProUGUI referencesCheckText;
    [SerializeField] CinemachineBrain cineBrain;

    [Header("References Manual")]
    [SerializeField]
        LayerMask groundLayer;
    [SerializeField]
        float maxSpeed;

    #region Input
    Vector2 moveInputVector = Vector2.zero;
    bool isJumping = false; bool isGrounded = false;
    bool canAttack = true;
    Mouse mouseInputScript;
    float smoothVel;
    float camRotTemp;

    //float rotationY, rotationX;
    #endregion



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NRb = GetComponent<NetworkRigidbody3D>();
        playerCollider = GetComponent<CapsuleCollider>();
        mouseInputScript = GetComponent<Mouse>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        //Runner.SetIsSimulated(Object, Object.HasStateAuthority);
        if(Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("You have spawnned");

            transform.name = $"P_{Object.Id}";

            //transform.name = "YOU";

            cineCamMain = GameObject.Find("Main Follow Camera").GetComponent<CinemachineVirtualCamera>();
            cineCamAds = GameObject.Find("ADS Follow Camera ").GetComponent<CinemachineVirtualCamera>();

            cineBrain = FindObjectOfType<CinemachineBrain>();

            cineCamMain.m_Follow = transform.Find("Follow Target").transform;
            cineCamAds.m_Follow = transform.Find("Follow Target").transform;
            //cineCam.m_LookAt = transform;
            mainCam = GameObject.Find("Main Camera").GetComponent<Transform>();

            referencesCheckText = FindObjectOfType<TextMeshProUGUI>();
            referencesCheckText.text = cineCamMain.m_Follow.name + transform.name + "\n" + cineCamAds.m_Follow.name + transform.name + "\n" + mainCam.name;
            mouseInputScript.assignReferences();
        }
        else
        {
            Debug.Log("This is the client spawnned");
            transform.name = $"P_{Object.Id}";
        }

        //transform.name = $"P_{Object.Id}";

        //referencesCheckText.text = cineCamMain.m_Follow.name + transform.name + "\n" + cineCamAds.m_Follow.name + transform.name + "\n" + mainCam.name;


    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //movement
        networkInputData.moveInput = moveInputVector;
        networkInputData.mouseInput.x = mouseInputScript.rotationY;
        networkInputData.mouseInput.y = mouseInputScript.rotationX;

        //jump
        /*if (isJumping)
        {
            networkInputData.isJumping = isJumping;
        }*/

        networkInputData.isJumping = isJumping;
        networkInputData.camRotY = camRotTemp;

        /*if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            networkInputData.isJumping = true;
        }*/

        return networkInputData;
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if(Object.HasInputAuthority)
        camRotTemp = mainCam.eulerAngles.y;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            Debug.Log("jumping");
        }
    }

    public override void Render()
    {
        if(Object.HasInputAuthority)
        {
            cineBrain.ManualUpdate();
            cineCamMain.UpdateCameraState(Vector3.up, Runner.LocalAlpha);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            isGrounded = GroundCheck();

            if (!isGrounded)
            {
                rb.AddForce(Vector3.down * 8f);
            }

            if (transform.position.y < -20f)
            {
                NRb.Teleport(Vector3.zero, Quaternion.identity);
            }
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            float inputMag = networkInputData.moveInput.magnitude;
            Vector3 moveDirn = new Vector3(networkInputData.moveInput.x, 0, networkInputData.moveInput.y).normalized;


            transform.rotation = Quaternion.Euler(0f, networkInputData.mouseInput.x, 0f); // Rotate player body

            if (mouseInputScript.orientation != null)
            {
                mouseInputScript.orientation.rotation = Quaternion.Euler(0f, networkInputData.mouseInput.x, 0f);
            }

            if (mouseInputScript.cameraHolder != null)
            {
                mouseInputScript.cameraHolder.localRotation = Quaternion.Euler(networkInputData.mouseInput.y, 0f, 0f); // Rotate camera up/down
            }


            if (inputMag > 0.2f)
            {
                float desiredAngle = Mathf.Atan2(moveDirn.x, moveDirn.z) * Mathf.Rad2Deg + networkInputData.camRotY;
                //float currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, desiredAngle, ref smoothVel, 0.2f);


                Vector3 moveDirn2 = Quaternion.Euler(0f, desiredAngle, 0f) * Vector3.forward.normalized;
                rb.velocity = new Vector3(moveDirn2.x * maxSpeed, rb.velocity.y, moveDirn2.z * maxSpeed);
            }

            if (networkInputData.isJumping)
            {
                rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
                Debug.Log("jumping");
                isJumping = false;
                networkInputData.isJumping = false;
            }

        }

        if(Object.HasInputAuthority)
        {
            referencesCheckText.text = mouseInputScript.rotationY.ToString() + " and " + mouseInputScript.rotationX.ToString()+"\n"+"is Grounded:"+isGrounded+"\n"+"isJumping:"+isJumping;
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(Object.InputAuthority == player)
        {
            Runner.Despawn(Object);
        }
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
