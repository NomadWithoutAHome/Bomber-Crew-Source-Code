using UnityEngine;

public class ShowTaggedItemHealth : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar m_healthbar;

	[SerializeField]
	private TagUIProgress m_tag;

	private void Update()
	{
		TaggableItem taggableItem = m_tag.GetTaggableItem();
		if (taggableItem != null)
		{
			SmoothDamageable component = taggableItem.GetComponent<SmoothDamageable>();
			m_healthbar.Value = component.GetHealthNormalised();
		}
	}
}
