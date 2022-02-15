using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Rewired;

public class MainActionButtonMonitor : Singleton<MainActionButtonMonitor>
{
	public enum ButtonPress
	{
		Confirm,
		Back,
		Start,
		LeftAction,
		TopAction,
		LeftStart,
		ConfirmDown,
		LeftActionDown,
		TopActionDown,
		LeftStartDown,
		BackDown
	}

	public List<Func<ButtonPress, bool>> m_currentListeners = new List<Func<ButtonPress, bool>>();

	private bool m_invalidateCurrentFrame;

	public void AddListener(Func<ButtonPress, bool> callback)
	{
		m_currentListeners.Add(callback);
	}

	public void RemoveListener(Func<ButtonPress, bool> callback, bool invalidateCurrentPress)
	{
		if (invalidateCurrentPress)
		{
			m_invalidateCurrentFrame = true;
		}
		m_currentListeners.Remove(callback);
	}

	private void DoPress(ButtonPress bp)
	{
		int num = m_currentListeners.Count - 1;
		bool flag;
		do
		{
			flag = m_currentListeners[num](bp);
			num--;
		}
		while (!flag && num != -1);
	}

	public void InvalidateCurrentFrame()
	{
		m_invalidateCurrentFrame = true;
	}

	private void Update()
	{
		if (m_currentListeners.Count > 0 && !m_invalidateCurrentFrame)
		{
			if (ReInput.players.GetPlayer(0).GetButtonUp(3))
			{
				DoPress(ButtonPress.Back);
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(3))
			{
				DoPress(ButtonPress.BackDown);
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(2))
			{
				DoPress(ButtonPress.ConfirmDown);
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(2))
			{
				DoPress(ButtonPress.Confirm);
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(21))
			{
				DoPress(ButtonPress.Start);
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(44))
			{
				DoPress(ButtonPress.LeftActionDown);
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(44))
			{
				DoPress(ButtonPress.LeftAction);
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(46))
			{
				DoPress(ButtonPress.TopActionDown);
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(46))
			{
				DoPress(ButtonPress.TopAction);
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(47))
			{
				DoPress(ButtonPress.LeftStart);
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(47))
			{
				DoPress(ButtonPress.LeftStartDown);
			}
		}
		m_invalidateCurrentFrame = false;
	}
}
