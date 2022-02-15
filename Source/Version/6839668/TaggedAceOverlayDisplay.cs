using UnityEngine;

public class TaggedAceOverlayDisplay : MonoBehaviour
{
	[SerializeField]
	private TagUIProgress m_tag;

	[SerializeField]
	private TextSetter m_aceName;

	[SerializeField]
	private tk2dUIProgressBar m_healthbar;

	private void Update()
	{
		TaggableItem taggableItem = m_tag.GetTaggableItem();
		if (taggableItem != null)
		{
			AceFighterInMission component = taggableItem.GetComponent<AceFighterInMission>();
			EnemyFighterAce aceInformation = component.GetAceInformation();
			m_aceName.SetText(aceInformation.GetFirstName() + " " + aceInformation.GetSurname());
			FighterPlane component2 = taggableItem.GetComponent<FighterPlane>();
			m_healthbar.Value = component2.GetHealthNormalised();
		}
	}
}
