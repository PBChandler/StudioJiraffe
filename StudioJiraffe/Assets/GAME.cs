using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GAME : MonoBehaviour
{
    public static GAME instance;
    public GameObject EndOfRoundObject;
    int p1Score, p2Score;
    public void Start()
    {
        if (GAME.instance != null && GAME.instance != this)
            Destroy(this);
        else
            instance = this;
    }
    public void EndOfRound(int player)
    {
        //the player is the player who LOST (0 is red guy, 1 is blue guy)
        //add your code for the game being done (summoning the loss screen UI)
        switch (player)
        {
            case 0:
                p2Score++;
                break;
            case 1: 
                p1Score++;
                break;
            default:
                break;
        }
        EndOfRoundObject.SetActive(true);
        EndOfRoundObject.GetComponent<EndOfRoundObject>().Call(player);
    }

    public void NewRound()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync("title screen");
    }
}
