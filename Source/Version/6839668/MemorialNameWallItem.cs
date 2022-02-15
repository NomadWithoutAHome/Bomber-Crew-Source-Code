using TMPro;
using UnityEngine;

public class MemorialNameWallItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro m_nameTextMesh;

	public void SetUp(string name)
	{
		m_nameTextMesh.text = name;
	}
}
