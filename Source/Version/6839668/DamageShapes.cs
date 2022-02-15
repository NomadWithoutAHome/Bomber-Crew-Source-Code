using BomberCrewCommon;
using UnityEngine;

public class DamageShapes : Singleton<DamageShapes>
{
	[SerializeField]
	private Texture2D[] m_damageShapes;

	public Texture2D GetShape()
	{
		return m_damageShapes[Random.Range(0, m_damageShapes.Length)];
	}
}
