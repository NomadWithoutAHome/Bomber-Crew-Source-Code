using System;
using BomberCrewCommon;
using UnityEngine;

public class DamageObjectsOnTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private bool m_hasDoneDamage;

	private string m_trigger;

	private void Start()
	{
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(DoDamage));
		m_trigger = m_placeable.GetParameter("trigger");
	}

	private void DoDamage(string trigger)
	{
		if (!(trigger == m_trigger))
		{
			return;
		}
		m_hasDoneDamage = true;
		int num = Convert.ToInt32(m_placeable.GetParameter("numObjects"));
		for (int i = 0; i < num; i++)
		{
			string parameter = m_placeable.GetParameter("object" + i);
			MissionPlaceableObject placeableByRef = Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(parameter);
			if (placeableByRef != null)
			{
				DamageSource damageSource = new DamageSource();
				damageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
				damageSource.m_damageType = DamageSource.DamageType.Impact;
				Damageable component = placeableByRef.GetComponent<Damageable>();
				if (component != null)
				{
					component.DamageGetPassthrough(30000f, damageSource);
				}
			}
		}
	}
}
