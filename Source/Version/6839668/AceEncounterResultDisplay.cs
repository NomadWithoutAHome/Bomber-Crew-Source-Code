using System.Collections;
using Common;
using UnityEngine;
using WingroveAudio;

public class AceEncounterResultDisplay : MonoBehaviour
{
	[SerializeField]
	private Renderer m_portraitRenderer;

	[SerializeField]
	private GameObject m_defeatedHierarchy;

	[SerializeField]
	private GameObject m_escapedHierarchy;

	[SerializeField]
	private float m_preShowDelay;

	[SerializeField]
	private TextSetter m_name;

	public void SetUp(EnemyFighterAce ace, bool defeated)
	{
		m_name.SetText(ace.GetFirstName() + " " + ace.GetSurname());
		m_portraitRenderer.sharedMaterial = Object.Instantiate(m_portraitRenderer.sharedMaterial);
		m_portraitRenderer.sharedMaterial.SetTexture("_MainTex", ace.GetPortraitTexture());
		m_defeatedHierarchy.SetActive(value: false);
		m_escapedHierarchy.SetActive(value: false);
		StartCoroutine(DoDisplay(defeated, ace));
	}

	public float GetDelay()
	{
		return m_preShowDelay;
	}

	private IEnumerator DoDisplay(bool defeated, EnemyFighterAce ace)
	{
		yield return new WaitForSeconds(m_preShowDelay);
		if (defeated)
		{
			WingroveRoot.Instance.PostEvent("DEBRIEF_ACE_DEFEAT");
		}
		else
		{
			WingroveRoot.Instance.PostEvent("DEBRIEF_ACE_ESCAPE");
		}
		m_defeatedHierarchy.CustomActivate(defeated);
		m_escapedHierarchy.CustomActivate(!defeated);
	}
}
