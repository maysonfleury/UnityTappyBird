using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour {

    Text countdown;

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;
	
    // When the CountdownPage is set to active, this method runs a countdown to start the game
	void OnEnable() {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
	}

    IEnumerator Countdown()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1); // Similar to sleep(1000);
        }

        OnCountdownFinished();
    }
}
