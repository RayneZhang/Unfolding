using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MiniJSON;

public class Data {
    public int[] Lines;
    public int[] Faces;

    public Data(string DataPath)
    {
        ImportData(DataPath);
    }

    private void ImportData(string _path)
    {
        string jsonString = File.ReadAllText(_path);
        ParseJsonString(jsonString);
    }

    private void ParseJsonString(string data)
    {
        Dictionary<string, object> dict;
        dict = Json.Deserialize(data) as Dictionary<string, object>;

        List<object> LineObjList = dict["Lines"] as List<object>;
        List<object> FaceObjList = dict["Faces"] as List<object>;

        List<int> LineList = new List<int>();
        List<int> FaceList = new List<int>();

        foreach(object i in LineObjList)
            LineList.Add(System.Convert.ToInt32(i));

        foreach(object j in FaceObjList)
            FaceList.Add(System.Convert.ToInt32(j));

        Lines = LineList.ToArray();
        Faces = FaceList.ToArray();
    }

        /*public static int[] Lines = new int[]
        {
            // First two items are two face indices of the line, and the last 6 items are the positions of two nodes.
            0,1,0,0,0,4,0,0,
            0,3,0,0,0,0,0,4,
            0,2,0,0,4,4,0,4,
            0,4,4,0,4,4,0,0,
            1,3,0,0,0,0,2,0,
            3,2,0,0,4,0,4,4,
            2,4,4,0,4,4,4,4,
            4,1,4,0,0,4,4,0,
            5,6,0,2,3,4,2,3,
            5,7,2,2,1,4,2,1,
            5,8,2,2,1,2,2,0,
            5,1,0,2,0,2,2,0,
            5,3,0,2,0,0,2,3,
            5,4,4,2,1,4,2,3,
            6,3,0,2,3,0,4,3,
            6,4,4,2,3,4,4,3,
            7,4,4,2,1,4,4,1,
            7,8,2,2,1,2,4,1,
            8,1,2,2,0,2,4,0,
            9,3,0,4,3,0,4,4,
            9,2,0,4,4,4,4,4,
            9,4,4,4,4,4,4,3,
            9,6,4,4,3,0,4,3,
            10,1,2,4,0,4,4,0,
            10,8,2,4,0,2,4,1,
            10,7,2,4,1,4,4,1,
            10,4,4,4,1,4,4,0
        };

        public static int[] Faces = new int[]
        {
            // Normals: 0 - up, 1 - down, 2 - left, 3 - right, 4 - forward, 5 - back
            // The number of faces.
            11,
            // Face1 - bottom, the first is the number of vertices, the second is number of triangles, the third is the direction of normal
            4, 2, 1,

            0,0,0,
            0,0,4,
            4,0,4,
            4,0,0,
            2,1,0,
            2,0,3,
            // Face2 - front
            6,4,5,

            0,0,0,
            0,2,0,
            2,2,0,
            2,4,0,
            4,4,0,
            4,0,0,
            0,1,5,
            1,2,5,
            2,3,4,
            2,4,5,
            // Face3 - back
            4,2,4,

            0,0,4,
            0,4,4,
            4,4,4,
            4,0,4,
            2,1,0,
            2,0,3,
            // Face4 - left
            6,4,2,

            0,0,4,
            0,4,4,
            0,4,3,
            0,2,3,
            0,2,0,
            0,0,0,
            0,1,2,
            0,2,3,
            0,3,5,
            3,4,5,
            // Face5 - right
            8,6,3,

            4,0,0,
            4,4,0,
            4,4,1,
            4,2,1,
            4,2,3,
            4,4,3,
            4,4,4,
            4,0,4,
            0,1,2,
            0,2,3,
            0,3,7,
            7,3,4,
            7,4,5,
            7,5,6,
            // Face6 - middle
            6,4,0,

            0,2,0,
            0,2,3,
            4,2,3,
            4,2,1,
            2,2,1,
            2,2,0,
            0,1,2,
            0,2,4,
            0,4,5,
            4,2,3,
            // Face7 - middle_back
            4,2,5,

            0,2,3,
            0,4,3,
            4,4,3,
            4,2,3,
            0,1,2,
            0,2,3,
            // Face8 - middle_front
            4,2,4,

            2,2,1,
            2,4,1,
            4,4,1,
            4,2,1,
            2,1,0,
            2,0,3,
            // Face9 - middle_left
            4,2,2,

            2,2,0,
            2,2,1,
            2,4,1,
            2,4,0,
            0,1,2,
            0,2,3,
            // Face10 - top_back
            4,2,0,
            
            0,4,3,
            0,4,4,
            4,4,4,
            4,4,3,
            0,1,2,
            0,2,3,

            // Face11 - top_front
            4,2,0,

            2,4,0,
            2,4,1,
            4,4,1,
            4,4,0,
            0,1,2,
            0,2,3
        };*/
}

