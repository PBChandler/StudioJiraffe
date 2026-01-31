using UnityEngine;
using TMPro;

public class UI_PlayerHealthReader : MonoBehaviour
{
    public PlayerHealth myRenderer;
    public TextMeshProUGUI fire;
    public void Initialize()
    {
        myRenderer.dg_onHurt += UpdateText;
    }

    public void UpdateText()
    {
        fire.text = myRenderer.health+"";
    }
}
