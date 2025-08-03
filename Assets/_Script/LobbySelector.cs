using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbySelector : MonoBehaviour
{
    public TextMeshProUGUI AItext;
    public int LobbyNumber = 1;

    public void IncreaseNumber()
    {
        if (LobbyNumber < 6)
        {
            LobbyNumber++;
        }

    }

    public void DecreaseNumber()
    {
        if (LobbyNumber > 1)
        {
            LobbyNumber--;
        }

    }

    private void Update()
    {
        AItext.text = LobbyNumber.ToString();
    }
}
