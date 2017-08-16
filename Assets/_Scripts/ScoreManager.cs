using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    Text txt;

	// Use this for initialization
	void Start () {
        //txt = GetComponent<Text>();
	}
	
	public void SetScore(int score)
    {
        txt = GetComponent<Text>();
        txt.text = "Score: " + score;
    }
}
