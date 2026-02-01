using UnityEngine;

public class KillThePlayer : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<PlayerHealth>().Hurt(999);
    }
}
