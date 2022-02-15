using UnityEngine;

public class UnlockSummaryDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_typeTextSetter;

	[SerializeField]
	private TextSetter m_nameTextSetter;

	public void SetUp(string typeNameText, string nameNameText)
	{
		m_typeTextSetter.SetText(typeNameText);
		m_nameTextSetter.SetText(nameNameText);
	}
}
