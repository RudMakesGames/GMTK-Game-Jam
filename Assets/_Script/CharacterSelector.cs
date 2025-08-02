/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Material[] materials;
    public GameObject Player;
    public GameObject Prefab;
    public GameObject Daruma;

    public static Material selectedMat;
    public static string selectedName;

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
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import for TMP_InputField

public class CharacterSelector : MonoBehaviour
{
    public Material[] materials;
    public GameObject Player;
    public GameObject Prefab;
    public GameObject Daruma;

    public TMP_InputField nameInputField; // Reference to the TMP input field

    public static Material selectedMat;
    public static string selectedName;

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
        if (nameInputField != null)
        {
            selectedName = nameInputField.text;
            Debug.Log("Selected Name -> " + selectedName);
        }

        if (Prefab != null && materials.Length > 0)
        {
            Renderer prefabRenderer = Prefab.GetComponentInChildren<Renderer>();
            if (prefabRenderer != null)
            {
                prefabRenderer.material = materials[currentMaterialIndex];
                selectedMat = materials[currentMaterialIndex];
                Debug.Log("Selected Material -> " + selectedMat);
            }

            Player.SetActive(false);
            if (Daruma != null)
            {
                Daruma.SetActive(true);
                Renderer darumaRenderer = Daruma.GetComponent<Renderer>();
                if (darumaRenderer != null)
                    darumaRenderer.material = materials[currentMaterialIndex];
            }
        }
    }
}
