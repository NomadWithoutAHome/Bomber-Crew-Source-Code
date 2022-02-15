using UnityEngine;

public class EndlessModeUpgradeIcon : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite[] m_iconSprite;

	[SerializeField]
	private TagUIProgress m_taggableItem;

	[SerializeField]
	private AnimateDetailAlpha m_animateAlpha;

	[SerializeField]
	private TextSetter m_pointsText;

	[SerializeField]
	private GameObject[] m_pointsEnabled;

	private EndlessModeUpgrade m_upgrade;

	private EndlessModeUpgrade.UpgradeType m_currentType;

	private void Start()
	{
		Refresh();
	}

	private void Refresh()
	{
		m_upgrade = m_taggableItem.GetTaggableItem().GetComponent<EndlessModeUpgrade>();
		EndlessModeUpgrade.UpgradeType upgradeType = m_upgrade.GetUpgradeType();
		string sprite = "Icon_PickUp";
		bool flag = true;
		switch (upgradeType)
		{
		case EndlessModeUpgrade.UpgradeType.FuelRefill:
			sprite = "Icon_Fuel";
			break;
		case EndlessModeUpgrade.UpgradeType.HealCrew:
			sprite = "Station_MedicalBed";
			break;
		case EndlessModeUpgrade.UpgradeType.Repair:
			sprite = "Icon_Reliability";
			break;
		case EndlessModeUpgrade.UpgradeType.XPUpgrade:
			sprite = "Icon_PickUp";
			break;
		case EndlessModeUpgrade.UpgradeType.Points:
			flag = false;
			break;
		}
		if (flag)
		{
			tk2dBaseSprite[] iconSprite = m_iconSprite;
			foreach (tk2dBaseSprite tk2dBaseSprite2 in iconSprite)
			{
				tk2dBaseSprite2.SetSprite(sprite);
			}
			GameObject[] pointsEnabled = m_pointsEnabled;
			foreach (GameObject gameObject in pointsEnabled)
			{
				gameObject.SetActive(value: false);
			}
		}
		else
		{
			tk2dBaseSprite[] iconSprite2 = m_iconSprite;
			foreach (tk2dBaseSprite tk2dBaseSprite3 in iconSprite2)
			{
				tk2dBaseSprite3.gameObject.SetActive(value: false);
			}
			GameObject[] pointsEnabled2 = m_pointsEnabled;
			foreach (GameObject gameObject2 in pointsEnabled2)
			{
				gameObject2.SetActive(value: true);
			}
			m_pointsText.SetText(m_upgrade.GetPoints().ToString());
		}
		m_currentType = upgradeType;
	}

	private void Update()
	{
		if (m_upgrade != null)
		{
			m_animateAlpha.SetDetailAlpha(m_upgrade.GetFlash());
			if (m_upgrade.GetUpgradeType() != m_currentType)
			{
				Refresh();
			}
		}
	}
}
