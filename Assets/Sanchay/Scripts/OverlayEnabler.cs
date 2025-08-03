using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayEnabler : MonoBehaviour
{
    public GameObject overlayCamera;

    public void enableOverlayCamera()
    {
        overlayCamera.SetActive(true);
    }
}
