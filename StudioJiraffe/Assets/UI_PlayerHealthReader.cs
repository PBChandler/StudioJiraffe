using UnityEngine;
using TMPro;
using System.Collections;

public class UI_PlayerHealthReader : MonoBehaviour
{
    public PlayerHealth myRenderer;
    public TextMeshProUGUI fire;
    public playerFollower tiedFollower;
    bool initialized = false;

    public void Start()
    {
        
    }

    public void Update()
    {
        try
        {
            if (tiedFollower != null && !initialized)
            {
                myRenderer = tiedFollower.target.GetComponent<PlayerHealth>();
                initialized = true;
                Initialize();
            }
        }
        catch
        {

        }
        
    }
    public void Initialize()
    {
        myRenderer.dg_onHurt += UpdateText;
    }

    public void UpdateText()
    {
        fire.text = myRenderer.health+"";
        StartCoroutine(dumbo());
    }

    public IEnumerator dumbo()
    {
        float ticker = 0;
        fire.color = Color.red;
       while(ticker < 1)
       {
            fire.color = Color.Lerp(fire.color, Color.white, 0.5f);
            ticker += 0.1f;
            yield return new WaitForSeconds(0.1f);
       }
        fire.color = Color.white;
    }
}
