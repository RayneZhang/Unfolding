using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;

public class PlayerControl : MonoBehaviour {

    public MeshGenerator meshGenerator;

    [HideInInspector]
    public bool unfolding = false;
    bool unfoldA = false;
    bool unfoldB = false;

    //List<Vector3> TargetPositions;

    // Use this for initialization
    void Start () {
        //TargetPositions = new List<Vector3>();
        
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
}
