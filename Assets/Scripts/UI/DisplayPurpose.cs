using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPurpose : MonoBehaviour
{
    [SerializeField]
    private Text purposeText;

    public void SetPurposeText(string text)
    {
        purposeText.text = text;
    }
}
