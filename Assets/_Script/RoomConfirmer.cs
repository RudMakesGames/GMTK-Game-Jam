using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomConfirmer : MonoBehaviour
{
    [SerializeField]
    private GameObject ConfirmButton;
    [SerializeField]
    public TMP_InputField NameField, RoomField;
    void Update()
    {
        bool nameFilled = !string.IsNullOrWhiteSpace(NameField.text);
        bool roomFilled = !string.IsNullOrWhiteSpace(RoomField.text);

        ConfirmButton.SetActive(nameFilled || roomFilled);
    }
}
