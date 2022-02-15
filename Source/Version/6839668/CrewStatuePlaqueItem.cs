using TMPro;
using UnityEngine;

public class CrewStatuePlaqueItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro m_text;

	public void SetUp(string rankAndNameText)
	{
		m_text.text = rankAndNameText.ToUpper();
	}
}
