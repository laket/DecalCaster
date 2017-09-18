using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリーンショットを管理するクラス
/// </summary>
public class ScreenShotController : MonoBehaviour {
    /// <summary>
    /// 外部からDecalRendererをAttachする
    /// </summary>
    public GameObject m_DecalRenderer;
    private int shotCount_ = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("screen" + shotCount_.ToString() + ".png");
            shotCount_++;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            RenderTexture maskTex = m_DecalRenderer.GetComponent<DeferredDecalRenderer>().TextMask;

            string pathOut = "./back.png";
            Debug.Log("capture running : path " + pathOut);

            Texture2D tex = new Texture2D(maskTex.width, maskTex.height);
            RenderTexture.active = maskTex;

            tex.ReadPixels(new Rect(0, 0, maskTex.width, maskTex.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(pathOut, bytes);
            RenderTexture.active = null;
        }

    }
}
