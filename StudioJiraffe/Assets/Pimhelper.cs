using UnityEngine;
using UnityEngine.InputSystem;

public class Pimhelper : MonoBehaviour
{
    PlayerInputManager chiller;
    public Transform theGrouper;
    public void Awake()
    {
        chiller = GetComponent<PlayerInputManager>();
        chiller.onPlayerJoined += Chiller_onPlayerJoined;
    }

    private void Chiller_onPlayerJoined(PlayerInput obj)
    {
        obj.transform.parent = theGrouper;
    }
}
