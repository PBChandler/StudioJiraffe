using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GAME : MonoBehaviour
{
    public static GAME instance;
    public GameObject EndOfRoundObject;
    public int p1Score, p2Score;
    public Transform p1Spawnpoint, p2Spawnpoint;
    public Transform player1, player2;
    public GameObject healthBar;
    public void Start()
    {
        if (GAME.instance != null && GAME.instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
   
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
        healthBar.SetActive(false);
    }

    public void NewRound()
    {
        healthBar.SetActive(true);
        player1.transform.position = p1Spawnpoint.position;
        player2.transform.position = p2Spawnpoint.position;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        EndOfRoundObject.SetActive(false);
        player1.GetComponent<PlayerHealth>().health = 100;
        player2.GetComponent<PlayerHealth>().health = 100;
    }

    public void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            healthBar.SetActive(true);
            player1.transform.position = p1Spawnpoint.position;
            player2.transform.position = p2Spawnpoint.position;
            player1.GetComponent<PlayerMovement>().m_State = PlayerStates.Regular;
            player2.GetComponent<PlayerMovement>().m_State = PlayerStates.Regular;
            player1.GetComponent<PlayerHealth>().health = 100;
            player2.GetComponent<PlayerHealth>().health = 100;
        }
    }
    public void Restart()
    {
        p1Score = 0;
        p2Score = 0;
        player1.transform.position = p1Spawnpoint.position;
        player2.transform.position = p2Spawnpoint.position;
        player1.GetComponent<PlayerMovement>().m_State = PlayerStates.CompletelyImmobile;
        player2.GetComponent<PlayerMovement>().m_State = PlayerStates.CompletelyImmobile;
        EndOfRoundObject.SetActive(false);
        SceneManager.LoadSceneAsync("title screen");
        player1.GetComponent<PlayerHealth>().health = 100;
        player2.GetComponent<PlayerHealth>().health = 100;
    }
}
