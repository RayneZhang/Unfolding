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

    //List<Vector3> TargetPositions;

    // Use this for initialization
    void Start () {
        //TargetPositions = new List<Vector3>();
        WinCanvas.SetActive(false);
        LoseCanvas.SetActive(false);
        UserCanvas.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
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
                Debug.Log(midPoint);
                //Get two faces' indices of the selected line.
                List<int> faceIndices = meshGenerator.GetFaceIndicesOfLine(midPoint);

                int indexA = faceIndices[0];
                int indexB = faceIndices[1];

                //Delete the lines in the two faces. Then check if one of these faces has only one line left, if so, it needs to unfold.
                unfoldA = DeleteAndCheckLinesInFace(indexA, midPoint);
                unfoldB = DeleteAndCheckLinesInFace(indexB, midPoint);
                unfolding = unfoldA || unfoldB;

                int TargetIndexofA = -1;
                int TargetIndexofB = -1;
                Vector3 currentNormalofA = new Vector3();
                Vector3 currentNormalofB = new Vector3();
                Vector3 TargetNormalofA = new Vector3();
                Vector3 TargetNormalofB = new Vector3();

                if (unfoldA)
                {
                    
                    Vector3 rmStartingPoint = meshGenerator.model.faces[indexA].lineStartingPoint[0];
                    Vector3 rmEndingPoint = meshGenerator.model.faces[indexA].lineEndingPoint[0];
                    Vector3 rmMidPoint = meshGenerator.model.faces[indexA].linesMidpoints[0];

                    DeleteLastLine(rmMidPoint);

                    TargetIndexofA = meshGenerator.GetTheOtherFaceIndex(rmMidPoint, indexA);
                    currentNormalofA = meshGenerator.GetNormalofFace(indexA);
                    TargetNormalofA = meshGenerator.GetNormalofFace(TargetIndexofA);

                    int NumOfConnectedFaces = meshGenerator.model.faces[indexA].ConnectedFaces.ToArray().Length;
                    for(int i = -1; i < NumOfConnectedFaces; i++)
                    {
                        int currentIndex;
                        if (i == -1)
                            currentIndex = indexA;
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

                    TargetIndexofB = meshGenerator.GetTheOtherFaceIndex(rmMidPoint, indexB);
                    currentNormalofB = meshGenerator.GetNormalofFace(indexB);
                    TargetNormalofB = meshGenerator.GetNormalofFace(TargetIndexofB);

                    int NumOfConnectedFaces = meshGenerator.model.faces[indexB].ConnectedFaces.ToArray().Length;
                    for (int i = -1; i < NumOfConnectedFaces; i++)
                    {
                        int currentIndex;
                        if (i == -1)
                            currentIndex = indexB;
                        else
                            currentIndex = meshGenerator.model.faces[indexB].ConnectedFaces[i];

                        meshGenerator.model.faces[TargetIndexofB].ConnectedFaces.Add(currentIndex);
                    }

                    StartUnfolding(indexB, currentNormalofB, TargetNormalofB, rmStartingPoint, rmEndingPoint);
                }
            }
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
        int index = face.linesMidpoints.IndexOf(midPoint);
        face.linesMidpoints.Remove(midPoint);
        face.lineStartingPoint.RemoveAt(index);
        face.lineEndingPoint.RemoveAt(index);
        Debug.Log(face.linesMidpoints.ToArray().Length);
        if (face.linesMidpoints.ToArray().Length > 1)
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

        List<int> faceIndices = meshGenerator.GetFaceIndicesOfLine(midPoint);
        //TODO: You need to consider a series of unfolding, which means when you delete the last line, a new unfolding happens.
        //But this can be done by deleting the line after the previous unfolding finished.
        DeleteAndCheckLinesInFace(faceIndices[0], midPoint);
        DeleteAndCheckLinesInFace(faceIndices[1], midPoint);
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
            if (meshGenerator.model.faces[i].ConnectedFaces.ToArray().Length != results.Length)
                return false;
            foreach(string result in results)
            {
                if (!meshGenerator.model.faces[i].ConnectedFaces.Contains(int.Parse(result)))
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
            //List<int> ConnectedFaces = meshGenerator.model.faces[i].ConnectedFaces;
            Debug.Log("Face " + i + " contains:");
            string CurrentConnectedFaces = "";
            foreach(int j in meshGenerator.model.faces[i].ConnectedFaces)
            {
                CurrentConnectedFaces += j.ToString();
                CurrentConnectedFaces += ";";
            }
            Debug.Log(CurrentConnectedFaces);
        }
    }

    public void SaveResult()
    {
        StreamWriter writer = new StreamWriter(path, false);

        int NumofFaces = meshGenerator.NumofFaces;
        for (int i = 0; i < NumofFaces; i++)
        {
            writer.WriteLine("Face " + i + " contains:");
            string CurrentConnectedFaces = "";
            foreach (int j in meshGenerator.model.faces[i].ConnectedFaces)
            {
                CurrentConnectedFaces += j.ToString();
                CurrentConnectedFaces += ";";
            }

            writer.WriteLine(CurrentConnectedFaces);
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
