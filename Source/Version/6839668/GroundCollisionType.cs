using UnityEngine;

public class GroundCollisionType : MonoBehaviour
{
	public enum GroundCollisionEffectType
	{
		Default,
		Woodland
	}

	[SerializeField]
	private GroundCollisionEffectType m_collisionEffectType;

	public GroundCollisionEffectType GetEffectType()
	{
		return m_collisionEffectType;
	}
}
