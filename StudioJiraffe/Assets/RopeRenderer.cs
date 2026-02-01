using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HookRopeRenderer : MonoBehaviour
{
    [SerializeField] private Transform startPoint;   // player / hookHand
    [SerializeField] private PlayerAiming aiming;    // ?????

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
    }

    private void LateUpdate()
    {
        UpdateRope();
    }

    private void UpdateRope()
    {
        if (!aiming.IsHookActive)   // ?????“???? hook”???
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, aiming.HookEndPoint);
    }
}