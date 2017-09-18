using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using StringImageMaker;
using StringImageMaker.StringDrawing;

/*
  動的にDecalを作成するクラス。
  EmptyObjectに紐づけられる。prefabを一応つくるか

    クリックした点との衝突判定を通じて、Decalをはる
     */

public class DecalCasterController : MonoBehaviour {
    public GameObject decalPrefab_;
    private StringDrawer drawer_;
    private List<GameObject> decals_;

    // Use this for initialization
    void Start () {
        FontManager fontManager = new FontManager(
            new string[] { "meiryo UI", "Segoe Script", "SketchFlow Print"},
            minSize: 30,
            maxSize: 30
        );
        IMessageCreator messageCreator = RandomCharactorCreator.makeNumericCreator(minLen_: 3, maxLen_: 6);

        drawer_ = new StringDrawer(messageCreator, fontManager);
        decals_ = new List<GameObject>();
    }

    Texture2D convertToTexture2D(Bitmap bitmap)
    {
        Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);

        // ビットマップ上から持ってきたいピクセルがメモリ上の固定位置になるようにする
        var bits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        // ARGB32前提であることに注意
        var copyTo = new byte[bits.Width * bits.Height * 4];
        IntPtr head = bits.Scan0;

        // チャンネルの順序と、行の順序がBitmapとTexture2Dで異なることを考慮しながらコピー
        unsafe
        {
            byte* src = (byte*)head.ToPointer();

            for (int r = 0; r < bits.Height; r++)
            {
                // Unityは下から上にデータが並んでいる
                int toOffset = (bits.Height - 1 - r) * bits.Width * 4;
                int fromOffset = r * bits.Width * 4;

                for (int c = 0; c < bits.Width; c++)
                {
                    int to_base = toOffset + c * 4;
                    int from_base = fromOffset + c * 4;

                    // coypTo = ARGB
                    // bits   = BGRA
                    copyTo[to_base] = src[from_base + 3];
                    copyTo[to_base + 1] = src[from_base + 2];
                    copyTo[to_base + 2] = src[from_base + 1];
                    copyTo[to_base + 3] = src[from_base + 0];
                }
            }
        }

        texture.LoadRawTextureData(copyTo);
        texture.Apply();

        return texture;
    }

    void makeObject(Vector3 pos, Ray ray)
    {
        // デバッグ用にいきなりDecalを作成する
        GameObject go = Instantiate(decalPrefab_) as GameObject;

        go.transform.position = pos;

        // マウスでクリックした方向にz軸が向く (xy平面に交差するあたりに文字は描写される)
        go.transform.rotation = UnityEngine.Quaternion.Euler(ray.direction);
        go.transform.LookAt(pos + ray.direction);

        // テクスチャに文字画像をロード
        Bitmap bitmap = drawer_.drawNext();
        Texture2D texture = convertToTexture2D(bitmap);
        bitmap.Dispose();

        // マテリアルの参照を共有しないようにする (これをやらないと一つのテクスチャをかえると他も変わる)
        go.GetComponent<Decal>().m_Material = Instantiate(go.GetComponent<Decal>().m_Material);
        go.GetComponent<Decal>().m_Material.mainTexture = texture;

        decals_.Add(go);
    }


    // Update is called once per frame
    void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitpos = hit.point;
                
                makeObject(hitpos, ray);
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach(var go in decals_)
            {
                Destroy(go);
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach(var go in decals_)
            {
                Destroy(go);
            }

        }
    }

}
