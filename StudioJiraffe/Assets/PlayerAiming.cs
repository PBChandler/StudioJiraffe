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
    bool hookStarted = false;
    Vector2 lastGoodAim;
    public Vector2 direction;
    public bool soundCooldownAttack;
    public AudioClip attackSound;
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
        lastGoodAim = direction;
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
            PlayerMovement targPM;
            if (GetComponent<PlayerHealth>().playerID == 0)
            {
                Debug.Log("BUT NO ONE");
                targPM = GAME.instance.player2.GetComponent<PlayerMovement>();
            }
            else
            {
                targPM = GAME.instance.player1.GetComponent<PlayerMovement>();
            }

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
        capturedEnemy.GetComponent<PlayerMovement>().m_State = PlayerStates.CompletelyImmobile;
        if (GetComponent<PlayerHealth>().playerID == 0)
        {
            pm.animator.Play("attack");
        }
        else
        {
            pm.animator.Play("blue_attack1");
        }
        if (pm.attackingInput)
        {
            PlayerMovement targPM;
            if (GetComponent<PlayerHealth>().playerID == 0)
            {
                targPM = GAME.instance.player2.GetComponent<PlayerMovement>();
                LaunchPlayer(targPM);
                if (GetComponent<PlayerHealth>().playerID == 0)
                {
                    pm.animator.Play("attack2");
                }
            }
            else
            {
                targPM = GAME.instance.player1.GetComponent<PlayerMovement>();
                LaunchPlayer(targPM);
                if (GetComponent<PlayerHealth>().playerID == 0)
                {
                    pm.animator.Play("blue_attack2");
                }
            }
                Debug.Log("burgerking");
            pm.m_State = PlayerStates.Regular;
            
            Invoke("closedown", 0.1f);
            Invoke("modern", 1f);
        }
    }

    public void LaunchPlayer(PlayerMovement targPM)
    {
        targPM.m_State = PlayerStates.CompletelyImmobile;
        Vector3 target = new Vector3(transform.position.x + (-baseAim * 5f).x, transform.position.y + (-baseAim * 5f).y);
       targPM.transform.position = Vector3.Lerp(targPM.transform.position,target+new Vector3(0,5f,0), 2f);
        if(!soundCooldownAttack)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
            soundCooldownAttack = true;
            Invoke("resetSound", 0.1f);
        }
        
        // targPM.externalForce = (new Vector2(-lastGoodAim.x * 5f, -lastGoodAim.y + 5f)) * 50f;
        // targPM.rb.linearVelocity = (new Vector2(-baseAim.x * 5f, -baseAim.y + 1f)) * 50f;
    }

    public void resetSound()
    {
        soundCooldownAttack = false;
    }
    void closedown()
    {
        capturedEnemy = null;
        hookState = hookStates.NOTBEINGUSED;
    }

    public void modern()
    {
        if (GetComponent<PlayerHealth>().playerID == 0)
        {
            pm.animator.Play("player1blendrtree");
        }
        else
        {
            pm.animator.Play("player2blendtree");
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

    public bool IsHookActive =>
    hookState != hookStates.NOTBEINGUSED;

    public Vector3 HookEndPoint
    {
        get
        {
            switch (hookState)
            {
                case hookStates.HOOKING:
                    return transform.position + (Vector3)currentAim;
                case hookStates.LURING:
                    return transform.position + (Vector3)currentAim;
                case hookStates.HITSTATE:
                    if(capturedEnemy == null) return transform.position;
                    else
                        return capturedEnemy.position;
                case hookStates.LAUNCHING:
                    return hitpoint;
                default:
                    return transform.position;
            }
        }
    }
}
