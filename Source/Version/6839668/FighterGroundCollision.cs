using UnityEngine;

public class FighterGroundCollision : MonoBehaviour
{
	[SerializeField]
	private FighterPlane m_fighter;

	[SerializeField]
	private LayerMask m_environmentLayer;

	private void OnTriggerEnter(Collider c)
	{
		if (((1 << c.gameObject.layer) & (int)m_environmentLayer) != 0 && !c.isTrigger)
		{
			m_fighter.SetDestroyed(forceExplode: true, forceDontExplode: false, byPlayer: false);
		}
	}
}
