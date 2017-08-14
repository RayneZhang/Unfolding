using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;
using System.IO;
using System;

public class PlayerControl : MonoBehaviour {

    public MeshGenerator meshGenerator;
    public GameObject WinCanvas;
    public GameObject LoseCanvas;
    public GameObject UserCanvas;

    private string path = "Assets/_Results/result_1.txt";

    [HideInInspector]
    public bool unfolding = false;
    bool unfoldA = false;
    bool unfoldB = false;

    List<Vector3> WaitingLines;

    // Use this for initialization
    void Start () {
        //TargetPositions = new List<Vector3>();
        WinCanvas.SetActive(false);
        LoseCanvas.SetActive(false);
        
        WaitingLines = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!unfolding && WaitingLines.ToArray().Length != 0)
        {
            Vector3 midPoint = WaitingLines[0];
            WaitingLines.RemoveAt(0);
            ClickAtLine(midPoint);
        }

        if (!unfolding && Input.GetMouseButtonDown(0)) {
            RaycastHit hitObject = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitObject) && hitObject.transform.tag == "Line")
            {
                Vector3 startingPoint = hitObject.transform.GetComponent<LineRenderer>().GetPosition(0);
                Vector3 endingPoint = hitObject.transform.GetComponent<LineRenderer>().GetPosition(1);

                //Destroy the Line.
                hitObject.transform.GetComponent<Glow>().destroyLine();

                Vector3 midPoint = (startingPoint + endingPoint) / 2;
                //Debug.Log(midPoint);

                ClickAtLine(midPoint);
            }
        }
    }

    private void ClickAtLine(Vector3 midPoint)
    {
        //Get two faces' indices of the selected line.
        List<int> faceIndices = meshGenerator.GetFaceIndicesOfLine(midPoint);

        int indexA = faceIndices[0];
        int indexB = faceIndices[1];

        //Delete the lines in the two faces. Then check if one of these faces has only one line left, if so, it needs to unfold.
        unfoldA = DeleteAndCheckLinesInFace(indexA, midPoint);
        unfoldB = DeleteAndCheckLinesInFace(indexB, midPoint);
        unfolding = unfoldA || unfoldB;
        Debug.Log(unfolding);

        if (unfoldA)
        {
            Vector3 rmStartingPoint = meshGenerator.model.faces[indexA].lineStartingPoint[0];
            Vector3 rmEndingPoint = meshGenerator.model.faces[indexA].lineEndingPoint[0];
            Vector3 rmMidPoint = meshGenerator.model.faces[indexA].linesMidpoints[0];

            DeleteLastLine(rmMidPoint);
            if (GameObject.FindGameObjectWithTag("Line") == null)
            {
                WaitingLines.Clear();
                return;
            }

            int TargetIndexofA = meshGenerator.GetTheOtherFaceIndex(rmMidPoint, indexA);
            Vector3 currentNormalofA = meshGenerator.GetNormalofFace(indexA);
            Vector3 TargetNormalofA = meshGenerator.GetNormalofFace(TargetIndexofA);

            int NumOfConnectedFaces = meshGenerator.model.faces[indexA].ConnectedFaces.ToArray().Length;
            for (int i = -1; i < NumOfConnectedFaces; i++)
            {
                int currentIndex;
                if (i == -1)
                {
                    currentIndex = indexA;
                    meshGenerator.model.faces[TargetIndexofA].Neighbors.Add(currentIndex);
                    meshGenerator.model.faces[currentIndex].Neighbors.Add(TargetIndexofA);
                }
                else
                    currentIndex = meshGenerator.model.faces[indexA].ConnectedFaces[i];

                meshGenerator.model.faces[TargetIndexofA].ConnectedFaces.Add(currentIndex);
            }



            StartUnfolding(indexA, currentNormalofA, TargetNormalofA, rmStartingPoint, rmEndingPoint);
        }
        if (unfoldB)
        {

            Vector3 rmStartingPoint = meshGenerator.model.faces[indexB].lineStartingPoint[0];
            Vector3 rmEndingPoint = meshGenerator.model.faces[indexB].lineEndingPoint[0];
            Vector3 rmMidPoint = meshGenerator.model.faces[indexB].linesMidpoints[0];

            DeleteLastLine(rmMidPoint);
            if (GameObject.FindGameObjectWithTag("Line") == null)
            {
                WaitingLines.Clear();
                return;
            }

            int TargetIndexofB = meshGenerator.GetTheOtherFaceIndex(rmMidPoint, indexB);
            Vector3 currentNormalofB = meshGenerator.GetNormalofFace(indexB);
            Vector3 TargetNormalofB = meshGenerator.GetNormalofFace(TargetIndexofB);

            int NumOfConnectedFaces = meshGenerator.model.faces[indexB].ConnectedFaces.ToArray().Length;
            for (int i = -1; i < NumOfConnectedFaces; i++)
            {
                int currentIndex;
                if (i == -1)
                {
                    currentIndex = indexB;
                    meshGenerator.model.faces[TargetIndexofB].Neighbors.Add(currentIndex);
                    meshGenerator.model.faces[currentIndex].Neighbors.Add(TargetIndexofB);
                }
                else
                    currentIndex = meshGenerator.model.faces[indexB].ConnectedFaces[i];

                meshGenerator.model.faces[TargetIndexofB].ConnectedFaces.Add(currentIndex);
            }

            StartUnfolding(indexB, currentNormalofB, TargetNormalofB, rmStartingPoint, rmEndingPoint);
        }
    }

    private void StartUnfolding(int faceIndex, Vector3 startingNormal, Vector3 endingNormal,  Vector3 startingPoint, Vector3 endingPoint)
    {
        //Debug.Log(Vector3.Dot(startingNormal, endingNormal));
        //float angle = Mathf.Acos(Vector3.Dot(startingNormal, endingNormal));

        //int vertexOffset = meshGenerator.model.faces[faceIndex].offset;
        //int vertexSize = meshGenerator.model.faces[faceIndex].vertices.ToArray().Length;

        Quaternion rotation = new Quaternion();
        rotation.SetFromToRotation(startingNormal, endingNormal);

        //Debug.Log("Starting Normal is " + startingNormal);
        //Debug.Log("Endign Normal is " + endingNormal);
        //Debug.Log("Starting Point is " + startingPoint);
        //Debug.Log("Ending Point is " + endingPoint);

        meshGenerator.StartUnfolding(faceIndex, rotation, startingNormal, endingNormal, startingPoint, endingPoint);
    }

    private bool DeleteAndCheckLinesInFace(int faceIndex, Vector3 midPoint)
    {
        Face face = meshGenerator.model.faces[faceIndex];

        if (face.linesMidpoints.ToArray().Length <= 0)
            return false;

        int index = face.linesMidpoints.IndexOf(midPoint);

        face.linesMidpoints.Remove(midPoint);
        face.lineStartingPoint.RemoveAt(index);
        face.lineEndingPoint.RemoveAt(index);
        Debug.Log(face.linesMidpoints.ToArray().Length);
        if (face.linesMidpoints.ToArray().Length != 1)
            return false;
        else
            return true;   
    }

    private void DeleteLastLine(Vector3 midPoint)
    {
        foreach(GameObject line in GameObject.FindGameObjectsWithTag("Line"))
        {
            Vector3 startingPoint = line.GetComponent<LineRenderer>().GetPosition(0);
            Vector3 endingPoint = line.GetComponent<LineRenderer>().GetPosition(1);
            if (midPoint == (startingPoint + endingPoint) / 2)
                Destroy(line);
        }

        //You need to consider a series of unfolding, which means when you delete the last line, a new unfolding happens.
        //But this can be done by deleting the line after the previous unfolding finished.

        //Solution:Push the midPoint to an List, waiting to be processed.
        WaitingLines.Add(midPoint);
    }

    public void Grading()
    {
        bool result = CheckResult();
        Debug.Log(result);
        UserCanvas.SetActive(false);

        if (result)
            WinCanvas.SetActive(true);
        else
            LoseCanvas.SetActive(true);
    }

    private bool CheckResult()
    {
        StreamReader reader = new StreamReader(path, false);
        char[] delimiterChars = { ';' };

        int NumofFaces = meshGenerator.NumofFaces;
        for(int i = 0; i < NumofFaces; i++)
        {
            //Read header.
            reader.ReadLine();
            string currentLine = reader.ReadLine();

            string[] results = currentLine.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            if (meshGenerator.model.faces[i].Neighbors.ToArray().Length != results.Length)
                return false;
            foreach(string result in results)
            {
                if (!meshGenerator.model.faces[i].Neighbors.Contains(int.Parse(result)))
                    return false;
            }
            
        }
        reader.Close();
        return true;
    }

    public void ShowResult()
    {
        int NumofFaces = meshGenerator.NumofFaces;
        for(int i = 0; i < NumofFaces; i++)
        {
            Debug.Log("Face " + i + " contains:");
            string CurrentNeighbor = "";
            foreach(int j in meshGenerator.model.faces[i].Neighbors)
            {
                CurrentNeighbor += j.ToString();
                CurrentNeighbor += ";";
            }
            Debug.Log(CurrentNeighbor);
        }
    }

    public void SaveResult()
    {
        StreamWriter writer = new StreamWriter(path, false);

        int NumofFaces = meshGenerator.NumofFaces;
        for (int i = 0; i < NumofFaces; i++)
        {
            writer.WriteLine("Face " + i + " contains:");
            string CurrentNeighbor = "";
            foreach (int j in meshGenerator.model.faces[i].Neighbors)
            {
                CurrentNeighbor += j.ToString();
                CurrentNeighbor += ";";
            }

            writer.WriteLine(CurrentNeighbor);
        }

        writer.Close();
    }

    private void DeleteAllLines()
    {
        foreach (GameObject line in GameObject.FindGameObjectsWithTag("Line"))
        {
            Destroy(line);
        }
    }

    public void Replay()
    {
        DeleteAllLines();
        meshGenerator.ReGenerate();

        WinCanvas.SetActive(false);
        LoseCanvas.SetActive(false);
        UserCanvas.SetActive(true);
    }
    
}
