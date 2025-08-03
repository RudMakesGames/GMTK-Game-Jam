using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayCastScript : MonoBehaviour
{

    [Header("Raycast Settings")]
    public float radius = 9f;
    public int pointCount = 6;
    public float raycastHeight = 6f;
    public LayerMask groundLayer;

    [Header("Output")]
    public List<Vector3> validGroundPoints = new List<Vector3>();
    public bool drawDebug = true;

    public bool updatePos=true;

    private void Start()
    {
        groundLayer = GetComponent<NetworkPlayer>().groundLayer;
    }

    private void Update()
    {
        RaycastFromCircle();
        if (validGroundPoints.Count > 0) updatePos = true;
        else updatePos = false;
    }

    void RaycastFromCircle()
    {
        validGroundPoints.Clear(); 

        Vector3 center = transform.position;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pointCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 rayOrigin = center + offset + Vector3.up * raycastHeight;

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastHeight * 2, groundLayer))
            {
                validGroundPoints.Add(hit.point);

                if (drawDebug)
                {
                    Debug.DrawLine(rayOrigin, hit.point, Color.green);
                    Debug.DrawRay(hit.point, Vector3.up * 0.2f, Color.green);
                }
            }
            else if (drawDebug)
            {
                Debug.DrawRay(rayOrigin, Vector3.down * raycastHeight * 2, Color.red);
            }
        }
    }


}
