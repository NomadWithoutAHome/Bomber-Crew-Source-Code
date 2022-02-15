using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class FreeLookCamera : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] m_camerasToDisable;

	[SerializeField]
	private Animator m_animatorToDisable;

	[SerializeField]
	private float m_moveSpeedRegular;

	[SerializeField]
	private float m_moveSpeedRun;

	[SerializeField]
	private tk2dCamera m_camera;

	[SerializeField]
	private Camera m_cameraAlt;

	[SerializeField]
	private DepthOfField m_depthOfField;

	private bool m_enabled;

	private bool m_lockedToBomber;

	private bool m_lockLockAtBomber;

	private bool m_noMessages;

	private string m_msg;

	private float m_messageTime;

	private bool m_mouseCursorEnabled;
}
