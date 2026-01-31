using UnityEngine;
using TMPro;
public class EndOfRoundObject : MonoBehaviour
{
    public TextMeshProUGUI p1, p2;

    public void Call(int id)
    {
        p1.text = id == 1 ? "WINNER!" : "LOSER :(";
        p2.text = id == 1 ? "LOSER :(" : "WINNER!";
    }
}
