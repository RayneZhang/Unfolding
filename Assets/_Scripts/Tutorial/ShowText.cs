using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowText : MonoBehaviour {

    public float timing = 5.0f;
    Text txt;

	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
        
        StartCoroutine("PlayText");
    }
	
	IEnumerator PlayText()
    {
        yield return new WaitForSeconds(timing);
        txt.text = "";
    }
}
