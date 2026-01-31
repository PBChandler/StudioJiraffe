using UnityEngine;

public class hookhandcardoorhookhand : MonoBehaviour
{
    public Color p1, p2;
    SpriteRenderer yarg;

    public void Start()
    {
        yarg = GetComponent<SpriteRenderer>();
    }

    public Animator anim;

    public void Update()
    {
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
