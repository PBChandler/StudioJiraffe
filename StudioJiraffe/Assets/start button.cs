using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;


public class startbutton : MonoBehaviour
{
    public Button m_button;
    public TextMeshProUGUI m_Text;

    private void Start()
    {
        m_button.onClick.AddListener(ButtonOnClick);
    }
    public void ButtonOnClick()
    {
        SceneManager.LoadScene("SampleScene");
        m_button.interactable = false;
        m_Text.text = "Loading...";
        Debug.Log("Start Button Clicked");
    }
}
