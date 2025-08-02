using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Material[] materials;
    public GameObject Player;
    public GameObject Prefab;
    public GameObject Daruma;

    public static Material selectedMat;

    private int currentMaterialIndex = 0;
    private Renderer playerRenderer;

    private void Start()
    {
        if (Player != null)
        {
            playerRenderer = Player.GetComponentInChildren<Renderer>();
            if (playerRenderer != null && materials.Length > 0)
            {
                ApplyMaterial();
            }
        }
    }

    public void NextMaterial()
    {
        if (materials.Length == 0 || playerRenderer == null)
            return;

        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        playerRenderer.material = materials[currentMaterialIndex];
    }

    public void ConfirmSelection()
    {
        if (Prefab != null && materials.Length > 0)
        {
            Renderer prefabRenderer = Prefab.GetComponentInChildren<Renderer>();
            if (prefabRenderer != null)
            {
                prefabRenderer.material = materials[currentMaterialIndex];
                selectedMat = materials[currentMaterialIndex];
                Debug.Log("selected mat->" + selectedMat);
            }
            Player.SetActive(false);
            Daruma?.SetActive(true);
            Renderer DarumaRenderer = Daruma.GetComponent<Renderer>();
            DarumaRenderer.material = materials[currentMaterialIndex];
        }
    }

}
