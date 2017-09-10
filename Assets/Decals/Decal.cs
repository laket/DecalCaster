using UnityEngine;

[ExecuteInEditMode]
public class Decal : MonoBehaviour
{
	public enum Kind
	{
		DiffuseOnly
	}
	public Kind m_Kind;
	public Material m_Material;
    //MEMO: 動的に生成すると不要になる
    public Mesh m_Cube;

    public void OnEnable()
    {
        DecalManager.instance.AddDecal(this);
    }

    public void Start()
    {
        DecalManager.instance.AddDecal(this);
    }


    public void OnDisable()
    {
        DecalManager.instance.RemoveDecal(this);
    }

    private void DrawGizmo(bool selected)
	{
		var col = new Color(0.0f,0.7f,1f,1.0f);
		col.a = selected ? 0.3f : 0.1f;
		Gizmos.color = col;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube (Vector3.zero, Vector3.one);
		col.a = selected ? 0.5f : 0.2f;
		Gizmos.color = col;
		Gizmos.DrawWireCube (Vector3.zero, Vector3.one);		
	}

	public void OnDrawGizmos()
	{
		DrawGizmo(false);
	}
	public void OnDrawGizmosSelected()
	{
		DrawGizmo(true);
	}
}
