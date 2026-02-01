using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public delegate void onHurt();
    public onHurt dg_onHurt;
    public hookhandcardoorhookhand myHook;
    public int playerID;
    public void Hurt(float damage)
    {
        health -= damage;
        dg_onHurt?.Invoke();
        if(health <= 0)
        {
            GAME.instance.EndOfRound(playerID);
        }
    }

    

}
