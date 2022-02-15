using UnityEngine;

public class DamageablePassthrough : Damageable
{
	[SerializeField]
	private Damageable m_masterDamageReceiver;

	public override bool IsDamageBlocker()
	{
		if (m_masterDamageReceiver != null)
		{
			return m_masterDamageReceiver.IsDamageBlocker();
		}
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		if (m_masterDamageReceiver != null)
		{
			return m_masterDamageReceiver.DamageGetPassthrough(amt, ds);
		}
		return 1f;
	}
}
