using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveRenderTexture : MonoBehaviour {

    private new string name = "texture.png";
    public RenderTexture oldRenderTexture;

	public void SaveRT()
    {
        //Application.CaptureScreenshot("Screenshot.png");
        RenderTexture.active = oldRenderTexture;
        Texture2D tex = new Texture2D(oldRenderTexture.width, oldRenderTexture.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        var bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(name, bytes);
        Destroy(tex);
    }
}
