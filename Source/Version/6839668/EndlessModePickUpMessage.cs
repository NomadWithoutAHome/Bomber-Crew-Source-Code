using System;
using System.Collections;
using Common;
using UnityEngine;

public class EndlessModePickUpMessage : MonoBehaviour
{
	[SerializeField]
	private GameObject m_enableNode;

	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private tk2dTextMesh m_textMesh;

	[SerializeField]
	private tk2dBaseSprite m_iconSprite;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private float m_displayTime = 3f;

	[SerializeField]
	private float m_destroyDelay = 2.5f;

	private void OnEnable()
	{
		tk2dTextMesh textMesh = m_textMesh;
		textMesh.OnRefresh = (Action)Delegate.Combine(textMesh.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
	}

	private void OnDisable()
	{
		tk2dTextMesh textMesh = m_textMesh;
		textMesh.OnRefresh = (Action)Delegate.Remove(textMesh.OnRefresh, new Action(m_layoutGrid.RepositionChildren));
	}

	public void SetUp(string iconSpriteName, string pickUpName)
	{
		m_enableNode.SetActive(value: false);
		m_iconSprite.SetSprite(iconSpriteName);
		m_textSetter.SetText(pickUpName);
		StartCoroutine(ShowMessage());
	}

	public void SetUp(string pickUpName, int score)
	{
		m_enableNode.SetActive(value: false);
		m_iconSprite.gameObject.SetActive(value: false);
		m_textSetter.SetText(string.Format(pickUpName, score));
		StartCoroutine(ShowMessage());
	}

	private IEnumerator ShowMessage()
	{
		m_enableNode.CustomActivate(active: true);
		yield return new WaitForSeconds(m_displayTime);
		m_enableNode.CustomActivate(active: false);
		yield return new WaitForSeconds(m_destroyDelay);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
