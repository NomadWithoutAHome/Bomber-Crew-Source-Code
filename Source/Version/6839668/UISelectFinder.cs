using UnityEngine;

public abstract class UISelectFinder : MonoBehaviour
{
	[SerializeField]
	private GameObject m_pointingWidget;

	[SerializeField]
	private UISelectorMovementType m_movementType;

	public abstract tk2dUIItem[] GetAllItems();

	public abstract bool DoesItemMatch(tk2dUIItem toMatch);

	public UISelectorMovementType GetMovementType()
	{
		return m_movementType;
	}

	public GameObject GetPointingWidgetPrefab()
	{
		return m_pointingWidget;
	}
}
