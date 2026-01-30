using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public PlayerInput player;
    void Start()
    {
        player.onActionTriggered += Player_onActionTriggered;
    }

    private void Player_onActionTriggered(InputAction.CallbackContext obj)
    {
        switch (obj.action.name)
        {
            case "Move":
                Debug.Log("Success");
                break;
            default:
                break;
        }
    }

    public void ProcessMoveInput()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
