using UnityEngine;

public class playerFollower : MonoBehaviour
{
    public bool occupied;
    public Transform target;
    public void Awake()
    {
        
    }

    public void Update()
    {
        if(target != null)
            transform.position = Vector3.Lerp(transform.position, target.position, 25 * Time.deltaTime);
    }
}
