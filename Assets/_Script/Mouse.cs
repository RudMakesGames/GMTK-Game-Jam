using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mouse : NetworkBehaviour
{

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Transform orientation;
    public Transform cameraHolder; // Assign the camera or its pivot here (used for vertical look)
    public CinemachineVirtualCamera MainCamera, ADSCamera;

    public float rotationY; // Yaw (left/right)
    public float rotationX; // Pitch (up/down)

    [Header("Shooting")]
    public GameObject Projectile;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireCooldown = 0.2f;
    private bool canFire = true;

    public float mouseX, mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //if (!Object.HasInputAuthority) return;

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

    public void assignReferences()
    {
        MainCamera = transform.Find("Main Follow Camera").GetComponent<CinemachineVirtualCamera>();
        ADSCamera = transform.Find("ADS Follow Camera ").GetComponent<CinemachineVirtualCamera>();
    }

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
            ADSCamera.Priority = 1;
            MainCamera.Priority = 0;
            Debug.Log("Started Aiming");
        }
        else if (context.canceled)
        {
            ADSCamera.Priority = 0;
            MainCamera.Priority = 1;
            Debug.Log("Stopped Aiming");
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        //if (!Object.HasInputAuthority) return;
        if (context.started && canFire)
        {
            StartCoroutine(FireWithCooldown());
        }
    }

    private IEnumerator FireWithCooldown()
    {
        canFire = false;

        if (Projectile != null && firePoint != null && Object.HasStateAuthority)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(firePoint.forward) * Quaternion.Euler(0, 90, 90);
            //GameObject spawnedProjectile = Instantiate(Projectile, firePoint.position, projectileRotation);
            NetworkObject spawnedProjectile = Runner.Spawn(Projectile, firePoint.position, projectileRotation);

            Cinemachine.CinemachineImpulseSource source = spawnedProjectile.GetComponent<Cinemachine.CinemachineImpulseSource>();
            source.GenerateImpulse(Camera.main.transform.forward);
            Rigidbody rb = spawnedProjectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }
}
