using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsRotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed;



    void Update()
    {
        float rotation = Time.time * rotateSpeed;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}
