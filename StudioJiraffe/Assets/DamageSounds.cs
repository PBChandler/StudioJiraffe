using Unity.VisualScripting;
using UnityEngine;

public class DamageSounds : MonoBehaviour
{
    public AudioClip[] damageSounds;
    public PlayerHealth playerHealth;
    public AudioSource s;

    public void Start()
    {
        playerHealth.dg_onHurt += PlaySound;
        //AudioSource s = gameObject.AddComponent<AudioSource>();
       // s.volume = 0.5f;
    }

    public void PlaySound()
    {
        if (s.isPlaying) return;
        s.clip = damageSounds[Random.Range(0, damageSounds.Length)];
        s.volume = Random.Range(0.7f, 1.4f);
        if(playerHealth.playerID == 0)
        {
            s.pitch = 0.8f;
        }
        else
        {
            s.pitch = 1.1f;
        }
    }
}
