using System;
using System.Collections.Generic;
using BomberCrewCommon;

public class SystemLoader : Singleton<SystemLoader>
{
	private List<LoadableSystem> m_allLoadableSystems = new List<LoadableSystem>();

	private bool m_allHaveLoaded = true;

	private LoadableSystem m_currentlyLoading;

	private DateTime m_startTime;

	private bool m_splashScreensPresent;

	private bool m_pauseLoading;

	public event Action OnLoadingStart;

	public event Action OnLoadingStop;

	public void RegisterLoadableSystem(LoadableSystem ls)
	{
		m_allLoadableSystems.Add(ls);
		m_allHaveLoaded = false;
		if (this.OnLoadingStart != null)
		{
			this.OnLoadingStart();
		}
	}

	public void SetSplashSequence(bool splashSequence)
	{
		m_splashScreensPresent = splashSequence;
	}

	public void SetPauseLoading(bool pause)
	{
		m_pauseLoading = pause;
	}

	public bool IsPaused()
	{
		return m_pauseLoading && m_currentlyLoading == null;
	}

	public bool IsLoadComplete()
	{
		return m_allHaveLoaded && !m_splashScreensPresent;
	}

	public void ResetHaveAllLoaded()
	{
		m_allHaveLoaded = false;
	}

	private void Update()
	{
		if (m_currentlyLoading != null)
		{
			if (m_currentlyLoading.IsLoadComplete())
			{
				TimeSpan timeSpan = DateTime.UtcNow - m_startTime;
				m_currentlyLoading = null;
			}
			else
			{
				m_currentlyLoading.ContinueLoad();
			}
		}
		else
		{
			if (m_allHaveLoaded || m_pauseLoading)
			{
				return;
			}
			LoadableSystem loadableSystem = null;
			foreach (LoadableSystem allLoadableSystem in m_allLoadableSystems)
			{
				if (allLoadableSystem.IsLoadComplete())
				{
					continue;
				}
				bool flag = true;
				if (allLoadableSystem.GetDependencies() != null)
				{
					LoadableSystem[] dependencies = allLoadableSystem.GetDependencies();
					foreach (LoadableSystem loadableSystem2 in dependencies)
					{
						if (!loadableSystem2.IsLoadComplete())
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					loadableSystem = allLoadableSystem;
					break;
				}
			}
			if (loadableSystem == null)
			{
				m_allHaveLoaded = true;
				if (this.OnLoadingStop != null)
				{
					this.OnLoadingStop();
				}
			}
			else
			{
				m_currentlyLoading = loadableSystem;
				m_startTime = DateTime.UtcNow;
				m_currentlyLoading.StartLoad();
			}
		}
	}
}
