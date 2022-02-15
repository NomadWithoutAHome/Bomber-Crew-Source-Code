using UnityEngine;

public class UIEnableToggle : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_enableNodes;

	[SerializeField]
	private GameObject[] m_disableNodes;

	[SerializeField]
	private Collider[] m_interactionColliders;

	private void EnableNodes(bool enable)
	{
		if (enable)
		{
			GameObject[] enableNodes = m_enableNodes;
			foreach (GameObject gameObject in enableNodes)
			{
				gameObject.SetActive(value: true);
			}
			GameObject[] disableNodes = m_disableNodes;
			foreach (GameObject gameObject2 in disableNodes)
			{
				gameObject2.SetActive(value: false);
			}
			Collider[] interactionColliders = m_interactionColliders;
			foreach (Collider collider in interactionColliders)
			{
				collider.enabled = true;
			}
		}
		else
		{
			GameObject[] enableNodes2 = m_enableNodes;
			foreach (GameObject gameObject3 in enableNodes2)
			{
				gameObject3.SetActive(value: false);
			}
			GameObject[] disableNodes2 = m_disableNodes;
			foreach (GameObject gameObject4 in disableNodes2)
			{
				gameObject4.SetActive(value: true);
			}
			Collider[] interactionColliders2 = m_interactionColliders;
			foreach (Collider collider2 in interactionColliders2)
			{
				collider2.enabled = false;
			}
		}
	}

	public void EnableUI()
	{
		EnableNodes(enable: true);
	}

	public void DisableUI()
	{
		EnableNodes(enable: false);
	}
}
