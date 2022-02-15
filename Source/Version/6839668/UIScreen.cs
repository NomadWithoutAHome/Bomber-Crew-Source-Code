using UnityEngine;

public class UIScreen : MonoBehaviour
{
	public void EnableColliders(bool enable)
	{
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = enable;
		}
	}
}
