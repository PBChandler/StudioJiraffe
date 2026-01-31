using UnityEngine;

public class hookhandcardoorhookhand : MonoBehaviour
{
    public Color p1, p2;
    SpriteRenderer yarg;
    public PlayerAiming playerAim;

    public void Start()
    {
        yarg = GetComponent<SpriteRenderer>();
    }

    public Animator anim;

    public void Update()
    {
        switch (playerAim.hookState)
        {
            case PlayerAiming.hookStates.NOTBEINGUSED: //aiming/not being used
                break;
            case PlayerAiming.hookStates.HOOKING: //going out
                break;
            case PlayerAiming.hookStates.LURING: //going back to you
                break;
            case PlayerAiming.hookStates.LAUNCHING: //flinging player
                break;
            case PlayerAiming.hookStates.HITSTATE: //hitting the enemy
                break;
            default:
                break;
        }
        if(anim.GetBool("player2"))
        {
            yarg.color = p2;
        }
        else
        {
            yarg.color = p1;
        }
    }
}
