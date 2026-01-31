using UnityEngine;

public class HookInteractable : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerHook")
        {
            Event();
        }
    }

    public virtual void Event()
    {

    }
}
