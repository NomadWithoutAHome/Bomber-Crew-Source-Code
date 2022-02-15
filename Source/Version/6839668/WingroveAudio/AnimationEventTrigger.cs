using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Triggers/Animation Event Trigger")]
public class AnimationEventTrigger : MonoBehaviour
{
	[SerializeField]
	[AudioEventName]
	private string m_audioEvent = string.Empty;

	public void OnAnimationTrigger()
	{
		WingroveRoot.Instance.PostEvent(m_audioEvent);
	}
}
