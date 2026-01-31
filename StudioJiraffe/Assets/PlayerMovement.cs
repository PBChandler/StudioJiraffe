using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public PlayerStates m_State = PlayerStates.Regular;
    public PlayerInput player;
    public Rigidbody2D rb;
    Vector2 moveInput = Vector2.zero;
    public float moveSpeed;

    public Animator animator;
    [Header("References")]
    public PlayerMovementStats MoveStats;
    public Collider2D feetColl, bodyColl;

    private Vector2 moveVelocity;
    public bool isFacingRight;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    public bool isGrounded;
    private bool bumpedHead;

    public Vector2 lookInput;
    public bool hookingInput;
    public bool attackingInput;

    public float VerticalVelocity, fastFallTime, fastFallReleaseSpeed;
    private bool isJumping, isFastFalling, isFalling;
    private int numberOfJumpsUsed;
    //apex vars
    private float apexPoint, timePastApexThreshold;
    private bool isPastApexThreshold;

    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    private float coyoteTimer;

    public delegate void jumpInteraction();
    public jumpInteraction dg_onJumpPressed, dg_onJumpReleased;
    public bool jumpWasPressed = false;

    public Vector2 externalForce;
    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        dg_onJumpPressed += HandleOnJumpPressed;
        dg_onJumpReleased += OnJumpReleased;
    }

    private void HandleOnJumpPressed()
    {
        jumpWasPressed = true;

    }

    private void OnJumpReleased()
    {
        jumpWasPressed = false;
    }

   
    void Start()
    {
       
        player.onActionTriggered += Player_onActionTriggered;
    }

    private void Player_onActionTriggered(InputAction.CallbackContext obj)
    {
        switch (obj.action.name)
        {
            case "Move":
                ProcessMoveInput(obj);
                break;
            case "Jump":
                ProcessJumpInput(obj);
                break;
            case "Look":
                ProcessLookInput(obj);
                break;
            case "Hooking":
                ProcessHookInput(obj);
                break;
            case "Attack":
                ProcessAttackInput(obj);
                break;
            default:
                break;
        }
    }

    private void ProcessAttackInput(InputAction.CallbackContext obj)
    {
        if (obj.performed || obj.started)
        {
            attackingInput = true;
        }
        else
        {
            attackingInput= false;
        }
    }

    public void ProcessLookInput(InputAction.CallbackContext obj)
    {
        lookInput = obj.ReadValue<Vector2>();
    }

    public void ProcessHookInput(InputAction.CallbackContext obj)
    {
        if (obj.performed || obj.started)
        {
            hookingInput = true;
        }
        else
        {
           hookingInput = false;
        }
    }
    public void ProcessJumpInput(InputAction.CallbackContext obj)
    {
        if(obj.started)
        {
            dg_onJumpPressed.Invoke();
        }
        else if(obj.canceled)
        {
            dg_onJumpReleased.Invoke();
        }
    }
    public void ProcessMoveInput(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
       
        if (isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration);
        }
    }
    public void Move(float acceleration, float deceleration)
    {
        if (m_State == PlayerStates.CompletelyImmobile) return;
        //if (m_State == PlayerStates.NoControl) return;
        if ((moveInput != Vector2.zero))
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;

            targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxRunSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x + externalForce.x, rb.linearVelocity.y + externalForce.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if(turnRight)
        {
            isFacingRight = true;
            //transform.Rotate(0, 180f, 0);
        }
        else
        {
            isFacingRight = false;
            //transform.Rotate(0, -180f, 0);
        }
    }

    public void CollisionChecks()
    {
        IsGrounded();
        IsHeadBump();
    }

   

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.groundLayer);
        if(groundHit.collider != null) isGrounded = true;
        else isGrounded = false;
    }

    public void IsHeadBump()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.groundLayer);
        if (groundHit.collider != null) isGrounded = true;
        else isGrounded = false;
    }
    private void JumpChecks()
    {
        if(jumpWasPressed)
        {
            jumpBufferTimer = MoveStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        if(!jumpWasPressed)
        {
            if (jumpBufferTimer > 0)
            {
                jumpReleasedDuringBuffer = true;
            }

            if(isJumping && VerticalVelocity > 0f)
            {
                if(isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        if(jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if(jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        else if(jumpBufferTimer>0f && isJumping && numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }

        else if(jumpBufferTimer >0f && isFalling && numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        if((isJumping || isFastFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    public void InitiateJump(int naumberOfJumpsUsed)
    {
        if(!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0f;
        numberOfJumpsUsed += naumberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }
    private void Jump()
    {
        if (m_State == PlayerStates.NoControl || m_State == PlayerStates.CompletelyImmobile) return;
        if (isJumping)
        {
            //headbump check
            if(bumpedHead)
            {
                isFastFalling = true;
            }

            if(VerticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if(apexPoint > MoveStats.ApexThreshold)
                {
                    if(!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if(isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if(timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                            isFastFalling = true;
                        }
                    }
                }

                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if(isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }

            
            
        }
        else if (!isFastFalling)
        {
            VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }
        else if ((VerticalVelocity < 0))
        {
            if (!isFalling)
            {
                isFalling = true;
            }
        }

        if (isFastFalling)
        {
            if (fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            fastFallTime += Time.fixedDeltaTime;
        }

        if ((!isGrounded && !isJumping))
        {
            isFalling = true;
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }

    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = MoveStats.JumpCoyoteTime;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        CountTimers();
        JumpChecks();
    }
}

public enum PlayerStates
{
    NULL,
    Regular,
    Hooking,
    NoControl,
    CompletelyImmobile,
}