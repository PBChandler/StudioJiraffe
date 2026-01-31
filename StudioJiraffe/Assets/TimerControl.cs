using UnityEngine;
using TMPro;
using System.Collections;

public class TimerControl : MonoBehaviour
{
    public float remainingTime;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownTimer;
    public float countdownTime;

    bool roundStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownTime = 3;
        StartCoroutine(NewRound());
    }

    // Update is called once per frame
    void Update()
    {
        if (roundStarted)
        {
            if (remainingTime > 0)
            {
                remainingTime = remainingTime - Time.deltaTime;
            }
            else
            {
                remainingTime = 0;
                countdownTimer.enabled = true;
                countdownTimer.text = "Finished";
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }

    IEnumerator NewRound()
    {
        countdownTimer.text = "READY?";

        yield return new WaitForSeconds(1f);


        timerText.enabled = false;
        countdownTimer.enabled = true;

        while (countdownTime > 0)
        {
            countdownTimer.text = countdownTime.ToString();

            yield return new WaitForSeconds(1f);

            countdownTime--; 
        }

        countdownTimer.text = "BEGIN";
        yield return new WaitForSeconds(1f);

        countdownTimer.enabled = false;
        timerText.enabled = true;
        remainingTime = 60;
        roundStarted = true;


    }
}
