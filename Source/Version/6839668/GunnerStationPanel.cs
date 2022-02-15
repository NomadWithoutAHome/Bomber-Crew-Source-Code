using BomberCrewCommon;
using UnityEngine;

public class GunnerStationPanel : StationPanel
{
	[SerializeField]
	private tk2dUIItem m_testFireButton;

	[SerializeField]
	private TextSetter m_ammoCount;

	[SerializeField]
	private TextSetter m_beltCount;

	[SerializeField]
	private PanelCooldownButton m_focusButton;

	[SerializeField]
	private PanelCooldownButton m_defensiveFireButton;

	[SerializeField]
	private PanelCooldownButton m_incendiaryAmmoButton;

	[SerializeField]
	private PanelCooldownButton m_armourPiercingButton;

	[SerializeField]
	private PanelCooldownButton m_highExplosiveButton;

	[SerializeField]
	private GameObject m_outOfAmmoLight;

	[SerializeField]
	private GameObject m_ammoFeedInstalledIndicator;

	[SerializeField]
	private GameObject m_showMissileOptionsHierarchy;

	[SerializeField]
	private PanelCooldownButton m_missileLaunchButton;

	[SerializeField]
	private TextSetter m_missileCounter;

	private BomberSystems m_bomberSystems;

	private BomberState m_bomber;

	private bool m_isOutOfAmmo;

	private bool m_showingMissile;

	private int m_lastAmmo = -2;

	private int m_lastBelts = -2;

	private int m_lastMissiles = -1;

	private bool m_bomberHasMissiles;

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomber = m_bomberSystems.GetBomberState();
		m_testFireButton.OnClick += RequestTestFire;
		m_focusButton.OnClick += Focus;
		m_defensiveFireButton.OnClick += Defensive;
		m_incendiaryAmmoButton.OnClick += RequestIncendiaryAmmo;
		m_armourPiercingButton.OnClick += RequestAPAmmo;
		m_highExplosiveButton.OnClick += RequestHEAmmo;
		m_missileLaunchButton.OnClick += LaunchMissile;
		m_outOfAmmoLight.SetActive(value: false);
		m_showMissileOptionsHierarchy.SetActive(value: false);
		StationGunner stationGunner = (StationGunner)GetStation();
		m_ammoFeedInstalledIndicator.SetActive(stationGunner.GetAmmoFeed().HasAmmoFeed());
		m_bomberHasMissiles = stationGunner.GetMissileLauncher() != null;
		Refresh();
	}

	private void Focus()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.Focus();
	}

	private void Defensive()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.DefensiveFire();
	}

	private void RequestTestFire()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.RequestTestFire();
	}

	private void RequestIncendiaryAmmo()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.RequestIncendiaryAmmo();
	}

	private void RequestAPAmmo()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.RequestAPAmmo();
	}

	private void RequestHEAmmo()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.RequestHEAmmo();
	}

	private void LaunchMissile()
	{
		StationGunner stationGunner = (StationGunner)GetStation();
		stationGunner.FireMissile();
	}

	private void SetAmmoBelts(int ammoValue, int beltValue)
	{
		if (m_lastAmmo != ammoValue)
		{
			if (ammoValue == -1)
			{
				m_ammoCount.SetText("-");
			}
			else
			{
				m_ammoCount.SetText(ammoValue.ToString());
			}
			ammoValue = m_lastAmmo;
		}
		if (m_lastBelts != beltValue)
		{
			if (beltValue == -1)
			{
				m_beltCount.SetText("-");
			}
			else
			{
				m_beltCount.SetText(beltValue.ToString());
			}
			m_lastBelts = beltValue;
		}
	}

	private void Refresh()
	{
		bool flag = false;
		StationGunner stationGunner = (StationGunner)GetStation();
		if (stationGunner.GetAmmoFeed().IsInfinite())
		{
			SetAmmoBelts(-1, -1);
		}
		else if (stationGunner.GetAmmoFeed().HasAmmoFeed())
		{
			SetAmmoBelts(stationGunner.GetAmmoFeed().GetAmmo(), -1);
		}
		else
		{
			SetAmmoBelts(stationGunner.GetAmmoFeed().GetAmmo(), stationGunner.GetAmmoFeed().GetBelts());
			if (stationGunner.GetAmmoFeed().GetAmmo() + stationGunner.GetAmmoFeed().GetBelts() == 0)
			{
				flag = true;
			}
		}
		if (flag != m_isOutOfAmmo)
		{
			m_isOutOfAmmo = flag;
			m_outOfAmmoLight.SetActive(flag);
		}
		SkillRefreshTimer.SkillTimer focusTimer = stationGunner.GetFocusTimer();
		m_focusButton.SetStatus(focusTimer.IsActive(), focusTimer.CanStart(), isDisabled: false, focusTimer.GetRechargeNormalised(), focusTimer.GetInUseNormalised());
		SkillRefreshTimer.SkillTimer defensiveTimer = stationGunner.GetDefensiveTimer();
		m_defensiveFireButton.SetStatus(defensiveTimer.IsActive(), defensiveTimer.CanStart(), isDisabled: false, defensiveTimer.GetRechargeNormalised(), defensiveTimer.GetInUseNormalised());
		SkillRefreshTimer.SkillTimer incendiaryTimer = stationGunner.GetIncendiaryTimer();
		m_incendiaryAmmoButton.SetStatus(incendiaryTimer.IsActive(), incendiaryTimer.CanStart(), isDisabled: false, incendiaryTimer.GetRechargeNormalised(), incendiaryTimer.GetInUseNormalised());
		SkillRefreshTimer.SkillTimer armourPiercingTimer = stationGunner.GetArmourPiercingTimer();
		m_armourPiercingButton.SetStatus(armourPiercingTimer.IsActive(), armourPiercingTimer.CanStart(), isDisabled: false, armourPiercingTimer.GetRechargeNormalised(), armourPiercingTimer.GetInUseNormalised());
		SkillRefreshTimer.SkillTimer highExplosiveTimer = stationGunner.GetHighExplosiveTimer();
		m_highExplosiveButton.SetStatus(highExplosiveTimer.IsActive(), highExplosiveTimer.CanStart(), isDisabled: false, highExplosiveTimer.GetRechargeNormalised(), highExplosiveTimer.GetInUseNormalised());
		bool flag2 = m_bomberHasMissiles && stationGunner.GetMissileLauncher().ShouldShowMissileLauncherSettings();
		if (m_showingMissile != flag2)
		{
			m_showingMissile = flag2;
			m_showMissileOptionsHierarchy.SetActive(flag2);
		}
		int num = (m_bomberHasMissiles ? stationGunner.GetMissileLauncher().GetNumMissilesRemaining() : 0);
		if (num != m_lastMissiles)
		{
			m_missileCounter.SetText(num.ToString());
			m_lastMissiles = num;
		}
		SkillRefreshTimer.SkillTimer missileTimer = stationGunner.GetMissileTimer();
		bool isDisabled = false;
		if (num == 0)
		{
			isDisabled = true;
		}
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			isDisabled = true;
		}
		m_missileLaunchButton.SetStatus(missileTimer.IsActive(), missileTimer.CanStart(), isDisabled, missileTimer.GetRechargeNormalised(), missileTimer.GetInUseNormalised());
	}

	private void Update()
	{
		Refresh();
	}
}
