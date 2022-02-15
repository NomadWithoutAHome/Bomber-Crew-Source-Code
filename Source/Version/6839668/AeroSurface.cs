using BomberCrewCommon;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
	[SerializeField]
	private AeroSurfacesEfficiency m_efficiencySystem;

	[SerializeField]
	private float m_defaultFuelEfficiencyMultiplier;

	[SerializeField]
	private float m_fuelEfficiencyMultiplierOnDestroy;

	[SerializeField]
	private float m_fuelEffiencyMultiplierOnHidden;

	[SerializeField]
	private GameObject m_dragMesh;

	[SerializeField]
	private float m_minVelocityForDragMesh;

	private bool m_isDestroyed;

	private float m_hiddenAmount;

	private BomberSystems m_bomberSystems;

	public void SetHidden(float hiddenAmount)
	{
		m_hiddenAmount = hiddenAmount;
	}

	private void Start()
	{
		m_efficiencySystem.Register(this);
		BomberDestroyableSection.FindDestroyableSectionFor(base.transform).OnSectionDestroy += OnSectionDestroy;
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
	}

	private void OnSectionDestroy()
	{
		m_isDestroyed = true;
	}

	public float GetInverseEfficiency()
	{
		if (m_isDestroyed)
		{
			return m_fuelEfficiencyMultiplierOnDestroy;
		}
		return Mathf.Lerp(m_defaultFuelEfficiencyMultiplier, m_fuelEffiencyMultiplierOnHidden, m_hiddenAmount);
	}

	private void Update()
	{
		if (m_dragMesh != null)
		{
			if (!m_isDestroyed)
			{
				bool active = m_hiddenAmount < 0.5f && m_bomberSystems.GetBomberState().GetPhysicsModel().GetVelocity()
					.magnitude > m_minVelocityForDragMesh;
				m_dragMesh.SetActive(active);
			}
			else
			{
				m_dragMesh.SetActive(value: false);
			}
		}
	}
}
