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
    [Networked, OnChangedRender(nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    [Networked, OnChangedRender(nameof(OnMaterialChanged))]
    public int materialIndex { get; set; }

    [Networked, OnChangedRender(nameof(OnWeaponChanged))]
    public int currentWeapon { get; set; }

    [Networked, OnChangedRender(nameof(OnProjectileChanged))]
    public NetworkBool loopMode { get; set; }


    //[Networked] int nickMat {  get; set; } 
    public string nicknameTemp; public int materianIndexTemp;

    public Material[] materials;

    NetworkMecanimAnimator networkAnim;

    public MatchManager matchManagerInstance;

    bool quip1Called = false; bool quip2Called=false;

    public bool isSinglePlayer=false;


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
    //[SerializeField] int currentWeaponIndex;
    [SerializeField] CinemachineBrain cineBrain;
    [SerializeField] TextMeshProUGUI matchTimerText;

    [Header("References Manual")]
    public LayerMask groundLayer;
    [SerializeField] float maxSpeed, knockBackStrength;

    [Header("Parachute Settings")]
    public GameObject parachuteVisual;
    public float parachuteFallSpeed = 2f;
    public float deployHeight = 15f;
    private bool isParachuting = false;
    private bool parachuteRequested = false;

    bool matchEnded = false;

    public bool isAiming;
    public bool isFiring;
    public bool isHit;

    public bool shouldTp;

    public GameObject enemyPrefab;

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
        networkAnim = GetComponent<NetworkMecanimAnimator>();
        respawnerScript = GetComponent<Respawner>();
    }

    public override void Spawned()
    {
        /*if(Object.HasStateAuthority)
        {
            nickName = CharacterSelector.selectedName;
            //nickMat = CharacterSelector.selectedMat;
        }*/
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
            
            cineCamMain.m_Follow = transform.Find("Follow Target"); //cineCamMain.m_LookAt = transform.Find("Follow Target");
            cineCamAds.m_Follow = transform.Find("Follow Target"); //cineCamAds.m_LookAt = transform.Find("Follow Target");
            cineCamParachute.m_Follow = transform.Find("Follow Target"); //cineCamParachute.m_LookAt = transform.Find("Follow Target");
            /*referencesCheckText = FindObjectOfType<TextMeshProUGUI>();
            referencesCheckText.text = cineCamMain.m_Follow.name + transform.name + "\n" + cineCamAds.m_Follow.name + transform.name + "\n" + mainCam.name;*/
            TeleportPoint = GameObject.Find("TpPoint").transform.position;
           // mouseInputScript.assignReferences();
            mouseInputScript.MainCamera = cineCamMain;
            mouseInputScript.ADSCamera = cineCamAds;
            mouseInputScript.mainCam = mainCam;

            respawnerScript.setReferences();

            //transform.Find("PlayerMesh").GetComponent<Renderer>().material = CharacterSelector.selectedMat;
            //transform.GetComponentInChildren<TextMeshProUGUI>().text = CharacterSelector.selectedName;

            //SetNameRPC(PlayerPrefs.GetString("PlayerNickName"));

            //StartCoroutine(NameChangerCoroutine());

            if (parachuteVisual == null)
            {
                parachuteVisual = GameObject.Find("ParachuteVisual");
            }
            parachuteVisual?.SetActive(false); //materianIndexTemp = PlayerPrefs.GetInt("SelectedMatIndex");
            SetNameRPC(PlayerPrefs.GetString("PlayerNickName"), PlayerPrefs.GetInt("SelectedMatIndex"));

           // currentWeaponIndex = 0;

            matchTimerText = GameObject.Find("MatchTimerText").GetComponent<TextMeshProUGUI>();

            //if(Runner.IsSharedModeMasterClient) 
                MatchManagerFinderHelper();
            /*else
            {
                matchManagerInstance = FindObjectOfType<MatchManager>();
                matchManagerInstance.MatchTime = 0;
            }*/

            Debug.Log(CharacterSelector.selectedMode);

            if(CharacterSelector.selectedMode!=1)
            {
                Debug.Log("player has selected single player mode");
                gameObject.AddComponent<PlayerRayCastScript>();
                isSinglePlayer=true;
                GameObject.Find("EnemySpawn").AddComponent<EnemySpawnner>().EnemyPrefab = enemyPrefab;
                
            }
        }
        else
        {
            transform.name = $"P_{Object.Id}";
            NetworkPlayer[] allPlayers = FindObjectsOfType<NetworkPlayer>();

            foreach (NetworkPlayer player in allPlayers)
            {
                player.SetNameRPC(player.nicknameTemp, player.materianIndexTemp);
            }
            //this.SetNameRPC(PlayerPrefs.GetString("PlayerNickName"));
            //SetNameRPC(PlayerPrefs.GetString("PlayerNickName"));
        }

        /*transform.Find("PlayerMesh").GetComponent<Renderer>().material = CharacterSelector.selectedMat;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = CharacterSelector.selectedName;*/

    }

    public void MatchManagerFinderHelper()
    {
        StartCoroutine(MatchManagerFinder());
    }

    public IEnumerator MatchManagerFinder()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Finding matchmanager");

        matchManagerInstance = FindObjectOfType<MatchManager>();
        /*if (matchManagerInstance != null) Debug.Log("matchmanger found");
        else Debug.Log("matchmanager not found");*/
        matchManagerInstance.MatchTime = 0;

        //SetNameRPC(PlayerPrefs.GetString("PlayerNickName"));

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
        //input.currentWeapon = currentWeaponIndex;

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

        /*if (isSinglePlayer)
        {
            Vector3 center = transform.position;

            for (int i = 0; i < numberOfPoints; i++)
            {
                float angle = i * Mathf.PI * 2f / numberOfPoints;
                Vector3 point = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

                // Direction: outward from center to point
                Vector3 direction = (point - center).normalized;

                Debug.DrawRay(center, direction * rayDistance, Color.red);

                if (Physics.Raycast(center, direction, out RaycastHit hit, rayDistance, raycastMask))
                {
                    Debug.Log($"Ray {i} hit {hit.collider.name} at {hit.point}");
                }
            }
        }*/
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
        /*if (Object.HasInputAuthority)
        {
            Debug.Log("Match Time->" + matchManagerInstance.MatchTime);
        }*/
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
                        rb.drag = 5;
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
                    rb.drag = 0;
                }
               
            }

            // Clamp below world
            
        }


        if (GetInput(out NetworkInputData input))
        {
            /*if (Input.GetButtonDown("WeaponSwitch"))
            {
                currentWeapon = 1;
            }*/

            if (transform.position.y < -20f)
            {
                Tp(TeleportPoint);
                this.respawnQuip();

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
                rb.drag = 5;
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
                if(currentWeapon!=2)
                {
                    mouseInputScript.FireReal();
                }

                else if(currentWeapon==2)
                {
                    networkAnim.SetTrigger("spatulaAttack");
                }
                    //spatulaAttackCode
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
            //Debug.Log("Match Time->" + matchManagerInstance.MatchTime);
            matchTimerText.text = Mathf.Ceil(matchManagerInstance.MatchTime).ToString();

            if (matchManagerInstance.MatchTime > 150f && matchManagerInstance.MatchTime < 160f && !quip1Called)
            {
                quip1Called = true;
                currentWeapon = 1;
            }
            else if(matchManagerInstance.MatchTime > 250f && !quip2Called)
            {
                quip2Called = true;
                currentWeapon = 2;
            }

            if(matchManagerInstance.MatchTime>350f && !matchEnded)
            {
                matchEnded = true;
                LeanTween.move(GameObject.Find("QUIPS4").GetComponent<RectTransform>(), new Vector2(0, 0), 1f).setEaseOutQuad();
                Invoke("changeToFinalScene", 5f);
            }

            // referencesCheckText.text = $"{mouseInputScript.rotationY} / {mouseInputScript.rotationX}\nGrounded: {isGrounded}\nParachuting: {isParachuting}";
        }
    }

    void changeToFinalScene()
    {
        Runner.LoadScene("FinalResults");
    }

    public void respawnQuip()
    {
        Vector2 ogPos = GameObject.Find("QUIPS3").GetComponent<RectTransform>().position;
        LeanTween.move(GameObject.Find("QUIPS3").GetComponent<RectTransform>(), new Vector2(725, 300), 1f).setEaseOutQuad().setOnComplete(() =>
        {
            LeanTween.delayedCall(GameObject.Find("QUIPS3").gameObject, 10f, () =>
            {
                LeanTween.move(GameObject.Find("QUIPS3").gameObject, ogPos, 0.5f).setEaseInQuad();
            });
        });
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
        if (!Object.HasInputAuthority) return;
        if (!isSinglePlayer) return;

        if(collision.gameObject.CompareTag("Bullet"))
        {
            anim.SetTrigger("isHit");
            isHit = true;
            Vector3 bulletVelDirn = /*collision.gameObject.GetComponent<ProjectileRicochet>().projectileVelocity.normalized*/collision.gameObject.GetComponent<Rigidbody>().velocity.normalized;
            bulletVelDirn.y = 0;
            Debug.Log("addoing force locally->" + bulletVelDirn * knockBackStrength);
            rb.AddForce(bulletVelDirn * knockBackStrength);
        }
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

    public void OnNickNameChanged()
    {
        Debug.Log("setting nickname");
        transform.GetComponentInChildren<TextMeshProUGUI>().text = nickName.ToString();
        //nicknameTemp = nickname2;
    
    }

    void OnMaterialChanged()
    {
        //helper empty method delegate ke liye
    }

    void OnProjectileChanged()
    {
        if(this.loopMode && this.materialIndex==0)
        {
            mouseInputScript.changeProjectile(2);
        }
        else if(this.loopMode && this.materialIndex!=0)
        {
            mouseInputScript.changeProjectile(1);
        }
        else if(!this.loopMode)
        {
            mouseInputScript.changeProjectile(0);
        }
    }


    void OnWeaponChanged()
    {
        Debug.Log("changing weapons");
        switch(currentWeapon)
        {
            case 0:
                transform.Find("Follow Target").transform.Find("goldenspatula").gameObject.SetActive(false);
                transform.Find("Follow Target").transform.Find("railgun").gameObject.SetActive(true);
                loopMode = false;
            break;

            case 1:
                transform.Find("Follow Target").transform.Find("goldenspatula").gameObject.SetActive(false);
                transform.Find("Follow Target").transform.Find("railgun").gameObject.SetActive(true);
                loopMode = true;
                Vector2 ogPos = GameObject.Find("QUIPS1").GetComponent<RectTransform>().position;
                LeanTween.move(GameObject.Find("QUIPS1").GetComponent<RectTransform>(), new Vector2(0, 0), 1f).setEaseOutQuad().setOnComplete(()=>
                {
                    LeanTween.delayedCall(GameObject.Find("QUIPS1").gameObject, 3.5f, () =>
                    {
                        LeanTween.move(GameObject.Find("QUIPS1").gameObject, ogPos, 0.5f).setEaseInQuad();
                    });
                });
                break;
            case 2:
                transform.Find("Follow Target").transform.Find("railgun").gameObject.SetActive(false);
                transform.Find("Follow Target").transform.Find("goldenspatula").gameObject.SetActive(true);
                loopMode = false;
                //LeanTween.move(GameObject.Find("QUIPS2"), new Vector2(0, 0), 0.5f).setLoopPingPong(1);
                Vector2 ogPos2 = GameObject.Find("QUIPS2").GetComponent<RectTransform>().position;
                LeanTween.move(GameObject.Find("QUIPS2").GetComponent<RectTransform>(), new Vector2(0, 0), 1f).setEaseOutQuad().setOnComplete(() =>
                {
                    LeanTween.delayedCall(GameObject.Find("QUIPS2").gameObject, 3.5f, () =>
                    {
                        LeanTween.move(GameObject.Find("QUIPS2").gameObject, ogPos2, 0.5f).setEaseInQuad();
                    });
                });
                break;
        }
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void SetNameRPC(string nickname2, int matIndex2)
    {
        nicknameTemp = nickname2;
        materianIndexTemp = matIndex2;
        /*player.isHit = true;
        rb.AddForce(projectileVelocity.normalized * knockBackStrength);
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received knockback on StateAuthority, Adding force to this object" + projectileVelocity.normalized * knockBackStrength);
        NetworkedHealth -= 0.5f;
        anim.SetTrigger("isHit");
        Invoke("resetHitBool", 0.5f);*/
        this.nickName = nickname2;
        this.materialIndex = matIndex2;

        TextMeshProUGUI[] nameTexts = transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var nameText in nameTexts)
        {
            nameText.text = nickName.ToString();
        }
        //transform.GetComponentInChildren<TextMeshProUGUI>().text = nickName.ToString();

        transform.Find("PlayerMesh").GetComponent<Renderer>().material = materials[materialIndex];
    }
}
