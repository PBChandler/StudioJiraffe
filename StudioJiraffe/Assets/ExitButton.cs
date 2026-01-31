using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;


public class ExirButton : MonoBehaviour
{
    public Button m_button;
    public TextMeshProUGUI m_Text;

    private void Start()
    {
        m_button.onClick.AddListener(ButtonOnClick);
    }
    public void ButtonOnClick()
    {
        Debug.Log("Exit Button Clicked");
        Application.Quit();
    }
}
