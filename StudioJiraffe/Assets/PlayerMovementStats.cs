using UnityEngine;

[CreateAssetMenu(menuName = "PlayerMovement")] 
public class PlayerMovementStats : ScriptableObject
{
    public float MaxWalkSpeed = 12.5f;
    public float GroundAcceleration = 5f;
    public float GroundDeceleration = 20;
    public float AirAcceleration = 5f;
    public float AirDeceleration = 5f;

    public float MaxRunSpeed = 20f;

    public LayerMask groundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    public float HeadWidth = 0.75f;

    [Header("jump")]
    public float JumpHeight = 6.5f;
    public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    public int NumberOfJumpsAllowed = 2;

    public float TimeForUpwardsCancel = 0.027f;
    public float ApexThreshold = 0.97f;
    public float ApexHangTime = 0.075f;

    public float JumpBufferTime = 0.125f;

    public float Gravity;
    public float InitialJumpVelocity;
    public float JumpCoyoteTime = 0.1f;

    public float AdjustedJumpHeight;

    public void OnValidate()
    {
        CalculateValues();
    }
    public void OnEnable()
    {
        CalculateValues();
    }
    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}
