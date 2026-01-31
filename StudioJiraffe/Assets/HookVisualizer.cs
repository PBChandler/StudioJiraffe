using UnityEngine;

public class HookVisualizer : MonoBehaviour
{
    public PlayerAiming boss;
    RaycastHit2D hit;
    public void Update()
    {
        Vector2 target = boss.direction;
        hit = Physics2D.Raycast(boss.transform.position, target, Mathf.Infinity, boss.LayerMask);
        Debug.DrawLine(boss.transform.position, transform.position, Color.red);
        if(hit.collider != null)
            transform.position = hit.point;
    }
}
