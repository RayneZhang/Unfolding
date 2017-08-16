using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeText : MonoBehaviour {

    public float timing = 0.05f;

    Text txt;
    string story;


	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
        story = txt.text;
        txt.text = "";

        StartCoroutine("PlayText");
	}
	
	IEnumerator PlayText()
    {
        foreach(char c in story)
        {
            txt.text += c;
            yield return new WaitForSeconds(timing);
        }
    }
}
