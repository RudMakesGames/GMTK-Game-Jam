using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AISelector : MonoBehaviour
{
    public TextMeshProUGUI AItext;
    public int AInumber = 1;

    public void IncreaseNumber()
    {
        if(AInumber < 6)
        {
            AInumber++;
        }

    }

    public void DecreaseNumber()
    {
        if (AInumber > 1)
        {
            AInumber--;
        }
 
    }

    private void Update()
    {
        AItext.text = AInumber.ToString();
    }
}
