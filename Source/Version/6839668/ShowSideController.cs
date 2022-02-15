using BomberCrewCommon;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowSideController : MonoBehaviour
{
	[SerializeField]
	private Renderer[] m_renderersLeft;

	[SerializeField]
	private Renderer[] m_renderersRight;

	private bool m_rightSideIsShowing = true;

	private bool m_leftSideIsShowing = true;

	private BomberState m_bomberState;

	private void Start()
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
	}

	private void Update()
	{
		if (Singleton<BomberCamera>.Instance != null)
		{
			ShowSide(Singleton<BomberCamera>.Instance.ShouldShowSides(), bomberIsFacingRight: true);
		}
		else
		{
			ShowSide(show: true, bomberIsFacingRight: true);
		}
	}

	public void ShowSide(bool show, bool bomberIsFacingRight)
	{
		bool flag = !bomberIsFacingRight || show;
		bool flag2 = bomberIsFacingRight || show;
		if (m_rightSideIsShowing != flag)
		{
			for (int i = 0; i < m_renderersRight.Length; i++)
			{
				if (flag)
				{
					m_renderersRight[i].shadowCastingMode = ShadowCastingMode.On;
				}
				else
				{
					m_renderersRight[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}
			m_rightSideIsShowing = flag;
		}
		if (m_leftSideIsShowing == flag2)
		{
			return;
		}
		for (int j = 0; j < m_renderersLeft.Length; j++)
		{
			if (flag2)
			{
				m_renderersLeft[j].shadowCastingMode = ShadowCastingMode.On;
			}
			else
			{
				m_renderersLeft[j].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
		}
		m_leftSideIsShowing = flag2;
	}
}
