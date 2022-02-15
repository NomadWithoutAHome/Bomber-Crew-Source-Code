using UnityEngine;

public class MultiTextSetter : TextSetter
{
	[SerializeField]
	private tk2dTextMesh[] m_allMeshesToSet;

	public override void SetText(string text)
	{
		tk2dTextMesh[] allMeshesToSet = m_allMeshesToSet;
		foreach (tk2dTextMesh tk2dTextMesh2 in allMeshesToSet)
		{
			tk2dTextMesh2.SetText(text);
		}
	}
}
