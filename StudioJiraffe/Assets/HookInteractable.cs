using System;
using Unity.Cinemachine;
using UnityEngine;

public class HookInteractable : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    public Rigidbody2D releaseWindow;
    private Collider2D myCollider; 

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerHook")
        {
            
            EnterEvent(collision, myRigidbody, myCollider);
        }

        
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Body")
        {
            ExitEvent(collision, myRigidbody);
        }
    }

    public virtual void EnterEvent(Collider2D collision, Rigidbody2D myRigidbody, Collider2D mycollider)
    {
        
    }

    public virtual void ExitEvent(Collider2D collision, Rigidbody2D myRigidbody)
    {

    }
}
