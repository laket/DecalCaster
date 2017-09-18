using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class DecalManager {
    bool waitingUpdate_ = false;
    static DecalManager m_Instance;
    internal HashSet<Decal> m_DecalsDiffuse = new HashSet<Decal>();

    static public DecalManager instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new DecalManager();
            return m_Instance;
        }
    }

    public void AddDecal(Decal d)
    {
        RemoveDecal(d);
        if (d.m_Kind == Decal.Kind.DiffuseOnly)
            m_DecalsDiffuse.Add(d);

        waitingUpdate_ = true;
    }
    public void RemoveDecal(Decal d)
    {
        m_DecalsDiffuse.Remove(d);

        waitingUpdate_ = true;
    }

    public bool WaitingUpdate {
        get { return waitingUpdate_; }
        set { waitingUpdate_ = value; }
    }
}

[ExecuteInEditMode]
public class DeferredDecalRenderer : MonoBehaviour
{
    private CommandBuffer commandBuffer_;
    public Mesh m_Mesh;

    const CameraEvent targetEvent = CameraEvent.BeforeLighting;


    public void OnDisable()
	{
        if (commandBuffer_ != null && Camera.main != null)
        {
            Camera.main.RemoveCommandBuffer(targetEvent, commandBuffer_);
        }
	}

    private RenderTexture backTex_;
    private RenderTargetIdentifier backTexID_;

    public void Start()
    {
        backTex_ = new RenderTexture(Screen.width, Screen.height, 0);
        backTexID_ = new RenderTargetIdentifier(backTex_);

    }


    public void Update()
	{
		var act = gameObject.activeInHierarchy && enabled;
		if (!act)
		{
			OnDisable();
			return;
		}

        if (Input.GetKeyDown(KeyCode.B))
        {
            //string pathOut = Application.persistentDataPath + "/back.png";
            string pathOut = "./back.png";
            Debug.Log("capture running : path " + pathOut);


            Texture2D tex = new Texture2D(backTex_.width, backTex_.height);
            RenderTexture.active = backTex_;

            tex.ReadPixels(new Rect(0, 0, backTex_.width, backTex_.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(pathOut, bytes);
            RenderTexture.active = null;
        }


        var system = DecalManager.instance;
        if (!system.WaitingUpdate) {
            return;
        }

        // カメラが単一と仮定
        var cam = Camera.main;
		if (!cam)
			return;

        if (commandBuffer_ == null)
        {
            commandBuffer_ = new CommandBuffer();
            commandBuffer_.name = "Deferred decals";
            cam.AddCommandBuffer(targetEvent, commandBuffer_);
        }
        else {
            commandBuffer_.Clear();
        }

        // copy g-buffer normals into a temporary RT
        var normalsID = Shader.PropertyToID("_NormalsCopy");
		commandBuffer_.GetTemporaryRT (normalsID, -1, -1);
		commandBuffer_.Blit (BuiltinRenderTextureType.GBuffer2, normalsID);

        // 位置テクスチャーをクリア
        commandBuffer_.SetRenderTarget(backTexID_);
        commandBuffer_.ClearRenderTarget(false, true, Color.black);

        // 描画先を指定する
        RenderTargetIdentifier[] identifiers = { BuiltinRenderTextureType.GBuffer0, backTexID_};
		commandBuffer_.SetRenderTarget(identifiers, BuiltinRenderTextureType.CameraTarget);


        foreach (var decal in system.m_DecalsDiffuse)
		{
            commandBuffer_.DrawMesh(decal.m_Cube, decal.transform.localToWorldMatrix, decal.m_Material);
        }
		// release temporary normals RT
		commandBuffer_.ReleaseTemporaryRT (normalsID);
        system.WaitingUpdate = false;
	}
}
