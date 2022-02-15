using UnityEngine;

public class CrewmanGraphicsInstantiate : MonoBehaviour
{
	[SerializeField]
	private GameObject m_crewmanGraphicsPrefabM;

	private CrewmanGraphics m_instantiatedGraphics;

	public void Init(Crewman cm)
	{
		GameObject gameObject = Object.Instantiate(m_crewmanGraphicsPrefabM);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		m_instantiatedGraphics = gameObject.GetComponent<CrewmanGraphics>();
	}

	public CrewmanGraphics GetCrewmanGraphics()
	{
		return m_instantiatedGraphics;
	}
}
