using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBarControl : MonoBehaviour {

    public GameObject TutorialCanvas;
    public GameObject UserCanvas;
    public MeshGenerator meshgenerator;
    public GameObject gameObj;
    public GameObject[] Texts = new GameObject[5];

    public int TotalPageNum = 5;
    public int MinPageNum = 1;
    private int CurrentPageNum = 0;

    Text txt;
    // Use this for initialization
    void Start () {
        // Deactive the gameobject first.
        gameObj.SetActive(false);

        CurrentPageNum = MinPageNum;
        txt = GetComponent<Text>();
        txt.text = CurrentPageNum + "/" + TotalPageNum;
    }
	
	public void RightClick()
    {
        int previousNum = CurrentPageNum;

        CurrentPageNum++;
        CurrentPageNum = CurrentPageNum > TotalPageNum ? TotalPageNum : CurrentPageNum;
        txt.text = CurrentPageNum + "/" + TotalPageNum;

        if(previousNum != CurrentPageNum)
        {
            LoadText(previousNum);
        }

    }

    public void LeftClick()
    {
        int previousNum = CurrentPageNum;

        CurrentPageNum--;
        CurrentPageNum = CurrentPageNum < MinPageNum ? MinPageNum : CurrentPageNum;
        txt.text = CurrentPageNum + "/" + TotalPageNum;

        if (previousNum != CurrentPageNum)
        {
            LoadText(previousNum);
        }
    }

    private void LoadText(int previousPageNum)
    {
        Texts[previousPageNum - 1].SetActive(false);
        Texts[CurrentPageNum - 1].SetActive(true);

        // Texting session.
        if(CurrentPageNum == MinPageNum)
        {
            gameObj.SetActive(false);
        }
        // Unfolding session.
        else if(CurrentPageNum == TotalPageNum - 1)
        {
            gameObj.SetActive(true);
            meshgenerator.ReGenerate(0);
        }
        else if (CurrentPageNum == TotalPageNum)
        {
            TutorialCanvas.SetActive(false);
            UserCanvas.SetActive(true);
            gameObj.SetActive(true);
            meshgenerator.ReGenerate(3);
        }
        // Practice session.
        else
        {
            gameObj.SetActive(true);
        }
        

    }
}
