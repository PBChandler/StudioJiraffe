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
    public Animator Animator;

    private void Start()
    {
        m_button.onClick.AddListener(ButtonOnClick);
        m_button.Select();
    }
    public void ButtonOnClick()
    {
        GetComponent<Button>().enabled = false;
        Animator.SetTrigger("Pressed");
        m_button.interactable = false;
        m_Text.text = "Loading...";
        Debug.Log("Start Button Clicked");
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
