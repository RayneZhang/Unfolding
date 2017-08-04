﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;
using MyConstants;

public class MeshGenerator : MonoBehaviour {

    MeshFilter mf;
    Mesh mesh;

    public Model model;

    public float unit = 1.0f;
    /// <summary>
    /// The Material of Line.
    /// </summary>
    public Material LineMaterial;

    //Vertices
    List<Vector3> vertices;
    //Triangles
    List<int> triangles;
    //Normals
    List<Vector3> normals;
    //UVs
    List<Vector2> uvs;

    List<Line> AllLines;

    /// <summary>
    /// The quantitiy of vertex of LineRenderer.
    /// </summary>
    int vertexCount = 2;

    /// <summary>
    /// The width of the line.
    /// </summary>
    float lineWidth = 0.1f;

    public PlayerControl player;

    public float smoothTime = 5.0f;
    private float smoothPassedTime = 0f;

    private List<int> UnfoldingFaces;
    private List<Vector3> StartingVertices;
    private List<Vector3> UnfoldingVertices;
    private List<Vector3> StartingNormals;
    private List<Vector3> UnfoldingNormals;

    [HideInInspector]
    public int NumofFaces = 0;

    // Use this for initialization
    void Start() {
        mf = gameObject.GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;

        InitArrays();

        CreateModel();

        //Assign Arrays
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();

        CreateLines();    
    }

    private void InitArrays()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();

