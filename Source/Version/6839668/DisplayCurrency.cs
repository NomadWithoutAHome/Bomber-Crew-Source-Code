using BomberCrewCommon;
using UnityEngine;

public class DisplayCurrency : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh m_textMesh;

	private void Update()
	{
		if (Singleton<SaveDataContainer>.Instance != null)
		{
			m_textMesh.text = $"{Singleton<SaveDataContainer>.Instance.Get().GetBalance():n0}";
		}
	}
}
