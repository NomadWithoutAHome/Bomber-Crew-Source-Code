using UnityEngine;

public class BomberUpgradeHangarBomberPositionController : MonoBehaviour
{
	[SerializeField]
	private Animation m_bomberPositionAnimation;

	[SerializeField]
	private GameObject m_bomberVisibleHierarchy;

	[SerializeField]
	private AnimationClip m_bomberArriveAnimation;

	public void SetBomberVisible(bool visible)
	{
		m_bomberVisibleHierarchy.SetActive(visible);
	}

	public void PlayArriveAnimation()
	{
		SetBomberVisible(visible: true);
		m_bomberPositionAnimation.Play(m_bomberArriveAnimation.name);
	}
}
