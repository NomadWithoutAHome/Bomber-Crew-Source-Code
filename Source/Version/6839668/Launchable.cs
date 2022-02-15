using UnityEngine;

public class Launchable : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] m_toEnable;

	public void SetLaunched(bool launched)
	{
		MonoBehaviour[] toEnable = m_toEnable;
		foreach (MonoBehaviour monoBehaviour in toEnable)
		{
			monoBehaviour.enabled = launched;
		}
		if (base.gameObject.btransform() != null && launched)
		{
			base.gameObject.btransform().SetFromCurrentPage(base.transform.position);
		}
		FighterAI component = base.gameObject.GetComponent<FighterAI>();
		if (component != null && launched)
		{
			component.SetWasLaunched();
		}
	}
}
