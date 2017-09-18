using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリーンショットを管理するクラス
/// OnPostRenderを使っているためカメラにアタッチする必要あり
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

    /// <summary>
    /// テキストが書かれた領域以外は黒く塗られたテクスチャから、
    /// テキストが書かれた領域のBBoxを取得する
    /// </summary>
    /// <param name="maskTex"></param>
    /// <returns></returns>
    System.Drawing.Rectangle CalculateBBox(Texture2D maskTex) {
        Color[] colors = maskTex.GetPixels();
        int width = maskTex.width;
        int height = maskTex.height;

        int maxHeight = int.MinValue;
        int minHeight = int.MaxValue;
        int maxWidth = int.MinValue;
        int minWidth = int.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) {
                Color now = colors[x + y*width];

                if (now.r != 0 || now.g != 0 || now.b != 0)
                {
                    //MEMO めっちゃ遅そう
                    minHeight = Math.Min(minHeight, y);
                    maxHeight = Math.Max(maxHeight, y);
                    minWidth = Math.Min(minWidth, x);
                    maxWidth = Math.Max(maxWidth, x);
                }
            }
        }

        if (maxHeight == int.MinValue)
        {
            return System.Drawing.Rectangle.Empty;
        }

        return new System.Drawing.Rectangle(minWidth, minHeight, maxWidth - minWidth + 1, maxHeight - minHeight + 1);
    }

    /// <summary>
    /// Textureに四角形を描画する
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="rect"></param>
    void DrawRectagnle(Texture2D tex, System.Drawing.Rectangle rect)
    {
        // 上下2本
        for (int w = 0; w < rect.Width; w++)
        {
            tex.SetPixel(rect.X + w, rect.Y, Color.blue);
            tex.SetPixel(rect.X + w, rect.Y + rect.Height - 1, Color.blue);
        }
        // 左右2本
        for (int h = 0; h < rect.Height; h++)
        {
            tex.SetPixel(rect.X, rect.Y + h, Color.blue);
            tex.SetPixel(rect.X + rect.Width - 1, rect.Y + h, Color.blue);
        }
    }

    Texture2D GetScreenshot()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        return texture;
    }

    bool requireScreenshot_ = false;
    bool requireMask_ = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            requireScreenshot_ = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            requireMask_ = true;
        }

    }

    // Update is called once per frame
    void OnPostRender () {
        if (requireScreenshot_)
        {
            requireScreenshot_ = false;
            ScreenCapture.CaptureScreenshot("screen" + shotCount_.ToString() + ".png");
            shotCount_++;
        }

        if (requireMask_)
        {
            requireMask_ = false;
            Debug.Log("called");

            Texture2D screen = GetScreenshot();

            RenderTexture maskTex = m_DecalRenderer.GetComponent<DeferredDecalRenderer>().TextMask;

            Texture2D tex = new Texture2D(maskTex.width, maskTex.height);
            RenderTexture.active = maskTex;
            tex.ReadPixels(new Rect(0, 0, maskTex.width, maskTex.height), 0, 0);
            RenderTexture.active = null;


            var bbox = CalculateBBox(tex);
            Debug.Log("BBox:" + bbox.ToString());

            byte[] bytes = tex.EncodeToPNG();
            string pathOut = "./back.png";
            System.IO.File.WriteAllBytes(pathOut, bytes);

            DrawRectagnle(screen, bbox);
            byte[] screenBytes = screen.EncodeToPNG();
            System.IO.File.WriteAllBytes("./bbox.png", screenBytes);
        }

    }
}
