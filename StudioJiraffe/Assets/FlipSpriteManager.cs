using UnityEngine;

public class FlipSpriteManager : MonoBehaviour
{
    public PlayerMovement flip;
    private SpriteRenderer twod;

    public void Start()
    {
        twod = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        twod.flipX = !flip.isFacingRight;
    }
}