        UnfoldingFaces = new List<int>();
        StartingVertices = new List<Vector3>();
        UnfoldingVertices = new List<Vector3>();
        StartingNormals = new List<Vector3>();
        UnfoldingNormals = new List<Vector3>();
    }

    void Update()
    {
        if (player.unfolding)
        {
            
            //Debug.Log("Amoving!");
            smoothPassedTime = Mathf.Min(smoothPassedTime + Time.deltaTime, smoothTime);
            if(smoothPassedTime >= smoothTime)
            {
                player.unfolding = false;
                smoothPassedTime = 0f;
                UnfoldingFaces.Clear();
                StartingVertices.Clear();
                UnfoldingVertices.Clear();
                StartingNormals.Clear();
                UnfoldingNormals.Clear();
            }
            else
            {
                int ptr = 0;
                int length = UnfoldingFaces.ToArray().Length;
                for (int i = 0; i < length; i++)
                {
                    int FaceIndex = UnfoldingFaces[i];
                    int VertexSize = model.faces[FaceIndex].vertices.ToArray().Length;
                    int Offset = model.faces[FaceIndex].offset;
                    int ReverseOffset = model.faces[FaceIndex + NumofFaces].offset;

                    Vector3 tmpNorm = Vector3.Lerp(StartingNormals[i], UnfoldingNormals[i], Mathf.SmoothStep(0, 1, smoothPassedTime / smoothTime));

                    for (int j = 0; j < VertexSize; j++)
                    {
                        Vector3 tmpPos = Vector3.Lerp(StartingVertices[j + ptr], UnfoldingVertices[j + ptr], Mathf.SmoothStep(0, 1, smoothPassedTime / smoothTime));
                        vertices[j + Offset] = tmpPos;
                        vertices[j + ReverseOffset] = tmpPos;

                        normals[j + Offset] = tmpNorm;
                        normals[j + ReverseOffset] = -tmpNorm;
                    }

                    ptr += VertexSize;
                }
                mesh.vertices = vertices.ToArray();
                mesh.normals = normals.ToArray();
            }
            
        }

    }

    private void CreateModel()
    {
        model = new Model();
        model.faces = new List<Face>();

        if(MyConstants.Data.Faces.Length == 0)
        {
            Debug.LogError("Information of the model is missing!");
        }
        else {
            NumofFaces = MyConstants.Data.Faces[0];
            int ptr = 1;
            int offset = 0;
            for(int i = 0; i < NumofFaces * 2; i++)
            {
                if (i == NumofFaces)
                    ptr = 1;

                Face newFace = FaceInit();

                int NumofVertices = MyConstants.Data.Faces[ptr++];
                int NumofTriangles = MyConstants.Data.Faces[ptr++];
                Vector3 normal = new Vector3();
                if (i < NumofFaces)
                    normal = GetNormalByNum(MyConstants.Data.Faces[ptr++]);
                else
                    normal = GetReverseNormalByNum(MyConstants.Data.Faces[ptr++]);

                for (int j = 0; j < NumofVertices; j++)
                {
                    Vector3 vertex = new Vector3(MyConstants.Data.Faces[ptr++],
                                                            MyConstants.Data.Faces[ptr++],
                                                            MyConstants.Data.Faces[ptr++]);
                    newFace.vertices.Add(vertex);
                    vertices.Add(vertex);

                    newFace.normals.Add(normal);
                    normals.Add(normal);
                }
                for(int k = 0; k < NumofTriangles; k++)
                {
                    int triangleNode1 = MyConstants.Data.Faces[ptr++] + offset;
                    int triangleNode2 = MyConstants.Data.Faces[ptr++] + offset;
                    int triangleNode3 = MyConstants.Data.Faces[ptr++] + offset;

                    if (i < NumofFaces)
                    {
                        newFace.triangles.Add(triangleNode1);
                        newFace.triangles.Add(triangleNode2);
                        newFace.triangles.Add(triangleNode3);
                        triangles.Add(triangleNode1);
                        triangles.Add(triangleNode2);
                        triangles.Add(triangleNode3);
                    }
                    else
                    {
                        newFace.triangles.Add(triangleNode3);
                        newFace.triangles.Add(triangleNode2);
                        newFace.triangles.Add(triangleNode1);
                        triangles.Add(triangleNode3);
                        triangles.Add(triangleNode2);
                        triangles.Add(triangleNode1);
                    }
                }
                newFace.offset = offset;
                offset += newFace.vertices.ToArray().Length;

                model.faces.Add(newFace);
            }
        }
    }

    private Face FaceInit()
    {
        Face newFace = new Face();
        newFace.vertices = new List<Vector3>();
        newFace.triangles = new List<int>();
        newFace.normals = new List<Vector3>();
        newFace.linesMidpoints = new List<Vector3>();
        newFace.lineStartingPoint = new List<Vector3>();
        newFace.lineEndingPoint = new List<Vector3>();
        newFace.ConnectedFaces = new List<int>();
        newFace.Neighbors = new List<int>();
        return newFace;
    }

    private Vector3 GetNormalByNum(int index)
    {
        switch (index) {
            case 0:
                return Vector3.up;
            case 1:
                return Vector3.down;
            case 2:
                return Vector3.left;
            case 3:
                return Vector3.right;
            case 4:
                return Vector3.forward;
            case 5:
                return Vector3.back;
            default:
                Debug.LogError("The normal is incorrect!");
                return Vector3.zero;
        }
    }

    private Vector3 GetReverseNormalByNum(int index)
    {
        switch (index)
        {
            case 0:
                return Vector3.down;
            case 1:
                return Vector3.up;
            case 2:
                return Vector3.right;
            case 3:
                return Vector3.left;
            case 4:
                return Vector3.back;
            case 5:
                return Vector3.forward;
            default:
                Debug.LogError("The normal is incorrect!");
                return Vector3.zero;
        }
    }

    /// <summary>
    /// Create all the Lines Objects of the 3D model.
    /// </summary>
    private void CreateLines() {
        AllLines = new List<Line>();
        int size = MyConstants.Data.Lines.Length;
        for(int i = 0;i < size; i += 8)
        {
            //Debug.Log(Container.Model.nodes[i]);
            //Debug.Log(Container.Model.nodes[i + 1]);

            Vector3 startingPoint = new Vector3(MyConstants.Data.Lines[i + 2], MyConstants.Data.Lines[i + 3], MyConstants.Data.Lines[i + 4]);
            Vector3 endingPoint = new Vector3(MyConstants.Data.Lines[i + 5], MyConstants.Data.Lines[i + 6], MyConstants.Data.Lines[i + 7]);
            Line newLine = LineInit(startingPoint, endingPoint);

            int faceA = MyConstants.Data.Lines[i];
            int faceB = MyConstants.Data.Lines[i + 1];
            newLine.SetFaceIndices(faceA, faceB);
            AllLines.Add(newLine);

            model.faces[faceA].linesMidpoints.Add((startingPoint + endingPoint) / 2);
            model.faces[faceA].lineStartingPoint.Add(startingPoint);
            model.faces[faceA].lineEndingPoint.Add(endingPoint);
            model.faces[faceB].linesMidpoints.Add((startingPoint + endingPoint) / 2);
            model.faces[faceB].lineStartingPoint.Add(startingPoint);
            model.faces[faceB].lineEndingPoint.Add(endingPoint);
        }
    }

    Line LineInit(Vector3 _vertexA, Vector3 _vertexB)
    {
        Line currentLine = new Line(_vertexA, _vertexB);
        
        GameObject currentLineObj = new GameObject();
        currentLineObj.transform.parent = this.transform;
        currentLineObj.name = "Line";
        currentLineObj.tag = "Line";

        LineRenderer currentRenderer = currentLineObj.AddComponent<LineRenderer>();

        //currentRenderer.sortingLayerName = "Foreground";

        currentRenderer.material = LineMaterial;
        currentRenderer.positionCount = vertexCount;
        currentRenderer.startColor = Color.red;
        currentRenderer.endColor = Color.red;
        currentRenderer.startWidth = lineWidth;
        currentRenderer.endWidth = lineWidth;

        currentRenderer.SetPosition(0, _vertexA);
        currentRenderer.SetPosition(1, _vertexB);

        /*MeshCollider meshCollider = currentLineObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;*/
        currentLineObj.AddComponent<Glow>();
        currentLineObj.GetComponent<Glow>().originalMaterial = LineMaterial;

        BoxCollider boxCollider = currentLineObj.AddComponent<BoxCollider>();
        float lineLength = Vector3.Distance(_vertexA, _vertexB);

        //TODO: This is buggy because lines may not be vertical or horizontal.
        if(_vertexA.x != _vertexB.x)
            boxCollider.size = new Vector3(lineLength, 0.1f, 0.1f);
        if(_vertexA.y != _vertexB.y)
            boxCollider.size = new Vector3(0.1f, lineLength, 0.1f);
        if(_vertexA.z != _vertexB.z)
            boxCollider.size = new Vector3(0.1f, 0.1f, lineLength);

        Vector3 midPoint = (_vertexA + _vertexB) / 2;
        currentLine.midpoint = midPoint;
        

        return currentLine;   
    }

    /// <summary>
    /// Return the two faces of a given line.
    /// </summary>
    /// <param name="midPoint">the midPoint of the given line</param>
    /// <returns></returns>
	public List<int> GetFaceIndicesOfLine(Vector3 midPoint)
    {
        List<int> FaceIndices = new List<int>();
        foreach(Line line in AllLines)
        {
            if(line.midpoint == midPoint)
            {
                FaceIndices.Add(line.getFaceAIndex());
                FaceIndices.Add(line.getFaceBIndex());
            }
        }
        return FaceIndices;
    }

    /// <summary>
    /// Return the other face index of a given line and face.
    /// </summary>
    /// <param name="midPoint"></param>
    /// <param name="givenIndex"></param>
    /// <returns></returns>
    public int GetTheOtherFaceIndex(Vector3 midPoint, int givenIndex)
    {
        foreach (Line line in AllLines)
        {
            if (line.midpoint == midPoint)
            {
                if (givenIndex == line.getFaceAIndex())
                    return line.getFaceBIndex();
                else
                    return line.getFaceAIndex();
            }
        }

        // Can't find index
        return -1;
    }

    /// <summary>
    /// Return the normal vector of a face by a given face index. 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetNormalofFace(int index)
    {
        return model.faces[index].normals[0];
    }

    public void StartUnfolding(int FaceIndex, Quaternion rotation, Vector3 _StartNormal, Vector3 _TargetNormal, Vector3 startingPoint, Vector3 endingPoint)
    {
        /*int offset = model.faces[FaceIndex].offset;
        int vertexSize = model.faces[FaceIndex].vertices.ToArray().Length;
        for (int i = 0; i < vertexSize; i++)
        {
            Vector3 currentVertex = vertices[i + offset];
            Vector3 PivotPoint = GetPivotPoint(startingPoint, endingPoint, currentVertex);
            Vector3 newPosition = rotation * (currentVertex - PivotPoint) + PivotPoint;
            //Debug.Log("The target position of " + currentVertex + " is " + newPosition);
            UnfoldingVertices.Add(newPosition);

            StartingVertices.Add(currentVertex);
        }
        UnfoldingFaces.Add(FaceIndex);
        StartingNormals.Add(_StartNormal);
        UnfoldingNormals.Add(_TargetNormal);*/

        int NumOfConnectedFaces = model.faces[FaceIndex].ConnectedFaces.ToArray().Length;
        for (int i = -1; i< NumOfConnectedFaces; i++)
        {
            int currentIndex;
            if (i == -1)
                currentIndex = FaceIndex;
            else
                currentIndex = model.faces[FaceIndex].ConnectedFaces[i];

            int offset = model.faces[currentIndex].offset;
            int vertexSize = model.faces[currentIndex].vertices.ToArray().Length;
            for (int j = 0; j < vertexSize; j++)
            {
                Vector3 currentVertex = vertices[j + offset];
                Vector3 PivotPoint = GetPivotPoint(startingPoint, endingPoint, currentVertex);
                Vector3 newPosition = rotation * (currentVertex - PivotPoint) + PivotPoint;
                //Debug.Log("The target position of " + currentVertex + " is " + newPosition);
                UnfoldingVertices.Add(newPosition);

                StartingVertices.Add(currentVertex);
            }
            UnfoldingFaces.Add(currentIndex);
            StartingNormals.Add(_StartNormal);
            UnfoldingNormals.Add(_TargetNormal);

            model.faces[currentIndex].normals[0] = _TargetNormal;
            model.faces[currentIndex + NumofFaces].normals[0] = _TargetNormal;
        }

    }

    private Vector3 GetPivotPoint(Vector3 startingPoint, Vector3 endingPoint, Vector3 currentPoint)
    {
        float angle = Vector3.Angle(endingPoint - startingPoint, currentPoint - startingPoint);
        //Debug.Log("The angle is " + angle);
        Vector3 PivotPoint = startingPoint + (endingPoint - startingPoint).normalized * (Vector3.Distance(currentPoint, startingPoint) * Mathf.Cos(Mathf.Deg2Rad * angle));
        //Debug.Log("The PivotPoint of " + currentPoint + " is " + PivotPoint);
        return PivotPoint;
    }

    public void ReGenerate()
    {
        InitArrays();

        CreateModel();

        //Assign Arrays
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();

        CreateLines();
    }
}
