using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HookVisualizer : MonoBehaviour
{
    public PlayerAiming boss;
    LineRenderer lr;
    private SpriteRenderer kid;

    public void Start()
    {
        kid = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
    }
    RaycastHit2D hit;
    public void Update()
    {
        if (boss == null) return;
        Vector2 target = boss.direction;
        hit = Physics2D.Raycast(boss.transform.position, target, Mathf.Infinity, boss.LayerMask);
        Debug.DrawLine(boss.transform.position, transform.position, Color.red);
        Gradient g = new Gradient();
        g.colorKeys[0] = new GradientColorKey(kid.color, 0f);
        g.colorKeys[1] = new GradientColorKey(kid.color, 1f);
        lr.startColor = kid.color;
        lr.endColor = kid.color;
      
        List<Vector3> pos = new List<Vector3>();
        pos.Add(boss.transform.position);
        pos.Add(transform.position);
        lr.SetPositions(pos.ToArray());
        if(hit.collider != null)
            transform.position = hit.point;
    }
}
