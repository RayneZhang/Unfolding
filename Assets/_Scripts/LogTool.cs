using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class LogTool : MonoBehaviour {

    string content = "";
    string filePath = "Assets/_Logs/log.txt";

    /// <summary>
    /// Add the clicked line to log.
    /// </summary>
    /// <param name="p1">The first point of the line.</param>
    /// <param name="p2">The second point of the line.</param>
	public void LineClick(Vector3 p1, Vector3 p2)
    {
        content += "Type: ClickLine, ";

        content += "Line: (" + p1.x + "," + p1.y + "," + p1.z + ":" + p2.x + "," + p2.y + "," + p2.z + "), ";
        content += "Time: " + DateTime.Now + "\n";
    }

    public void SaveLog()
    {
        // Write some text to the text.txt file, but we don't need it in webGL.
        StreamWriter writer = new StreamWriter(filePath, false);
        writer.Write(content);
        writer.Close();
    }


}
