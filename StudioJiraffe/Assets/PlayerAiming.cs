using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using TMPro;
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
    public SpringJoint2D joint;
    RaycastHit2D hookHit, hookCapture;
    Vector2 hitpoint;
    private bool aimingUp = false;
    public LayerMask LayerMask;
    public LayerMask PlayerLayerMask;
    public Transform capturedEnemy;

    public Vector2 direction;
    void Start()
    {
        joint.enabled = false;
        pm = GetComponent<PlayerMovement>();
    }

    public void ProcessNotBeingUsed()
    {
        joint.enabled = false;
        pm.externalForce = Vector2.zero;
        pm.rb.gravityScale = 0f;
        pm.rb.mass = 0.0001f;
        pm.m_State = PlayerStates.Regular;
        hookHand.localPosition = pm.lookInput;
        baseAim = hookHand.localPosition;
        currentAim = baseAim;
        direction = new Vector2(baseAim.x, baseAim.y);
        if (pm.hookingInput)
        {
            hookState = hookStates.HOOKING;
        }
    }

    public void ProcessHooking()
    {
        joint.enabled = false;
        pm.rb.gravityScale = 0f;
        pm.rb.mass = 0.0001f;
        pm.m_State = PlayerStates.Hooking;
        direction = new Vector2(baseAim.x, baseAim.y);
        currentAim += direction * Time.deltaTime * hookSpeed;
        hookHand.localPosition = currentAim;
        if(!pm.hookingInput)
        {
            hookState = hookStates.LURING;
        }
        hookHit = Physics2D.CircleCast(hookHand.position, 0.4f, hookHand.transform.up, 0.5f, LayerMask);
        hookCapture = Physics2D.CircleCast(new Vector2(hookHand.position.x + baseAim.normalized.x, hookHand.position.y + baseAim.normalized.y), 0.4f, hookHand.transform.up, 0.85f, PlayerLayerMask);
        aimingUp = pm.lookInput.y > 0.5f; 
        if(hookCapture.collider != null && hookCapture.collider.transform.parent.GetComponent<PlayerAiming>() != this && hookCapture.collider.transform.parent.parent.GetComponent<PlayerAiming>() != this)
        {
            PlayerMovement targPM = hookCapture.collider.transform.parent.parent.GetComponent<PlayerMovement>();
            targPM.externalForce = (transform.position - targPM.transform.position).normalized;
            targPM.m_State = PlayerStates.CompletelyImmobile;
            targPM.rb.linearVelocity = Vector2.zero;
            capturedEnemy = targPM.transform;
            targPM.GetComponent<PlayerHealth>().Hurt(10);
            hookState = hookStates.LURING;
            return;
        }
        if(hookHit.collider != null)
        {
            Debug.DrawLine(transform.position, hookHit.point, Color.green);
            hookState = hookStates.LAUNCHING;
            if (aimingUp)
                hitpoint = hookHit.point + new Vector2(0, 5);
            else
                hitpoint = hookHit.point;
        }
        else
        {
            Debug.DrawLine(transform.position, hookHand.position, Color.red);
        }
    }

    public void ProcessLuring()
    {
        joint.enabled = false;
        pm.m_State = PlayerStates.Hooking;
        direction = new Vector2(baseAim.x, baseAim.y);
        currentAim -= direction * Time.deltaTime * hookSpeed;
        hookHand.localPosition = currentAim;
 
        if (Vector2.Distance(hookHand.localPosition, Vector2.zero) < 0.5f)
        {
            if(capturedEnemy == null)
            {
                hookState = hookStates.NOTBEINGUSED;
                currentAim = Vector2.zero;
            }
            else
            {
                hookState = hookStates.HITSTATE;
            }
           
        }

        if (capturedEnemy != null)
        {
            capturedEnemy.position = Vector3.Lerp(capturedEnemy.transform.position, transform.position, 5 * Time.deltaTime); 
        }
    }
    private float airtime = 0;
    private void ProcessLaunching()
    {
        airtime += Time.deltaTime;
        joint.enabled = true;   
        direction = -(new Vector2(transform.position.x - hitpoint.x, transform.position.y - hitpoint.y));
        pm.m_State = PlayerStates.NoControl;
        pm.rb.linearVelocity = direction * 5f;
        pm.externalForce = direction * 5f;
        if (aimingUp)
            hookHand.position = hitpoint - new Vector2(0, 5);
        else
            hookHand.position = hitpoint;
        joint.anchor = hitpoint;
        joint.distance = new Vector2(hitpoint.x - transform.position.x, hitpoint.y - transform.position.y).magnitude;
        joint.connectedAnchor = hitpoint;
        pm.rb.gravityScale = 1;
        pm.rb.mass = 9.2f;
        if(Vector2.Distance(transform.position, hitpoint) < 2 || airtime > 0.5f)
        {
            pm.VerticalVelocity = 5f;
            pm.jumpWasPressed = true;
            hookState = hookStates.NOTBEINGUSED;
            currentAim = Vector2.zero;
            pm.jumpWasPressed = false;
            airtime = 0;
        }
        
    }

    public void ProcessHitting()
    {
        joint.enabled = false;
        GetComponent<SpringJoint2D>().enabled = false;
        pm.externalForce = Vector2.zero;
        pm.rb.linearVelocity = Vector2.zero;
        pm.rb.gravityScale = 0f;
        pm.rb.mass = 0.0001f;
        pm.m_State = PlayerStates.Regular;
        hookHand.localPosition = pm.lookInput;
        pm.m_State = PlayerStates.CompletelyImmobile;
        if(pm.attackingInput)
        {
            pm.m_State = PlayerStates.Regular;
            capturedEnemy.GetComponent<PlayerMovement>().m_State = PlayerStates.NoControl;
            capturedEnemy.GetComponent<PlayerMovement>().externalForce = (new Vector2(-baseAim.x * 5f, -baseAim.y + 1f))* 50f;
            Invoke("closedown", 0.1f);
        }
    }

    void closedown()
    {
        capturedEnemy = null;
        hookState = hookStates.NOTBEINGUSED;
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
            case hookStates.HITSTATE:
                ProcessHitting();
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
        HITSTATE,
    }
}
