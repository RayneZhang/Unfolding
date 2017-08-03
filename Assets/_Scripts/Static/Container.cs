using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Container
{
    public struct Model
    {
        public List<Face> faces;
    }

    public class Face
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector3> normals;

        public List<Vector3> linesMidpoints;
        public List<Vector3> lineStartingPoint;
        public List<Vector3> lineEndingPoint;
        public int offset = 0;

        public List<int> ConnectedFaces;
    }

    public class Line
    {
        //public LineRenderer renderer;

        public Vector3 midpoint;

        Vector3 vertexA;
        Vector3 vertexB;
        
        int FaceAIndex = -1;
        int FaceBIndex = -1;

        public Line(Vector3 _vertexA, Vector3 _vertexB)
        {
            vertexA = _vertexA;
            vertexB = _vertexB;
        }

        public int getFaceAIndex()
        {
            return FaceAIndex;
        }

        public int getFaceBIndex()
        {
            return FaceBIndex;
        }

        public void SetFaceIndices(int _FaceAIndex, int _FaceBIndex) {
            FaceAIndex = _FaceAIndex;
            FaceBIndex = _FaceBIndex;
        }
    }
}
