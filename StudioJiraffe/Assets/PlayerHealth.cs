using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public delegate void onHurt();
    public onHurt dg_onHurt;
    public void Hurt(float damage)
    {
        health -= damage;
        dg_onHurt?.Invoke();
    }
}
