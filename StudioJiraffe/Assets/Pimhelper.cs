using UnityEngine;
using UnityEngine.InputSystem;

public class Pimhelper : MonoBehaviour
{
    public PlayerInputManager chiller;
    public playerFollower follower1, follower2;
    public Transform theGrouper;
    public HookVisualizer h1, h2;
    public void Awake()
    {
        chiller = GetComponent<PlayerInputManager>();
        chiller.onPlayerJoined += Chiller_onPlayerJoined;
    }

    private void Chiller_onPlayerJoined(PlayerInput obj)
    {
        obj.transform.parent = theGrouper;
        if(follower1.occupied == false)
        {
            follower1.occupied = true;
            follower1.target = obj.transform;
            h1.boss = obj.GetComponent<PlayerAiming>();
            return;
        }
        else
        {
            h2.boss = obj.GetComponent<PlayerAiming>();
            obj.GetComponent<PlayerMovement>().animator.SetBool("player2", true);
            follower2.occupied = true;
            follower2.target = obj.transform;
        }
    }
}
