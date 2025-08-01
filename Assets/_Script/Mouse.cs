using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;

public class Mouse : NetworkBehaviour
{

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Transform orientation;
    public Transform cameraHolder; // Assign the camera or its pivot here (used for vertical look)
    public CinemachineVirtualCamera MainCamera, ADSCamera;
    public Transform mainCam;

    public float rotationY; // Yaw (left/right)
    public float rotationX; // Pitch (up/down)

    [Header("Shooting")]
    public GameObject Projectile;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireCooldown = 0.2f;
    private bool canFire = true;

    public float mouseX, mouseY;

    NetworkPlayer player;

    private void Awake()
    {
        player = GetComponent<NetworkPlayer>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f); // Clamp vertical look

        /*transform.rotation = Quaternion.Euler(0f, rotationY, 0f); // Rotate player body
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, rotationY, 0f); // For movement direction
        }

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Rotate camera up/down
        }*/
    }

    /*public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //movement
        networkInputData.moveInput = moveInputVector;
        networkInputData.mouseInput.x = mouseInputScript.rotationY;
        networkInputData.mouseInput.y = mouseInputScript.rotationX;

        //jump
        *//*if (isJumping)
        {
            networkInputData.isJumping = isJumping;
        }*//*

        networkInputData.isJumping = isJumping;
        networkInputData.camRotY = camRotTemp;



        return networkInputData;
    }
*/
    /*public override void Spawned()
    {
        if(Object.HasInputAuthority)
        {
            MainCamera = transform.Find("Main Follow Camera").GetComponent<CinemachineVirtualCamera>();
            ADSCamera = transform.Find("ADS Follow Camera ").GetComponent<CinemachineVirtualCamera>();
        }
    }*/

    /*public void assignReferences()
    {
        MainCamera = transform.Find("Main Follow Camera").GetComponent<CinemachineVirtualCamera>();
        ADSCamera = transform.Find("ADS Follow Camera ").GetComponent<CinemachineVirtualCamera>();
    }*/

    /*public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            *//*transform.rotation = Quaternion.Euler(0f, rotationY, 0f); // Rotate player body
            if (orientation != null)
            {
                orientation.rotation = Quaternion.Euler(0f, rotationY, 0f); // For movement direction
            }

            if (cameraHolder != null)
            {
                cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Rotate camera up/down
            }*//*
        }
    }*/

    public void AimDownSights(InputAction.CallbackContext context)
    {
        //if (!Object.HasInputAuthority) return;


        if (context.started)
        {
            player.isAiming = true;
            
        }
        else if (context.canceled)
        {
            player.isAiming = false;
            
        }
    }


    public void adsReal(bool aiming)
    {
        if(Object.HasInputAuthority)
        if(aiming)
        {
            ADSCamera.Priority = 1;
            MainCamera.Priority = 0;
            
        }
        else
        {
            ADSCamera.Priority = 0;
            MainCamera.Priority = 1;
           
        }
    }    

    public void Fire(InputAction.CallbackContext context)
    {
        //if (!Object.HasInputAuthority) return;
        if (context.started && canFire)
        {
            player.isFiring = true;
            //StartCoroutine(FireWithCooldown());
        }
    }

    public void FireReal()
    {
        StartCoroutine(FireWithCooldown());
    }

    private IEnumerator FireWithCooldown()
    {
        canFire = false;

        NetworkObject spawnedProjectile = null;

        if (Projectile != null && firePoint != null && Object.HasStateAuthority)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(firePoint.forward) * Quaternion.Euler(0, 90, 90);
            //GameObject spawnedProjectile = Instantiate(Projectile, firePoint.position, projectileRotation);
            //NetworkObject spawnedProjectile = Runner.Spawn(Projectile, firePoint.position, projectileRotation);

            spawnedProjectile = Runner.Spawn(Projectile, firePoint.position, projectileRotation, Object.InputAuthority);
            ProjectileRicochet ricochet = spawnedProjectile.GetComponent<ProjectileRicochet>();
            Rigidbody rb = spawnedProjectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileSpeed;
            }

            CinemachineImpulseSource source = spawnedProjectile.GetComponent<CinemachineImpulseSource>();
            source.GenerateImpulse(Camera.main.transform.forward);


            //if(Object.HasInputAuthority)
            //source.GenerateImpulse(mainCam.forward);
        }

        /*if(Object.HasInputAuthority)
        {
            *//*CinemachineImpulseSource source = spawnedProjectile.GetComponent<CinemachineImpulseSource>();
            source.GenerateImpulse(Camera.main.transform.forward);*//*

        }*/

        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}
