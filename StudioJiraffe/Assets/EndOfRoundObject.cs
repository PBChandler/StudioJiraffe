using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EndOfRoundObject : MonoBehaviour
{
    public TextMeshProUGUI p1, p2;
    public Button b;

    public void Call(int id)
    {
        p1.text = id == 1 ? "WINNER!" : "LOSER :(";
        p1.text += " " + GAME.instance.p1Score;
        p2.text = id == 1 ? "LOSER :(" : "WINNER!";
        p2.text += " " + GAME.instance.p2Score;
        b.Select();
    }
}
