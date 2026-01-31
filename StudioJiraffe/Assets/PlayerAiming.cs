using System;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public PlayerMovement pm;
    public Transform hookHand;
    Vector2 baseAim = Vector2.zero;
    Vector2 currentAim = Vector2.zero;
    public float hookSpeed;
    bool hookOut = false;
    bool startHooking = false;
    bool hookReeling = false;
    public hookStates hookState;
    RaycastHit2D hookHit;
    Vector2 hitpoint;

    public LayerMask LayerMask;

    
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    public void ProcessNotBeingUsed()
    {
        pm.m_State = PlayerStates.Regular;
        hookHand.localPosition = pm.lookInput;
        baseAim = hookHand.localPosition;
        currentAim = baseAim;
        if (pm.hookingInput)
        {
            hookState = hookStates.HOOKING;
        }
    }

    public void ProcessHooking()
    {
        pm.m_State = PlayerStates.Hooking;
        Vector2 direction = new Vector2(baseAim.x, baseAim.y);
        currentAim += direction * Time.deltaTime * hookSpeed;
        hookHand.localPosition = currentAim;
        if(!pm.hookingInput)
        {
            hookState = hookStates.LURING;
        }
        hookHit = Physics2D.CircleCast(hookHand.position, 0.4f, hookHand.transform.up, 1f, LayerMask);
        if(hookHit.collider != null)
        {
            Debug.DrawLine(transform.position, hookHit.point, Color.green);
            hookState = hookStates.LAUNCHING;
            hitpoint = hookHit.point;
        }
        else
        {
            Debug.DrawLine(transform.position, hookHand.position, Color.red);
        }
    }

    public void ProcessLuring()
    {
        pm.m_State = PlayerStates.Hooking;
        Vector2 direction = new Vector2(baseAim.x, baseAim.y);
        currentAim -= direction * Time.deltaTime * hookSpeed;
        hookHand.localPosition = currentAim;
        if (Vector2.Distance(hookHand.localPosition, Vector2.zero) < 0.5f)
        {
            hookState = hookStates.NOTBEINGUSED;
            currentAim = Vector2.zero;
        }
    }

    private void ProcessLaunching()
    {
        Vector2 direction = -(new Vector2(transform.position.x - hitpoint.x, transform.position.y - hitpoint.y));
        pm.m_State = PlayerStates.NoControl;
        pm.rb.linearVelocity = direction * 50f;
        hookHand.position = hitpoint;
        if(Vector2.Distance(transform.position, hitpoint) < 2)
        {
            pm.VerticalVelocity = 5f;
            pm.jumpWasPressed = true;
            hookState = hookStates.NOTBEINGUSED;
            currentAim = Vector2.zero;
            pm.jumpWasPressed = false;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        switch (hookState)
        {
            case hookStates.NOTBEINGUSED:
                ProcessNotBeingUsed();
                break;
            case hookStates.HOOKING:
                ProcessHooking();
                break;
            case hookStates.LURING:
                ProcessLuring();
                break;
            case hookStates.LAUNCHING:
                ProcessLaunching();
                break;

            default:
                break;
        }
        //if(pm.hookingInput && !hookOut && !startHooking)
        //{
        //    startHooking = true;
        //    hookOut = true;
        //}

        //if(pm.m_State != PlayerStates.NoControl && !pm.hookingInput && !hookOut)
        //{
        //    pm.m_State = PlayerStates.Regular;
        //    hookHand.localPosition = pm.lookInput;
        //    baseAim = hookHand.localPosition;
        //    currentAim = baseAim;
        //}
        //if(!pm.hookingInput && hookOut)
        //{
        //    hookReeling = true;
        //    Vector2 direction = new Vector2(baseAim.x, baseAim.y);
        //    currentAim -= direction * Time.deltaTime * hookSpeed;
        //    hookHand.localPosition = currentAim;
        //    if(Vector2.Distance(hookHand.localPosition, Vector2.zero) < 0.5f)
        //    {
        //        hookReeling = false;
        //        hookOut = true;
        //        startHooking = false;
        //        currentAim = Vector2.zero;
        //    }
        //}

        //if(startHooking)
        //{
        //    pm.m_State = PlayerStates.Hooking;
        //    Vector2 direction = new Vector2(baseAim.x, baseAim.y);
        //    currentAim += direction * Time.deltaTime * hookSpeed;
        //    hookHand.localPosition = currentAim;
        //    hookOut = true;
        //}
    }


    public enum hookStates
    {
        NOTBEINGUSED,
        HOOKING,
        LURING,
        LAUNCHING,
    }
}
