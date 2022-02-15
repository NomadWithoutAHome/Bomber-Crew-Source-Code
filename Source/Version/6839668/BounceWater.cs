using UnityEngine;

public class BounceWater : MonoBehaviour
{
	[SerializeField]
	private Transform m_centreTarget;

	public Transform GetCentreTarget()
	{
		return m_centreTarget;
	}
}
