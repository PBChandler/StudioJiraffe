using System.Collections;
using UnityEngine;

public class bomb : HookInteractable
{
    public float distance;
    [SerializeField]
    ParticleSystem myParticles;
  
    public override void EnterEvent(Collider2D other, Rigidbody2D myRigidbody, Collider2D myCollider)
    {
        //if (other.gameObject.tag == "PlayerHook")
        //{
        //myRigidbody.bodyType = RigidbodyType2D.Kinematic ;

        //Transform child = other.gameObject.transform.GetChild(1);
        transform.position = other.transform.position;
        transform.SetParent(other.transform);
        transform.localPosition = new Vector2(0f, distance);

        //}
        //myCollider.isTrigger = true; 

        base.EnterEvent(other, myRigidbody, myCollider);

        

    }

    public override void ExitEvent(Collider2D other, Rigidbody2D myRigidbody)
    {
        //StartCoroutine(explode(myRigidbody));
        

        base.ExitEvent(other, myRigidbody);

    }
    
    //IEnumerator explode (Rigidbody2D myRigidbody)
    //{

    //    yield return new WaitForSeconds(1f);
    //    transform.parent = null;
    //    myRigidbody.bodyType = RigidbodyType2D.Dynamic;
    //    myParticles.enableEmission = true;
    //    Destroy(gameObject); 
    //}
}
